using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using AlexaSkillsKit;


namespace AlexaFunctions
{
    public class AskTeenager
    {
        public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log, IAsyncCollector<string> alexaAskTeenagerRequestQueue)
        {
            log.Info($"Request={req}");            
            var speechlet = new AskTeenagerSpeechlet(log, alexaAskTeenagerRequestQueue);
            var getResponse = await speechlet.GetResponseAsync(req);
            
            var queueObject = $"{{[Request:{{{HttpHelpers.ToLogString(req)}}},Response:{{{HttpHelpers.ToLogString(getResponse)}}}]}}";
            await alexaAskTeenagerRequestQueue.AddAsync(queueObject);
            return getResponse;
        }
    }
}