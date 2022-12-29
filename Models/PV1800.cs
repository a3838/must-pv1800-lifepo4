using System;
using System.Collections.Generic;
using System.Text;

namespace Must.Models
{
    public partial class PV1800
    {
        public const int batteryCapacity = 4150; //Wh, 5120Wh * 0.9 * 0.9;

        private double _initialDischarginPower;
        private short? _previousWorkStateNo;

        [SensorInterpretation("chart-bell-curve-cumulative", "KWH")]
        public double? DischarginPower 
        { 
            get 
            {
                if (_initialDischarginPower >= 0) {
                   return AccumulatedDischargerPower - _initialDischarginPower;
                }

                return null;
            }
        }

        private short? SetCharginStatus 
        { 
            set
            {
                if (value == 2 && _previousWorkStateNo != value) {
                    _previousWorkStateNo = value;
                    _initialDischarginPower = AccumulatedDischargerPower ?? 0;
                }

                if (value != 2 && _previousWorkStateNo != value)
                {
                    _previousWorkStateNo = value;
                    _initialDischarginPower = AccumulatedDischargerPower ?? 0;
                }
            }
        }

        [SensorInterpretation("battery", "%")]
        public short? BatteryPercentByCapacity
        {
            get
            {
                if (!BatteryVoltage.HasValue || !WorkStateNo.HasValue || !ChrWorkstateNo.HasValue || !BattVoltageGrade.HasValue)
                {
                    return null;
                }

                var batteryMode = this.WorkStateNo == 2; // In battery mode. Battery is being used
                var charging = this.ChrWorkstateNo == 2; // Battery is being charged
                var batteryLoaded = batteryMode && !charging; // Load is supported by battery with no charging

                // Battery is not charging
                if (batteryLoaded)
                {
                    if (!LoadPercent.HasValue)
                    {
                        return null;
                    }
                    
                    return (short)Math.Truncate((1 - (DischarginPower ?? 0/batteryCapacity)) * 100);
                }

                // We are charging
                //if (!batteryLoaded)
                //{
                //    return CalculateBatteryPercent(batteryVoltage);
                //}

                return null;
            }
        }

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
            double[,] voltageGrades = new double[4,4] { 
                { 26.4, 25.6,  99.0, 17.0},
                { 25.6, 25.0,  17.0, 14.0},
                { 25.0, 24.0,  14.0,  9.0},
                { 24.0, 20.0,   9.0,  0.0}
            };

            var coeficient = (double) 1.0;
            
            for (int i = 0; i < 4; i++)
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
