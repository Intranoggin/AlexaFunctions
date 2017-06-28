using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Azure.WebJobs;

namespace AlexaFunctions
{
    public class ProcessRequestQueueLog
    {
        public static void Run(string queueItem,
    DateTimeOffset expirationTime,
    DateTimeOffset insertionTime,
    DateTimeOffset nextVisibleTime,
    string queueTrigger,
    string id,
    string popReceipt,
    int dequeueCount,
    ICollector<RequestLogEntry> requestLog,
    TraceWriter log)
        {
            log.Info($"[id:{id},insertionTime:{insertionTime},expirationTime:{expirationTime},nextVisibleTime:{nextVisibleTime},popReceipt:{popReceipt},dequeueCount:{dequeueCount},queueItem:{queueItem}");

            (var isRequestLogEntry, var requestLogEntry) = DeserializeQueueObject(queueItem);

            if (isRequestLogEntry)
            {

                requestLog.Add(requestLogEntry);
                    }
        }

        public static(bool isRequestLogEntry, RequestLogEntry requestLogEntry)
            DeserializeQueueObject(string queueItem)
        {
            JsonSerializerSettings deserializationSettings = new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Ignore };
            JObject json = JsonConvert.DeserializeObject<JObject>(queueItem, deserializationSettings);

            JObject request = json.Value<JObject>("Request");

            //is not a request/response queue entry
            if (request == null)
                return (false, null);

            JObject respose = json.Value<JObject>("Response");

            var requestLogEntry = new RequestLogEntry();

            return (false,requestLogEntry);
            
        }
    }
    public class RequestLogEntry
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public DateTime RequestDate { get; set; }
        public string RequestType { get; set; }
        public string Question { get; set; }
        public string SlotName { get; set; }
        public string SlotValue { get; set; }
        public string ResponseText { get; set; }

    }
}