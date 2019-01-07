using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace weatherskill.ServiceClients
{
    public interface IWeatherForecast
    {
        Task<string> GenerateForcastMessageDaily(string location, DateTime? startdate);

        Task<string> GenerateForcastMessageHourly(string location, DateTime? starttime);

    }
}
