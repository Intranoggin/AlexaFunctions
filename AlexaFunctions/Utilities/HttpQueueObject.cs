using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;

namespace AlexaFunctions.Utilities
{
    public class HttpQueueObject
    {
#region Member Data
        protected JsonSerializerSettings _SERIALIZATIONSETTINGS;
#endregion

#region Properties
        public dynamic RequestHeaders { get; set; }
        public System.Net.Http.HttpMethod RequestMethod { get; set; }
        public Uri RequestUri { get; set; }
        public dynamic RequestContent { get; set; }
        public bool ResponseIsSuccessStatusCode { get; set; }
        public string ResponseReasonPhrase { get; set; }
        public HttpStatusCode ResponseStatusCode { get; set; }
        public string ResponseContent { get; set; }
        public JsonSerializerSettings SerializationSettings
        {
            get
            {
                return _SERIALIZATIONSETTINGS;
            }
        }
#endregion

#region Constructors
        public HttpQueueObject()
        {
            _SERIALIZATIONSETTINGS = new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
        }
        public HttpQueueObject(HttpRequestMessage request,HttpResponseMessage response)
        {
            _SERIALIZATIONSETTINGS = new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            SetRequest(request);
            SetResponse(response);
        }
#endregion

#region Private Methods
        private void SetRequest(HttpRequestMessage request)
        {
            RequestHeaders = request.Headers;
            RequestMethod = request.Method;
            RequestUri = request.RequestUri;
            RequestContent = request.Content.ReadAsAsync<object>().Result;
        }
        private void SetResponse(HttpResponseMessage response)
        {
            ResponseIsSuccessStatusCode = response.IsSuccessStatusCode;
            ResponseReasonPhrase = response.ReasonPhrase;
            ResponseStatusCode = response.StatusCode;
            ResponseContent = response.Content.ReadAsStringAsync().Result;
        }
#endregion
    }
}