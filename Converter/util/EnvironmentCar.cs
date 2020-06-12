using System;
using System.Collections.Generic;
using System.Text;

namespace Converter.util
{
    public enum EnvironmentCar
    {
        SnowCar,
        DesertCar,
        RallyCar,
        BayCar,
        CoastCar,
        IslandCar
    }

    public static class EnvironmentCarExt
    {
        public static MapEnvironment GetEnvironment(this EnvironmentCar car)
        {
            return car switch
            {
                EnvironmentCar.SnowCar   => MapEnvironment.Alpine,
                EnvironmentCar.DesertCar => MapEnvironment.Speed,
                EnvironmentCar.RallyCar  => MapEnvironment.Rally,
                EnvironmentCar.BayCar    => MapEnvironment.Bay,
                EnvironmentCar.CoastCar  => MapEnvironment.Coast,
                EnvironmentCar.IslandCar => MapEnvironment.Island,
                _ => throw new Exception("Hmm"),
            };
        }
    }
}
