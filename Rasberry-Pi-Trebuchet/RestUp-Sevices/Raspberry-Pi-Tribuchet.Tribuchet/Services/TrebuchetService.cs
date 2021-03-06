﻿
using Raspberry_Pi_Trebuchet.RestUp.Servos.Models;
using Raspberry_Pi_Trebuchet.RestUp.Servos.Services;
using Raspberry_Pi_Trebuchet.RestUp.Sonic.Models;
using Raspberry_Pi_Trebuchet.RestUp.Sonic.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raspberry_Pi_Trebuchet.RestUp.Tribuchet.Services
{
    public class TrebuchetService
    {

        private bool SetServo(string servoStatus, ServoStatusService servoStatusService)
        {

            var GetServoTask = servoStatusService.RetrieveServos();
            GetServoTask.Wait();
            var servo = GetServoTask.Result.FirstOrDefault();

            servo.ServoStatus = servoStatus;
            //Move the servo to 180 to fire the trebuchet

            var SetServoTask = servoStatusService.SetServo(servo);
            SetServoTask.Wait();


            return SetServoTask.Result;
        }

        public bool FireTrebuchet()
        {
            ServoStatusService servoStatusService = ServoStatusService.Instance;
            UltraSonicSensorService ultrasonicSensorService = UltraSonicSensorService.Instance;           

            if (ultrasonicSensorService.IsUltraSonicServiceRunning())
                return false;

            //start the run Create the thread to get the distance of the trebuchet arm
            UltraSonicRunRequest runrequest = new UltraSonicRunRequest();
            runrequest.TimeInSecondsToRunSensor = 5;
            ultrasonicSensorService.StartUltraSonicRun(runrequest);          


            return SetServo("ONEEIGHTYDEGREES", servoStatusService);
        }

       

        public bool ResetTrebuchet()
        {
            ServoStatusService servoStatusService = ServoStatusService.Instance;
            return SetServo("ZERODEGREES", servoStatusService);
        }

    }
}
