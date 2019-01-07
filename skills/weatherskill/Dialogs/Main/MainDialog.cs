// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using weatherskill.Dialogs.Main.Resources;
using weatherskill.Dialogs.Shared.Resources;
using Luis;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Solutions;
using Microsoft.Bot.Solutions.Dialogs;
using Microsoft.Bot.Solutions.Extensions;
using Microsoft.Bot.Solutions.Skills;

namespace weatherskill
{
    public class MainDialog : RouterDialog
    {
        private bool _skillMode;
        private ISkillConfiguration _services;
        private UserState _userState;
        private ConversationState _conversationState;
        private IServiceManager _serviceManager;
        private IStatePropertyAccessor<weatherskillState> _stateAccessor;
        private IStatePropertyAccessor<DialogState> _dialogStateAccessor;
        private weatherskillResponseBuilder _responseBuilder = new weatherskillResponseBuilder();
        private string localfixed = "en";
        private Dictionary<weather.Intent, string> _mapDialog;

        public MainDialog(SkillConfiguration services, ConversationState conversationState, UserState userState, IBotTelemetryClient telemetryClient, IServiceManager serviceManager, bool skillMode)
            : base(nameof(MainDialog), telemetryClient)
        {
            _skillMode = skillMode;
            _services = services;
            _conversationState = conversationState;
            _userState = userState;
            _serviceManager = serviceManager;

            // Initialize state accessor
            _stateAccessor = _conversationState.CreateProperty<weatherskillState>(nameof(weatherskillState));
            _dialogStateAccessor = _conversationState.CreateProperty<DialogState>(nameof(DialogState));

            RegisterDialogs();
        }

        protected override async Task OnStartAsync(DialogContext dc, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!_skillMode)
            {
                // send a greeting if we're in local mode
                await dc.Context.SendActivityAsync(dc.Context.Activity.CreateReply(weatherskillMainResponses.WelcomeMessage));
            }
        }

        protected override async Task RouteAsync(DialogContext dc, CancellationToken cancellationToken = default(CancellationToken))
        {
            var state = await _stateAccessor.GetAsync(dc.Context, () => new weatherskillState());

            // get current activity locale
            // var locale = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            var locale = localfixed;
            var localeConfig = _services.LocaleConfigurations[locale];

            // If dispatch result is general luis model
            localeConfig.LuisServices.TryGetValue("weather", out var luisService);

            if (luisService == null)
            {
                throw new Exception("The specified LUIS Model could not be found in your Bot Services configuration.");
            }
            else
            {
                var result = await luisService.RecognizeAsync<weather>(dc.Context, CancellationToken.None);
                var intent = result?.TopIntent().intent;

                var skillOptions = new weatherskillDialogOptions
                {
                    SkillMode = _skillMode,
                };

                // switch on general intents
                switch (intent)
                {
	     case weather.Intent.Weather_GetForecast:
                        {
                         //   await dc.BeginDialogAsync(nameof(weatherforecastDialog), skillOptions);
                           
                            state.Clear();
                            state.LastIntent = intent;

                            await dc.BeginDialogAsync(_mapDialog[intent.Value], skillOptions);
                            break;
                        }

	    case weather.Intent.Weather_Wear:
                        {
                            state.LastIntent = intent;
                            await dc.BeginDialogAsync(_mapDialog[intent.Value], skillOptions);
                            break;
                        }

                    case weather.Intent.Weather_ContextContinue:
                        {
                            if (state.LastIntent.HasValue)
                            {
                                await dc.BeginDialogAsync(_mapDialog[state.LastIntent.Value], skillOptions);
                            }
                            else
                            {
                                await dc.BeginDialogAsync(_mapDialog[weather.Intent.Weather_GetForecast], skillOptions);
                            }
                            break;
                        }

                    case weather.Intent.None:
                        {
                            await dc.Context.SendActivityAsync(dc.Context.Activity.CreateReply(weatherskillSharedResponses.DidntUnderstandMessage));
                            if (_skillMode)
                            {
                                await CompleteAsync(dc);
                            }

                            break;
                        }

                    default:
                        {
                            await dc.Context.SendActivityAsync(dc.Context.Activity.CreateReply(weatherskillMainResponses.FeatureNotAvailable));

                            if (_skillMode)
                            {
                                await CompleteAsync(dc);
                            }

                            break;
                        }
                }
            }
        }

        protected override async Task CompleteAsync(DialogContext dc, DialogTurnResult result = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_skillMode)
            {
                var response = dc.Context.Activity.CreateReply();
                response.Type = ActivityTypes.EndOfConversation;

                await dc.Context.SendActivityAsync(response);
            }
            else
            {
                await dc.Context.SendActivityAsync(dc.Context.Activity.CreateReply(weatherskillSharedResponses.ActionEnded));
            }

            // End active dialog
            await dc.EndDialogAsync(result);
        }

        protected override async Task OnEventAsync(DialogContext dc, CancellationToken cancellationToken = default(CancellationToken))
        {
            switch (dc.Context.Activity.Name)
            {
                case Events.SkillBeginEvent:
                    {
                        var state = await _stateAccessor.GetAsync(dc.Context, () => new weatherskillState());

                        if (dc.Context.Activity.Value is Dictionary<string, object> userData)
                        {
                            // capture any user data sent to the skill from the parent here.
                        }

                        break;
                    }

                case Events.TokenResponseEvent:
                    {
                        // Auth dialog completion
                        var result = await dc.ContinueDialogAsync();

                        // If the dialog completed when we sent the token, end the skill conversation
                        if (result.Status != DialogTurnStatus.Waiting)
                        {
                            var response = dc.Context.Activity.CreateReply();
                            response.Type = ActivityTypes.EndOfConversation;

                            await dc.Context.SendActivityAsync(response);
                        }
                        break;
                    }
            }
        }

        protected override async Task<InterruptionAction> OnInterruptDialogAsync(DialogContext dc, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = InterruptionAction.NoAction;

            if (dc.Context.Activity.Type == ActivityTypes.Message)
            {
                // get current activity locale
                  var locale = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
      //          var locale = localfixed; 
                var localeConfig = _services.LocaleConfigurations[locale];

                // Update state with luis result and entities
                var skillLuisResult = await localeConfig.LuisServices["weather"].RecognizeAsync<weather>(dc.Context, cancellationToken);
                var state = await _stateAccessor.GetAsync(dc.Context, () => new weatherskillState());
                state.LuisResult = skillLuisResult;

                // check luis intent
                localeConfig.LuisServices.TryGetValue("general", out var luisService);

                if (luisService == null)
                {
                    throw new Exception("The specified LUIS Model could not be found in your Skill configuration.");
                }
                else
                {
                    var luisResult = await luisService.RecognizeAsync<General>(dc.Context, cancellationToken);
	    state.GeneralLuisResult = luisResult;
                    var topIntent = luisResult.TopIntent().intent;

                    // check intent
                    switch (topIntent)
                    {
                        case General.Intent.Cancel:
                            {
                                result = await OnCancel(dc);
                                break;
                            }

                        case General.Intent.Help:
                            {
                                result = await OnHelp(dc);
                                break;
                            }

                        case General.Intent.Logout:
                            {
                                result = await OnLogout(dc);
                                break;
                            }
                    }
                }
            }

            return result;
        }

        private async Task<InterruptionAction> OnCancel(DialogContext dc)
        {
            await dc.BeginDialogAsync(nameof(CancelDialog));
            return InterruptionAction.StartedDialog;
        }

        private async Task<InterruptionAction> OnHelp(DialogContext dc)
        {
            await dc.Context.SendActivityAsync(dc.Context.Activity.CreateReply(weatherskillMainResponses.HelpMessage));
            return InterruptionAction.MessageSentToUser;
        }

        private async Task<InterruptionAction> OnLogout(DialogContext dc)
        {
            BotFrameworkAdapter adapter;
            var supported = dc.Context.Adapter is BotFrameworkAdapter;
            if (!supported)
            {
                throw new InvalidOperationException("OAuthPrompt.SignOutUser(): not supported by the current adapter");
            }
            else
            {
                adapter = (BotFrameworkAdapter)dc.Context.Adapter;
            }

            await dc.CancelAllDialogsAsync();

            // Sign out user
            var tokens = await adapter.GetTokenStatusAsync(dc.Context, dc.Context.Activity.From.Id);

            // await adapter.SignOutUserAsync(dc.Context, _services.AuthConnectionName);
            foreach (var token in tokens)
            {
                await adapter.SignOutUserAsync(dc.Context, token.ConnectionName);
            }
            await dc.Context.SendActivityAsync(dc.Context.Activity.CreateReply(weatherskillMainResponses.LogOut));

            return InterruptionAction.StartedDialog;
        }

            private void RegisterDialogs()
            {
                _mapDialog = new Dictionary<weather.Intent, string>();

                AddDialog(new weatherforecastDialog(_services, _stateAccessor, _dialogStateAccessor,_serviceManager, TelemetryClient));
                _mapDialog.Add(weather.Intent.Weather_GetForecast, nameof(weatherforecastDialog));

               AddDialog(new weatherwearDialog(_services, _stateAccessor, _dialogStateAccessor, _serviceManager,TelemetryClient));
            _mapDialog.Add(weather.Intent.Weather_Wear, nameof(weatherwearDialog));

  
                AddDialog(new CancelDialog());
            }


        private class Events
        {
            public const string TokenResponseEvent = "tokens/response";
            public const string SkillBeginEvent = "skillBegin";
        }
    }
}
