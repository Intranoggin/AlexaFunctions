using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using AlexaSkillsKit;


namespace AlexaFunctions
{
    public class AskTeenageDaughter
    {
        public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log, IAsyncCollector<string> alexaAskTeenageRequestQueue)
        {
            log.Info($"Request={req}");            
            var speechlet = new AskTeenageDaughterSpeechlet(log, alexaAskTeenageRequestQueue);
            var getResponse = await speechlet.GetResponseAsync(req);
            
            var queueObject = $"{{[Request:{{{HttpHelpers.ToLogString(req)}}},Response:{{{HttpHelpers.ToLogString(getResponse)}}}]}}";
            await alexaAskTeenageRequestQueue.AddAsync(queueObject);
            return getResponse;
        }
    }
}