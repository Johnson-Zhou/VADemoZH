using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace weatherskill.ServiceClients
{
    public interface IWeatherWearSuggestion
    {
        Task<string> GenerateWearSuggestion(string location, DateTime? startdate, string[] clothes);
    }

    public class clothesTempRange
    {
        public string[] NameList;
        public double HighTemperature;
        public double LowTemperature;

        public clothesTempRange(double lowTemp, double HighTemp, params string[] nameList)
        {
            NameList = nameList;
            LowTemperature = lowTemp;
            HighTemperature = HighTemp;
        }

        public bool IsContain(string name)
        {
            var result = false;
            if (NameList != null)
            {
                foreach (var cloth in NameList)
                {
                    if (!string.IsNullOrEmpty(cloth) && string.Compare(cloth, name, true) == 0)
                    {
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }
    }

    public static class WeatherclothesList
    {
        static clothesTempRange[] _clothesList = new clothesTempRange[]
        {
            new clothesTempRange(25,100, "t-shirt","t-shirts","skirt","skirts","tshirt","tshirts","vest"),
            new clothesTempRange(15,25, "jacket","jackets"),
            new clothesTempRange(-100,15, "coat","coats","sweater","sweaters","cover","covers"),
        };

        static public clothesTempRange GetTempRange(string clothes)
        {
            clothesTempRange result = null;
            foreach (var range in _clothesList)
            {
               if (range.IsContain(clothes))
                {
                    result = range;
                }
            }
            return result;
        }
    }
}
