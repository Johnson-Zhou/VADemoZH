using Microsoft.Bot.Solutions.Skills;
using weatherskill.ServiceClients;

namespace weatherskill
{
    public class ServiceManager : IServiceManager
    {
        private ISkillConfiguration _skillConfig;
        
        //use bing map as geo service
        public static IGeoQueryService GeoService = new BingMapService("AgdkBEuF-jev-VA8YSeokteR0Kv4TZG9WSadc_jEZmEqZtz4QWTt3jU1sMGfvVev");

        //use darksky as weather forcast service
        public static IWeatherForecast ForcastService = new DarkSkyWeatherService("5ac8487620c5ec434fcd2dd87f77c997");

        //use darksky as weather wear suggestion service
      public static IWeatherWearSuggestion WearSuggestionService = new DarkSkyWeatherService("5ac8487620c5ec434fcd2dd87f77c997");

        public ServiceManager(ISkillConfiguration config)
        {
            _skillConfig = config;
        }
    }
}
