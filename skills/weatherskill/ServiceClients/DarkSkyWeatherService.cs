using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DarkSky;
using DarkSky.Services;
using static DarkSky.Services.DarkSkyService;

namespace weatherskill.ServiceClients
{
    public class DarkSkyWeatherService : IWeatherForecast,IWeatherWearSuggestion
    {
        private DarkSkyService _darkSky;

        public DarkSkyWeatherService(string key)
        {
            _darkSky = new DarkSkyService(key);
        }

        async public Task<string> GenerateForcastMessageDaily(string location, DateTime? startdate = null)
        {
            try
            {
                var geo = await ServiceManager.GeoService.QueryGeoInfoByLocation(location);
                if (geo != null)
                {
                    OptionalParameters param = new OptionalParameters { MeasurementUnits = "uk2" }; 
                    if (startdate != null && startdate.HasValue)
                    {
                        param = new OptionalParameters { ForecastDateTime = startdate, ExtendHourly = false, MeasurementUnits = "uk2" };
                    }

                    var response = await _darkSky.GetForecast(geo.Latitude, geo.Longitude, param);
                    if (response.IsSuccessStatus)
                    {
                        string message = $"In {geo.Location}, {response.Response.Daily.Summary}";
                        if (startdate!=null && startdate.HasValue)
                        {
                            message = $"At {geo.Location} on {startdate.Value.ToString("d")}, {response.Response.Hourly.Summary}";
                            if (response.Response.Currently.Temperature.HasValue)
                            {
                                message += $" The average temperature will be {response.Response.Currently.Temperature.Value}°C.";
                            }
                            if (response.Response.Currently.Humidity.HasValue)
                            {
                                message += $" The humidity will be around {response.Response.Currently.Humidity.Value*100}%.";
                            }
                            message += ".";
                        }

                        return message;
                    }
                    else
                    {
                        return response.ResponseReasonPhrase;
                    }
                }
                else throw new Exception($"Failed to find a location as {location}.");

            }
            catch (Exception err)
            {
                return err.Message;
            }
        }

        async public Task<string> GenerateForcastMessageHourly(string location, DateTime? starttime = null)
        {
            try
            {
                var geo = await ServiceManager.GeoService.QueryGeoInfoByLocation(location);
                if (geo != null)
                {
                    OptionalParameters param = null;
                    if (starttime != null && starttime.HasValue)
                    {
                        param = new OptionalParameters { ForecastDateTime = starttime, ExtendHourly = true };
                    }

                    var response = await _darkSky.GetForecast(geo.Latitude, geo.Longitude, param);
                    if (response.IsSuccessStatus)
                    {
                        string message = $"In {geo.Location}, {response.Response.Currently.Summary}";
                        if (starttime != null && starttime.HasValue)
                        {
                            message = $"At {geo.Location} from {starttime.Value.ToString("g")}, {response.Response.Hourly.Summary}";
                        }

                        return message;
                    }
                    else
                    {
                        return response.ResponseReasonPhrase;
                    }
                }
                else throw new Exception($"Failed to find a location as {location}.");

            }
            catch (Exception err)
            {
                return err.Message;
            }
        }

        public async Task<string> GenerateWearSuggestion(string location, DateTime? startdate, string[] clothes)
        {
            try
            {
                var geo = await ServiceManager.GeoService.QueryGeoInfoByLocation(location);
                if (geo != null)
                {
                    var param = new OptionalParameters { ForecastDateTime = DateTime.Now, MeasurementUnits = "uk2" };
                    if (startdate != null && startdate.HasValue)
                    {
                        param.ForecastDateTime = startdate.Value;
                    }

                    var response = await _darkSky.GetForecast(geo.Latitude, geo.Longitude, param);
                    if (response.IsSuccessStatus && response.Response.Currently.Temperature.HasValue)
                    {
                        string message = $"In {geo.Location}, the average temperature will be around {response.Response.Currently.Temperature.Value}°C at {startdate.Value.ToString("d")}.";

                        foreach (var name in clothes)
                        {
                            var range = WeatherclothesList.GetTempRange(name);
                            if (range != null)
                            {
                                if (response.Response.Currently.Temperature.Value < range.LowTemperature)
                                {
                                    message += $"And it will be too cold to wear {name}.";
                                }
                                else if (response.Response.Currently.Temperature.Value > range.HighTemperature)
                                {
                                    message += $"And it is not a good idea to wear {name} in a hot day.";
                                }
                                else
                                {
                                    message += $"And it is good to wear {name}.";
                                }
                            }
                        }

                        return message;
                    }
                    else
                    {
                        return response.ResponseReasonPhrase;
                    }
                }
                else throw new Exception($"Failed to find a location as {location}.");

            }
            catch (Exception err)
            {
                return err.Message;
            }
        }        
    }
}
