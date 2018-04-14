using System;

namespace CanonPhotoBooth
{
    public static class Config
    {
        public static double BicycleRimDiameter = 27.5; //inches
        public static double CaloriesPerMillisecond = 0.0001667;
        public static double DistancePerRevolution = (BicycleRimDiameter * 2.54) * Math.PI;

    }
}