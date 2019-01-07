using Luis;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Graph;
using System;
using System.Collections.Generic;

namespace weatherskill
{
    public enum ForecastType {Hour, Day};
    public class ForecastTime
    {
        public ForecastType Type;
        public DateTime? StartTime;
    }

    public class weatherskillState
    {
        public List<string> Locations;
        public List<ForecastTime> ForecastTimes;
        public List<string> Clothes;
        public weather.Intent? LastIntent;

        public weatherskillState()
        {
            Locations = new List<string>();
            ForecastTimes = new List<ForecastTime>();
            Clothes = new List<string>();
            LastIntent = null;
        }

        public void Clear()
        {
            Locations.Clear();
            ForecastTimes.Clear();
            Clothes.Clear();
            LastIntent = null;
        }

        public weather LuisResult { get; set; }

        public Luis.General GeneralLuisResult { get; set; }

    }
}
