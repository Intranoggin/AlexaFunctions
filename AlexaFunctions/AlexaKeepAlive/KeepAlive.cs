using System;
using System.Net;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace AlexaFunctions
{
    public class KeepAlive
    {
        public static async void Run(TimerInfo myTimer, TraceWriter log)
        {
            //CI Test 2
            log.Info($"Keep Alive Timer: {DateTime.Now}");
            System.IO.Stream stream = null;
            WebClient wc = new WebClient();

            //log.Info($"AlexaAskTeenageDaughterURL: {System.Environment.GetEnvironmentVariable("AlexaAskTeenagerURL")}");
            string url = System.Environment.GetEnvironmentVariable("AlexaAskTeenagerURL") + "&type=daughter&BypassCode=" + System.Environment.GetEnvironmentVariable("BypassCode");            
            stream = await wc.OpenReadTaskAsync(url);
            stream.Dispose();
        }
    }
}