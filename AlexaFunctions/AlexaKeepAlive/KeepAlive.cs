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

            //log.Info($"AlexaAskTeenageDaughterURL: {System.Environment.GetEnvironmentVariable("AlexaAskTeenageDaughterURL")}");
            stream = await wc.OpenReadTaskAsync(System.Environment.GetEnvironmentVariable("AlexaAskTeenageDaughterURL"));
            stream.Dispose();
            //log.Info($"AlexaAskTeenageSonURL: {System.Environment.GetEnvironmentVariable("AlexaAskTeenageSonURL")}");
            stream = await wc.OpenReadTaskAsync(System.Environment.GetEnvironmentVariable("AlexaAskTeenageSonURL"));
            stream.Dispose();
        }
    }
}