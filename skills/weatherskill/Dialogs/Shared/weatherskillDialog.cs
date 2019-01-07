using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using weatherskill.Dialogs.Main.Resources;
using weatherskill.Dialogs.Shared.Resources;
using Luis;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Solutions;
using Microsoft.Bot.Solutions.Authentication;
using Microsoft.Bot.Solutions.Extensions;
using Microsoft.Bot.Solutions.Skills;
using Microsoft.Bot.Solutions.Util;
using Microsoft.Recognizers.Text;
using Microsoft.Recognizers.Text.DateTime;
using Newtonsoft.Json.Linq;
using static Microsoft.Recognizers.Text.Culture;



namespace weatherskill
{
    public class weatherskillDialog : ComponentDialog
    {
        // Constants
        public const string SkillModeAuth = "SkillAuth";
        public const string LocalModeAuth = "LocalAuth";

        // Fields, seems that new version dose not have those protected claims
      //  protected ISkillConfiguration _services;
      //  protected IStatePropertyAccessor<weatherskillState> _accessor;
      //  protected IServiceManager _serviceManager;
     
        public weatherskillDialog(
            string dialogId,
            ISkillConfiguration services,
            IStatePropertyAccessor<weatherskillState> accessor,
            IStatePropertyAccessor<DialogState> dialogStateAccessor,
            IServiceManager serviceManager,
            IBotTelemetryClient telemetryClient)
            : base(dialogId)
        {
            Services = services;
            Accessor = accessor;
            WeatherServiceManager = serviceManager;
            DialogStateAccessor = dialogStateAccessor;
            TelemetryClient = telemetryClient;
            
       ///     var oauthSettings = new OAuthPromptSettings()
        //    {
      //         ConnectionName = _services.AuthConnectionName ?? throw new Exception("The authentication connection has not been initialized."),
       //         Text = $"Authentication",
        //        Title = "Signin",
       //         Timeout = 300000, // User has 5 minutes to login
        //    };

            AddDialog(new EventPrompt(SkillModeAuth, "tokens/response", TokenResponseValidator));
      //      AddDialog(new OAuthPrompt(LocalModeAuth, oauthSettings, AuthPromptValidator));
        }

   protected weatherskillDialog(string dialogId)
            : base(dialogId)
        {
        }

    protected ISkillConfiguration Services { get; set; }

    protected IStatePropertyAccessor<weatherskillState> Accessor { get; set; }

    protected IStatePropertyAccessor<DialogState> DialogStateAccessor { get; set; }

   // protected IServiceManager ServiceManager { get; set; }

    protected IServiceManager WeatherServiceManager { get; set; }

        protected weatherskillResponseBuilder ResponseBuilder { get; set; } = new weatherskillResponseBuilder();
//    protected weatherskillResponseBuilder _responseBuilder = new weatherskillResponseBuilder();


        protected override async Task<DialogTurnResult> OnBeginDialogAsync(DialogContext dc, object options, CancellationToken cancellationToken = default(CancellationToken))
        {
            var state = await Accessor.GetAsync(dc.Context);
            await DigestLuisResult(dc, state.LuisResult);
            return await base.OnBeginDialogAsync(dc, options, cancellationToken);
        }

        protected override async Task<DialogTurnResult> OnContinueDialogAsync(DialogContext dc, CancellationToken cancellationToken = default(CancellationToken))
        {
            var state = await Accessor.GetAsync(dc.Context);
            await DigestLuisResult(dc, state.LuisResult);
            return await base.OnContinueDialogAsync(dc, cancellationToken);
        }

        // Shared steps
        public async Task<DialogTurnResult> GetAuthToken(WaterfallStepContext sc, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var skillOptions = (weatherskillDialogOptions)sc.Options;

                // If in Skill mode we ask the calling Bot for the token
                if (skillOptions != null && skillOptions.SkillMode)
                {
                    // We trigger a Token Request from the Parent Bot by sending a "TokenRequest" event back and then waiting for a "TokenResponse"
                    // TODO Error handling - if we get a new activity that isn't an event
                    var response = sc.Context.Activity.CreateReply();
                    response.Type = ActivityTypes.Event;
                    response.Name = "tokens/request";

                    // Send the tokens/request Event
                    await sc.Context.SendActivityAsync(response);

                    // Wait for the tokens/response event
                    return await sc.PromptAsync(SkillModeAuth, new PromptOptions());
                }
                else
                {
                    return await sc.PromptAsync(LocalModeAuth, new PromptOptions());
                }
            }
            catch (Exception ex)
            {
                throw await HandleDialogExceptions(sc, ex);
            }
        }

        public async Task<DialogTurnResult> AfterGetAuthToken(WaterfallStepContext sc, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                // When the user authenticates interactively we pass on the tokens/Response event which surfaces as a JObject
                // When the token is cached we get a TokenResponse object.
                var skillOptions = (weatherskillDialogOptions)sc.Options;
                TokenResponse tokenResponse;
                if (skillOptions != null && skillOptions.SkillMode)
                {
                    var resultType = sc.Context.Activity.Value.GetType();
                    if (resultType == typeof(TokenResponse))
                    {
                        tokenResponse = sc.Context.Activity.Value as TokenResponse;
                    }
                    else
                    {
                        var tokenResponseObject = sc.Context.Activity.Value as JObject;
                        tokenResponse = tokenResponseObject?.ToObject<TokenResponse>();
                    }
                }
                else
                {
                    tokenResponse = sc.Result as TokenResponse;
                }

                if (tokenResponse != null)
                {
                    var state = await Accessor.GetAsync(sc.Context);
                }

                return await sc.NextAsync();
            }
            catch (Exception ex)
            {
                throw await HandleDialogExceptions(sc, ex);
            }
        }

        // Validators
        private Task<bool> TokenResponseValidator(PromptValidatorContext<Activity> pc, CancellationToken cancellationToken)
        {
            var activity = pc.Recognized.Value;
            if (activity != null && activity.Type == ActivityTypes.Event)
            {
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
        }

        private Task<bool> AuthPromptValidator(PromptValidatorContext<TokenResponse> pc, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }


        // Helpers
        public async Task DigestLuisResult(DialogContext dc, weather luisResult)
        {
            try
            {
                var state = await Accessor.GetAsync(dc.Context);

                // extract entities and store in state here.
if (luisResult.Entities.Weather_Location!=null && luisResult.Entities.Weather_Location.Length>0)
                {
                    state.Locations.Clear();
                    foreach (var location in luisResult.Entities.Weather_Location)
                    {
                        if (!state.Locations.Contains(location))
                        {
                            state.Locations.Add(location);
                        }
                    }
                }

                if (luisResult.Entities.datetime!= null && luisResult.Entities.datetime.Length > 0)
                {
                    state.ForecastTimes.Clear();
                    foreach (var datespcs in luisResult.Entities.datetime)
                    {
                       switch (datespcs.Type)
                        {
                            case "date":
                                if (datespcs.Expressions.Count > 0)
                                {
                                    var forecast = new ForecastTime { StartTime = null, Type = ForecastType.Day };
                                    if (DateTime.TryParse(datespcs.Expressions[0], out DateTime time))
                                    {
                                        forecast.StartTime = time;
                                    }
                                    state.ForecastTimes.Add(forecast);
                                }
                                break;

                            case "datetime":
                                if (datespcs.Expressions.Count > 0)
                                {
                                    var forecast = new ForecastTime { StartTime = null, Type = ForecastType.Hour };
                                    if (DateTime.TryParse(datespcs.Expressions[0], out DateTime time))
                                    {
                                        forecast.StartTime = time;
                                    }
                                    state.ForecastTimes.Add(forecast);
                                }
                                break;
                        }                        
                    }
                }

        if (luisResult.Entities.Wear_Clothes != null && luisResult.Entities.Wear_Clothes.Length > 0)
                {
                    state.Clothes.Clear();
                    foreach (var cloth in luisResult.Entities.Wear_Clothes)
                    {
                        if (!state.Clothes.Contains(cloth))
                        {
                            state.Clothes.Add(cloth);
                        }
                    }
                }

            }
            catch
            {
                // put log here
            }
        }

        // This method is called by any waterfall step that throws an exception to ensure consistency
        public async Task<Exception> HandleDialogExceptions(WaterfallStepContext sc, Exception ex)
        {
            await sc.Context.SendActivityAsync(sc.Context.Activity.CreateReply(weatherskillSharedResponses.ErrorMessage));
            await sc.CancelAllDialogsAsync();
            return ex;
        }
    }
}
