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
using AlexaFunctions.Utilities;

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

            (var isHttpQueueObject, var httpQueueObject) = DeserializeQueueObject(queueItem);

            if (isHttpQueueObject)
            {
                var requestLogEntry = new RequestLogEntry();
                requestLogEntry.HTTPQueueObject = queueItem;
                requestLogEntry.PartitionKey = "Version1Stage1";
                requestLogEntry.RowKey = id;
                requestLogEntry.QueueInsertion = insertionTime.ToUniversalTime().UtcDateTime;
                requestLogEntry.RequestUrl = httpQueueObject.RequestUri.ToString();
                requestLogEntry.ResponseIsSuccessStatusCode = httpQueueObject.ResponseIsSuccessStatusCode;
                requestLogEntry.ResponseReasonPhrase = httpQueueObject.ResponseReasonPhrase;
                requestLogEntry.ResponseStatusCode = httpQueueObject.ResponseStatusCode.ToString();
                requestLog.Add(requestLogEntry);
            }
        }

        public static(bool isHttpQueueObject, HttpQueueObject queueItem)
            DeserializeQueueObject(string queueItem)
        {
            HttpQueueObject httpQueueObject = new HttpQueueObject();
            JsonConvert.PopulateObject(queueItem, httpQueueObject, httpQueueObject.SerializationSettings);
            
            //is not a httpQueueObject queue entry
            if (httpQueueObject == null)
                return (false, null);
            
            return (true, httpQueueObject);
            
        }
    }
}