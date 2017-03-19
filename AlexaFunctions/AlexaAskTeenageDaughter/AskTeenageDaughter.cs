using System;
using System.Net;
using System.Net.Http;
using AlexaFunctions.RequestValidation;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AlexaFunctions
{
    public class AskTeenageDaughter
    {
        public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log, IAsyncCollector<string> alexaAskTeenageRequestQueue)
        {
            log.Info($"Request={req}");

            string requestContent = await req.Content.ReadAsStringAsync(); 

            JObject reqContentOb = JsonConvert.DeserializeObject<JObject>(requestContent);
            
            Task<ValidationResult> getValidationResult = AlexaFunctions.RequestValidation.RequestValidator.ValidateRequest(req.Headers, requestContent, log);
            //no need to await these because nothing depends on the return value.
            alexaAskTeenageRequestQueue.AddAsync(Convert.ToString(req));

            ValidationResult validationResult = await getValidationResult;
            if (validationResult != ValidationResult.OK)
            {
                log.Info($"validationResult={validationResult.ToString()}");
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    ReasonPhrase = validationResult.ToString()
                };
            }

            log.Info($"requestContent={requestContent.ToString()}");
            alexaAskTeenageRequestQueue.AddAsync(requestContent.ToString());
            log.Info($"reqContentOb={reqContentOb.ToString()}");


            string intentType = (string)reqContentOb["request"]["type"];
            if(intentType == "LaunchRequest")
            {
                return req.CreateResponse(HttpStatusCode.OK, new
                {
                    version = "1.1",
                    sessionAttributes = new { },
                    response = new
                    {
                        outputSpeech = new
                        {
                            type = "PlainText",
                            text = "Say something like\nTell teenage daughter good morning.\nAsk teenage daughter if she wants to go to soccer.\n Ask teenage daughter what she thinks of movies"
                        },
                        card = new
                        {
                            type = "Simple",
                            title = "Teenage Daughter Says",
                            content = "Say something like\nTell teenage daughter good morning.\nAsk teenage daughter if she wants to go to soccer.\n Ask teenage daughter what she thinks of movies"
                        },
                        shouldEndSession = true
                    }
                });

            }
            // Set name to query string or body data
            string intentName = (string)reqContentOb["request"]["intent"]["name"];
            log.Info($"intentName={intentName}");
            string outputText = "Growl";

            switch (intentName)
            {                
                case "AskTeenageDaughterOpinion":
                    string subject = (string)reqContentOb["request"]["intent"]["slots"]["Subject"]["value"];
                    outputText = $"{subject} sucks";


                    if (subject == "mom" || subject == "dad" || subject == "mother" || subject == "father")
                        outputText = $"{subject} rules!";
                    return req.CreateResponse(HttpStatusCode.OK, new
                    {
                        version = "1.1",
                        sessionAttributes = new { },
                        response = new
                        {
                            outputSpeech = new
                            {
                                type = "PlainText",
                                text = outputText
                            },
                            card = new
                            {
                                type = "Simple",
                                title = "Teenage Daughter Says",
                                content = outputText
                            },
                            shouldEndSession = true
                        }
                    });
                case "AskTeenageDaughterParticipation":
                    string activity = (string)reqContentOb["request"]["intent"]["slots"]["Activity"]["value"];
                    outputText = $"{activity} sucks";


                    if (activity == "mom" || activity == "dad" || activity == "mother" || activity == "father")
                        outputText = $"{activity} is the best!";
                    return req.CreateResponse(HttpStatusCode.OK, new
                    {
                        version = "1.1",
                        sessionAttributes = new { },
                        response = new
                        {
                            outputSpeech = new
                            {
                                type = "PlainText",
                                text = outputText
                            },
                            card = new
                            {
                                type = "Simple",
                                title = "Teenage Daughter Says",
                                content = outputText
                            },
                            shouldEndSession = true
                        }
                    });
                default: //should be case "AskTeenageDaughterStatus":
                    return req.CreateResponse(HttpStatusCode.OK, new
                    {
                        version = "1.1",
                        sessionAttributes = new { },
                        response = new
                        {
                            outputSpeech = new
                            {
                                type = "PlainText",
                                text = outputText
                            },
                            card = new
                            {
                                type = "Simple",
                                title = "Teenage Daughter Says",
                                content = outputText
                            },
                            shouldEndSession = true
                        }
                    });
            }
        }

    }
}