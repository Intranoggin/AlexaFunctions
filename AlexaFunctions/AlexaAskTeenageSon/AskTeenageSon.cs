using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace AlexaFunctions
{
    public class AskTeenageSon
    {
        public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log, IAsyncCollector<string> alexaAskTeenageRequestQueue)
        {
            log.Info($"Request={req}");
            AskTeenageSonSpeechlet speechlet = new AskTeenageSonSpeechlet(log, alexaAskTeenageRequestQueue);

            return await speechlet.GetResponseAsync(req);
        }

    }
}