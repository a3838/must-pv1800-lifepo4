using System;
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
                    
                    return CalculateBatteryPercent(batteryVoltage);
                }

                // We are charging
                if (!batteryLoaded)
                {
                    return CalculateBatteryPercent(batteryVoltage);
                }

                return null;
            }
        }

        private static short CalculateBatteryPercent(double currentVoltage)
        {
            // by information from https://footprinthero.com/lifepo4-battery-voltage-charts
            // for 24v LiFePO4 battery  
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
                if (voltageGrades[i, 0] >=  currentVoltage & currentVoltage > voltageGrades[i, 1]) {

                   coeficient =  (voltageGrades[i, 0] - voltageGrades[i, 1])/(voltageGrades[i, 2] - voltageGrades[i, 3]);

                   return  ((short)Math.Round(voltageGrades[i, 3] + (currentVoltage - voltageGrades[i, 1])/coeficient, 0));
                }
            }

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
