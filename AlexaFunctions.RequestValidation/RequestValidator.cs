using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AlexaFunctions.RequestValidation
{
    public class RequestValidator
    {
        public static async Task<ValidationResult> ValidateRequest(HttpRequestHeaders requestHeaders, string requestContent, TraceWriter log)
        {
            JObject reqContentOb = JsonConvert.DeserializeObject<JObject>(requestContent, new JsonSerializerSettings() { MissingMemberHandling = MissingMemberHandling.Ignore });
            DateTime requestReceived = DateTime.UtcNow;
            
            string chainUrl = null;
            if (!requestHeaders.Contains(ControlConstants.SIGNATURE_CERT_URL_REQUEST_HEADER) ||
                String.IsNullOrEmpty(chainUrl = requestHeaders.GetValues(ControlConstants.SIGNATURE_CERT_URL_REQUEST_HEADER).First()))
            {
                return ValidationResult.NoCertHeader;
            }

            string signature = null;
            if (!requestHeaders.Contains(ControlConstants.SIGNATURE_REQUEST_HEADER) ||
                String.IsNullOrEmpty(signature = requestHeaders.GetValues(ControlConstants.SIGNATURE_REQUEST_HEADER).First()))
            {
                return ValidationResult.NoSignatureHeader;
            }

            // Get request body             
            //byte[] data = await httpRequest.Content..ReadAsByteArrayAsync();
            log.Info($"requestContent={requestContent.ToString()}");
            log.Info($"signature={signature}");
            log.Info($"chainUrl={chainUrl}");
            

            // attempt to verify timestamp only if we were able to parse request body
            if (requestContent == null)
            {
                return ValidationResult.InvalidJson;
            }
            //byte[] requestContentBytes = System.Text.Encoding.UTF8.GetBytes(requestContent.ToString());
            if (!SignatureValidator.VerifyRequestSignature(requestContent, signature, chainUrl, log))
            {
                return ValidationResult.InvalidSignature;
            }

            //DateTime timeStamp = requestContent.Value<DateTime>("timestamp");
            //if (!TimestampValidator.VerifyRequestTimestamp(timeStamp, requestReceived))
            //{
            //    log.Info("line E-1");
            //    return ValidationResult.InvalidTimestamp;
            //}

            return ValidationResult.OK;
        }
    }
}
