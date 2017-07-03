using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlexaFunctions.Utilities
{
    public class RequestLogEntry
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }

        #region Populated by Stage 1
        public string HTTPQueueObject { get; set; }
        public DateTime QueueInsertion { get; set; }
        public string RequestUrl { get; set; }
        public bool ResponseIsSuccessStatusCode { get; set; }
        public string ResponseReasonPhrase { get; set; }
        public string ResponseStatusCode { get; set; }
        #endregion

        public DateTime? RequestDate { get; set; }
        public string RequestType { get; set; }
        public string Question { get; set; }
        public string SlotName { get; set; }
        public string SlotValue { get; set; }
        public string ResponseText { get; set; }
    }
}