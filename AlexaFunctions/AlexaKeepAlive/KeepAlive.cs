using System;
using System.Net;
using System.Text;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace AlexaFunctions
{
    public class KeepAlive
    {
        public static async void Run(TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"Keep Alive Timer: {DateTime.Now}");
            //System.IO.Stream stream = null;
            //WebClient wc = new WebClient();

            ////log.Info($"AlexaAskTeenageDaughterURL: {System.Environment.GetEnvironmentVariable("AlexaAskTeenagerURL")}");
            //string url = System.Environment.GetEnvironmentVariable("AlexaAskTeenagerURL") + "&type=daughter&BypassCode=" + System.Environment.GetEnvironmentVariable("BypassCode");            
            //stream = await wc.OpenReadTaskAsync(url);
            //stream.Dispose();

            string url = System.Environment.GetEnvironmentVariable("AlexaAskTeenagerURL") + "&type=daughter&BypassCode=" + System.Environment.GetEnvironmentVariable("BypassCode");

            log.Info($"URL: {url}");
            var http = (HttpWebRequest)WebRequest.Create(new Uri(url));
            http.Accept = "application/json";
            http.ContentType = "application/json";
            http.Method = "POST";

            string postBody = System.Environment.GetEnvironmentVariable("KeepAlivePostBody");
            ASCIIEncoding encoding = new ASCIIEncoding();
            Byte[] bytes = encoding.GetBytes(postBody);

            var newStream = http.GetRequestStream();
            newStream.Write(bytes, 0, bytes.Length);
            newStream.Close();

            var response = http.GetResponse();

            log.Info($"Response: {response.ToString()}");
        }
    }
}