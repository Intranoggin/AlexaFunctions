using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using AlexaSkillsKit;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace AlexaFunctions
{
    public class AskTeenager
    {
        public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log, IAsyncCollector<string> alexaAskTeenagerRequestQueue)
        {
            log.Info($"Request={req}");

            var queryParams = req.GetQueryNameValuePairs();
            string teenagerType = queryParams
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

            string code = queryParams
               .FirstOrDefault(q => string.Compare(q.Key, "Bypasscode", true) == 0)
               .Value;
            
            if (!string.IsNullOrEmpty(code) && code == System.Environment.GetEnvironmentVariable("BypassCode"))
                speechlet.SkipValidation = true;

            var getResponse = await speechlet.GetResponseAsync(req);

            //JsonSerializerSettings serializationSettings = new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            //var logObject = new LogObject(JsonConvert.SerializeObject(req, serializationSettings), JsonConvert.SerializeObject(getResponse, serializationSettings));

            //var queueObject = JsonConvert.SerializeObject(logObject, serializationSettings);
            //await alexaAskTeenagerRequestQueue.AddAsync(queueObject);
            return getResponse;
        }
    }
    public class LogObject
    {
        public string Request { get; set; }
        public string Response { get; set; }
        public LogObject(string request, string response)
        {
            Request = request;
            Response = response;
        }
    }
}