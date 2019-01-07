using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using weatherskill.Dialogs.Forecast.Resources;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Solutions.Extensions;
using Microsoft.Bot.Solutions.Resources;
using Microsoft.Bot.Solutions.Skills;
using Microsoft.Bot.Solutions.Dialogs;

namespace weatherskill
{
    public class weatherwearDialog : weatherskillDialog
    {
        public weatherwearDialog(
            ISkillConfiguration services,
            IStatePropertyAccessor<weatherskillState> weatherStateAccessor,
            IStatePropertyAccessor<DialogState> dialogStateAccessor,
            IServiceManager serviceManager,
            IBotTelemetryClient telemetryClient)
            : base(nameof(weatherwearDialog), services, weatherStateAccessor, dialogStateAccessor, serviceManager, telemetryClient)
        {
	TelemetryClient = telemetryClient;

            var forecast = new WaterfallStep[]
           {
                CollectLocation,
                CollectDate,
                ShowForecastInfo
           };

            // Define the conversation flow using a waterfall model.
            AddDialog(new WaterfallDialog(Actions.Forecast, forecast) { TelemetryClient = telemetryClient });
            InitialDialogId = Actions.Forecast;
        }

        public async Task<DialogTurnResult> CollectLocation(WaterfallStepContext sc, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var state = await Accessor.GetAsync(sc.Context);

                if (state.Locations.Count == 0)
                {
                    //set the default location as shanghai
                    state.Locations = new List<string>() { "shanghai" };
                }

                return await sc.NextAsync();
            }
            catch (Exception ex)
            {
                throw await HandleDialogExceptions(sc, ex);
            }
        }

        public async Task<DialogTurnResult> CollectDate(WaterfallStepContext sc, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var state = await Accessor.GetAsync(sc.Context);

                if (state.ForecastTimes.Count == 0)
                {
                    //set the default date as today
                    state.ForecastTimes = new List<ForecastTime> { new ForecastTime { StartTime = DateTime.Now, Type = ForecastType.Day } };
                }

                return await sc.NextAsync();
            }
            catch (Exception ex)
            {
                throw await HandleDialogExceptions(sc, ex);
            }
        }

        public async Task<DialogTurnResult> ShowForecastInfo(WaterfallStepContext sc, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var state = await Accessor.GetAsync(sc.Context);

                foreach (var location in state.Locations)
                {
                    if (state.ForecastTimes.Count > 0)
                    {
                        var text = await ServiceManager.WearSuggestionService.GenerateWearSuggestion(location, state.ForecastTimes[0].StartTime, state.Clothes.ToArray());

                        var replyMessage = sc.Context.Activity.CreateReply(text);
                        await sc.Context.SendActivityAsync(replyMessage);
                    }
                }

                return await sc.EndDialogAsync(true);
            }
            catch (Exception ex)
            {
                throw await HandleDialogExceptions(sc, ex);
            }
        }
    }
}
