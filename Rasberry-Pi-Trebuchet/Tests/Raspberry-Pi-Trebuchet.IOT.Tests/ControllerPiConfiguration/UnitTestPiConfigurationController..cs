﻿using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Newtonsoft.Json;
using Rasberry_Pi_Trebuchet.Common.RestViewModels;
using Raspberry_Pi_Trebuchet.RestUp.Configuration.Controllers.api;
using Raspberry_Pi_Trebuchet.RestUp.Configuration.RestupHttpRequests;
using Raspberry_Pi_Trebuchet.RestUp.Configuration.RestViewModels;
using Raspberry_Pi_Trebuchet.RestUp.Configuration.Services;
using Restup.HttpMessage.Models.Schemas;
using Restup.Webserver.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Raspberry_Pi_Trebuchet.IOT.Tests.ControllerPiConfiguration
{
    /// <summary>
    /// Class for  test the piconfiguration service
    /// 
    ///  Something to NOTE is that the calling from code the "api" in
    /// /api/piconfiguration is not used, instead only use the route
    /// /piconfiguration ... All routes must be in lower case Restup
    /// is case sensitive.
    /// </summary>

    [TestClass]
    public class UnitTestPiConfigurationController
    {

        #region  GetMultiplePIConfigurationStatuses()
        /// <summary>
        /// Retrieve the list pi configuration data from the PiConfiguration 
        /// service.       
        /// </summary>
        [TestMethod]
        public void ConfigurationTest_GetMultiplePIConfigurationStatuses()
        {
            //Create the Rest Rout handler to process the request
            var restRouteHandler = new RestRouteHandler();
            restRouteHandler.RegisterController<PiConfigurationController>();

            //Send the Request to the route Handler
            RestUpHttpServerRequest basicGet = HttpRequestsConfiguration.GetRequestPIConfigurationStatuses();  
            var request = restRouteHandler.HandleRequest(basicGet);

            //Deserialize the returned Values
            var val = request.Result.Content.ToString();
            val = System.Text.Encoding.UTF8.GetString(request.Result.Content);
            var PiConfigurations = JsonConvert.DeserializeObject<List<ViewModelRestNameValuePair>>(val);


            AzurePiConfiguration azPiConfig = new AzurePiConfiguration();
            GetMultiplePIConfigurationStatusesValueCheck(nameof(azPiConfig.AllowSendingofData), PiConfigurations);
            GetMultiplePIConfigurationStatusesValueCheck(nameof(azPiConfig.AllowSendingToastLightData), PiConfigurations);
            GetMultiplePIConfigurationStatusesValueCheck(nameof(azPiConfig.AllowSendingToastServoData), PiConfigurations);
            GetMultiplePIConfigurationStatusesValueCheck(nameof(azPiConfig.AllowSendingUltraSonicData), PiConfigurations);
            GetMultiplePIConfigurationStatusesValueCheck(nameof(azPiConfig.AzureIOTConnectionString), PiConfigurations);
            GetMultiplePIConfigurationStatusesValueCheck(nameof(azPiConfig.ToastWebSendURL), PiConfigurations);
        }


       


        private void GetMultiplePIConfigurationStatusesValueCheck(string ValueToCheckAgainst, List<ViewModelRestNameValuePair> PiConfigurations)
        {
            Assert.AreEqual(PiConfigurations.Where(x => x.name.ToUpper() == ValueToCheckAgainst.ToUpper()).Any(), true, $"Could not find configuration value '{ValueToCheckAgainst}'");
        }
        #endregion



        #region  SetPIConfigurationStatus
        [TestMethod]
        public void ConfigurationTest_SetMultiplePIConfigurationStatuses()
        {
            var restRouteHandler = new RestRouteHandler();
            restRouteHandler.RegisterController<PiConfigurationController>();
            //Retrieve all the configuration values
            var piConfigurations = SetMultiplePIConfigurationStatuses_PiConfigurations_Retrieve(restRouteHandler);

            //Set all Allowed values to true and validate
            SetMultiplePIConfigurationStatuses_AllowedValues(false, piConfigurations);
            SetMultiplePIConfigurationStatuses_PiConfigurations_Transfer(restRouteHandler, piConfigurations);
            String piConfigValue = SetPIConfigurationStatus_AllowSendingofData_RetrieveData(restRouteHandler);
            Assert.AreEqual(piConfigValue.ToUpper(), "false".ToUpper(), "Configuration value AllowSendingofData should be false after test run");


            //Set all Allowed values to true and validate
            SetMultiplePIConfigurationStatuses_AllowedValues(true, piConfigurations);
            SetMultiplePIConfigurationStatuses_PiConfigurations_Transfer(restRouteHandler, piConfigurations);
            piConfigValue = SetPIConfigurationStatus_AllowSendingofData_RetrieveData(restRouteHandler);
            Assert.AreEqual(piConfigValue.ToUpper(), "true".ToUpper(), "Configuraiton value AllowSendingofData should be true after test run");

        }


       private  List<ViewModelRestNameValuePair> SetMultiplePIConfigurationStatuses_PiConfigurations_Retrieve(RestRouteHandler restRouteHandler)
        {
            //Retrieve pi configurations
            RestUpHttpServerRequest basicGet = HttpRequestsConfiguration.GetRequestPIConfigurationStatuses();
            var request = restRouteHandler.HandleRequest(basicGet);
            Assert.AreEqual(request.Result.ResponseStatus, HttpResponseStatus.OK, $"Reponse should be OK but was {request.Result.ResponseStatus.ToString()} ");
            
            //Deserialize the configuration objects
            var val = request.Result.Content.ToString();
            val = System.Text.Encoding.UTF8.GetString(request.Result.Content);
            var piConfigurations = JsonConvert.DeserializeObject<List<ViewModelRestNameValuePair>>(val);

            return piConfigurations;
        }


        private void SetMultiplePIConfigurationStatuses_PiConfigurations_Transfer(RestRouteHandler restRouteHandler, 
                                                                            List<ViewModelRestNameValuePair> piConfigurations)
        {
            RestUpHttpServerRequest postRequest = HttpRequestsConfiguration.PostReqestPiConfigurationStatuses(piConfigurations);
            var request = restRouteHandler.HandleRequest(postRequest);
        }


        /// <summary>
        /// Sets all the properties with the name allowed values in the PiConfiguration list
        /// </summary>
        /// <param name="value"></param>
        /// <param name="PiConfigurations"></param>
        private void SetMultiplePIConfigurationStatuses_AllowedValues( bool value, List<ViewModelRestNameValuePair> PiConfigurations)
        {           
            var props = typeof(AzurePiConfiguration).GetProperties().Where(x => x.Name.StartsWith("Allow"));
            foreach (var prop in props)
                SetMultiplePIConfigurationStatuses_AllowedValue(prop.Name, value, PiConfigurations);
        }


        private void SetMultiplePIConfigurationStatuses_AllowedValue(string Name, bool value, List<ViewModelRestNameValuePair> PiConfigurations)
        {
          var piConfigurationValue = PiConfigurations.Where(x => x.name.ToUpper() == Name.ToUpper()).FirstOrDefault();
           piConfigurationValue.value = value.ToString().ToLower();
        }


       
        #endregion



        #region  SetGetPIConfigurationStatus_AllowSendingofData
        /// <summary>
        /// Tests the Getting and setting of the PI Configuration 
        /// Status code.
        /// </summary>
        [TestMethod]
        public void ConfigurationTest_SetAndGetPIConfigurationStatus_AllowSendingofData()
        {
            //Create the Rest Rout handler to process the request
            var restRouteHandler = new RestRouteHandler();
            restRouteHandler.RegisterController<PiConfigurationController>();

            //Send the Request to the route Handler as False
            RestUpHttpServerRequest basicPut = HttpRequestsConfiguration.PostReqestPiConfigurationStatus("AllowSendingofData", "\"false\"");            
            var request = restRouteHandler.HandleRequest(basicPut);
            Assert.AreEqual(request.Result.ResponseStatus, HttpResponseStatus.OK, "Failed to Set  AllowSendingofData to false");
            
            //Send the Request to the route as True
            basicPut = HttpRequestsConfiguration.PostReqestPiConfigurationStatus("AllowSendingofData", "\"true\"");
            request = restRouteHandler.HandleRequest(basicPut);           
            Assert.AreEqual(request.Result.ResponseStatus, HttpResponseStatus.OK, "Failed to Set AllowSendingofData to true");
                    
            String piConfigValue = SetPIConfigurationStatus_AllowSendingofData_RetrieveData(restRouteHandler);
            Assert.AreEqual(piConfigValue.ToUpper(), "true".ToUpper(), "AllowSendingofData should be true after test run");
        }     

       

        /// <summary>
        ///  Retrieve the allow send data values from the database through the web server
        /// </summary>
        /// <param name="restRouteHandler"></param>
        /// <returns></returns>
        string SetPIConfigurationStatus_AllowSendingofData_RetrieveData(RestRouteHandler restRouteHandler)
        {
            RestUpHttpServerRequest basicGet = HttpRequestsConfiguration.GetRequestPIConfigurationStatus("AllowSendingofData"); 
            var  request = restRouteHandler.HandleRequest(basicGet);

            //Deserialize the returned Values
            var val = request.Result.Content.ToString();
            val = System.Text.Encoding.UTF8.GetString(request.Result.Content);
            String piConfigValue = (String)JsonConvert.DeserializeObject(val, typeof(String));

            return piConfigValue;
        }
        #endregion
    }
}
