using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using AlexaSkillsKit;
using System.Linq;


namespace AlexaFunctions
{
    public class AskTeenager
    {
        public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log, IAsyncCollector<string> alexaAskTeenagerRequestQueue)
        {
            log.Info($"Request={req}");

            string teenagerType = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "type", true) == 0)
                .Value;

            string cardTitle = "Teenager Says";
            switch (teenagerType)
            {
                case "daughter":
                    cardTitle = "Teenage Daughter";
                    break;
                case "son":
                    cardTitle = "Teenage Son";
                    break;
            }

            var speechlet = new AskTeenagerSpeechlet(log, alexaAskTeenagerRequestQueue,cardTitle);
            var getResponse = await speechlet.GetResponseAsync(req);
            
            var queueObject = $"{{[Request:{{{HttpHelpers.ToLogString(req)}}},Response:{{{HttpHelpers.ToLogString(getResponse)}}}]}}";
            await alexaAskTeenagerRequestQueue.AddAsync(queueObject);
            return getResponse;
        }
    }
}