﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Must.Models
{
    public partial class PV1800
    {
        [SensorInterpretation("battery", "%")]
        public short? BatteryPercent
        {
            get
            {
                if (!BatteryVoltage.HasValue || !WorkStateNo.HasValue || !ChrWorkstateNo.HasValue || !BattVoltageGrade.HasValue)
                {
                    return null;
                }

                var batteryVoltage = this.BatteryVoltage.Value;
                //var batteryVoltageGrade = this.BattVoltageGrade.Value;
                //var batteryCellCount = batteryVoltageGrade / 2; // Assume 2 volt cells. So a 12 volt battery will have 6 cells.
                //var cellVoltage = batteryVoltage / batteryCellCount;
                var batteryMode = this.WorkStateNo == 2; // In battery mode. Battery is being used
                var charging = this.ChrWorkstateNo == 2; // Battery is being charged
                var batteryLoaded = batteryMode && !charging; // Load is supported by battery with no charging

                // if in line mode or battery is charing then don't use load
                // if battery is not charging and in battery mode then use load

                // Battery is not charging
                if (batteryLoaded)
                {
                    if (!LoadPercent.HasValue)
                    {
                        return null;
                    }
                    /*
                    var loadPercentage = this.LoadPercent.Value;

                    if (loadPercentage < 20)
                    {
                        return CalculateBatteryPercent(1.701d, 2.033d, 0.083, 4, cellVoltage);
                    }

                    if (loadPercentage < 50)
                    {
                        return CalculateBatteryPercent(1.651d, 1.983d, 0.083, 4, cellVoltage);
                    }

                    return CalculateBatteryPercent(1.551d, 1.883d, 0.083, 4, cellVoltage);
                    */
                    return CalculateBatteryPercent(batteryVoltage);
                }

                // We are charging
                if (!batteryLoaded)
                {
                    //return CalculateBatteryPercent(1.917d, 2.25d, 0.083, 4, cellVoltage);
                    return CalculateBatteryPercent(batteryVoltage);
                }

                return null;
            }
        }

        //private static short CalculateBatteryPercent(double lower, double upper, double interval, int intervals, double currentVoltage)
        private static short CalculateBatteryPercent(double currentVoltage)
        {
            double[,] voltageGrades = new double[5,4] { 
                { 29.2, 26.9, 100.0, 99.0}, 
                { 26.9, 25.6,  99.0, 17.0},
                { 25.6, 25.0,  17.0, 14.0},
                { 25.0, 24.0,  14.0,  9.0},
                { 24.0, 20.0,   9.0,  0.0}
            };

            var coeficient = (double) 1.0;
            
            for (int i = 0; i < voltageGrades.Length; i++)
            {
                if (voltageGrades[i, 0] >  currentVoltage || currentVoltage <= voltageGrades[i, 1]) {

                   coeficient =  (voltageGrades[i, 0] - voltageGrades[i, 1])/(voltageGrades[i, 2] - voltageGrades[i, 3]);
                   // we have delta by voltage is 1.3v and delta by percentage 82%, 
                   return  ((short)Math.Round((currentVoltage - voltageGrades[i, 1])/coeficient, 0));
                }
            }
/*
            if (cellVoltage >= upper) return 100;

            var threshold = upper;
            var percentageBase = 100d;
            var percentageInterval = 100d / (double)intervals;
            for (var level = 0; level < intervals; level++)
            {
                threshold -= (level * interval);
                percentageBase -= (level * percentageInterval);

                if (cellVoltage >= threshold)
                {
                    return (short)(percentageBase + Math.Round(percentageInterval * (cellVoltage - threshold) / interval, 0,
                        MidpointRounding.AwayFromZero));
                }
            }
*/
            return 100;
        }

        [SensorInterpretation("chart-bell-curve-cumulative", "KWH")]
        public double? AccumulatedChargerPower
        {
            get
            {
                return ((AccumulatedChargerPowerH ?? 0d) * 1000d) + ((AccumulatedChargerPowerL ?? 0d) * 0.1d);
            }
        }

        [SensorInterpretation("chart-bell-curve-cumulative", "KWH")]
        public double? AccumulatedDischargerPower
        {
            get
            {
                return ((AccumulatedDischargerPowerH ?? 0d) * 1000d) + ((AccumulatedDischargerPowerL ?? 0d) * 0.1d);
            }
        }

        [SensorInterpretation("chart-bell-curve-cumulative", "KWH")]
        public double? AccumulatedBuyPower
        {
            get
            {
                return ((AccumulatedBuyPowerH ?? 0d) * 1000d) + ((AccumulatedBuyPowerL ?? 0d) * 0.1d);
            }
        }

        [SensorInterpretation("chart-bell-curve-cumulative", "KWH")]
        public double? AccumulatedSellPower
        {
            get
            {
                return ((AccumulatedSellPowerH ?? 0d) * 1000d) + ((AccumulatedSellPowerL ?? 0d) * 0.1d);
            }
        }

        [SensorInterpretation("chart-bell-curve-cumulative", "KWH")]
        public double? AccumulatedLoadPower
        {
            get
            {
                return ((AccumulatedLoadPowerH ?? 0d) * 1000d) + ((AccumulatedLoadPowerL ?? 0d) * 0.1d);
            }
        }

        [SensorInterpretation("chart-bell-curve-cumulative", "KWH")]
        public double? AccumulatedSelfusePower
        {
            get
            {
                return ((AccumulatedSelfusePowerH ?? 0d) * 1000d) + ((AccumulatedSelfusePowerL ?? 0d) * 0.1d);
            }
        }

        [SensorInterpretation("chart-bell-curve-cumulative", "KWH")]
        public double? AccumulatedPvsellPower
        {
            get
            {
                return ((AccumulatedPowellPowerH ?? 0d) * 1000d) + ((AccumulatedPvsellPowerL ?? 0d) * 0.1d);
            }
        }

        [SensorInterpretation("chart-bell-curve-cumulative", "KWH")]
        public double? AccumulatedGridChargerPower
        {
            get
            {
                return ((AccumulatedGridChargerPowerH ?? 0d) * 1000d) + ((AccumulatedGridChargerPowerL ?? 0d) * 0.1d);
            }
        }

        [SensorInterpretation("chart-bell-curve-cumulative", "KWH")]
        public double? AccumulatedPvPower
        {
            get
            {
                return ((AccumulatedPvPowerH ?? 0d) * 1000d) + ((AccumulatedPvPowerL ?? 0d) * 0.1d);
            }
        }

    }
}
