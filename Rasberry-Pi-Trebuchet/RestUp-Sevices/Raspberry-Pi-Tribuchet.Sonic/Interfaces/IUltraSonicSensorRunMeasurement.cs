﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raspberry_Pi_Trebuchet.RestUp.Sonic.Interfaces
{
    public interface IUltraSonicSensorRunMeasurement
    {
        int SonicMeasurementId { get; set; }
        DateTime TimeOfMeasurment { get; set; }
        double MeasurementDistance { get; set; }
        int UltraSonicSensorRunId { get; set; }
        string SonicGUID { get; set; }
        string MeasurementGUID { get; set; }
    }
}
