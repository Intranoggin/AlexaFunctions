using System;
using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Threading.Tasks;

namespace AlexaFunctions
{
    public class AskTeenageSon
    {
        public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log, IAsyncCollector<string> alexaAskTeenageRequestQueue)
        {
            log.Info($"Request={req}");
            await alexaAskTeenageRequestQueue.AddAsync(Convert.ToString(req));

            // Get request body
            dynamic data = await req.Content.ReadAsAsync<object>();

            if (data == null)
            {
                return null;
            }
            await alexaAskTeenageRequestQueue.AddAsync(Convert.ToString(data));

            log.Info($"Content={data}");
            // Set name to query string or body data
            string intentName = data.request.intent.name;
            log.Info($"intentName={intentName}");

            string outputText = "You're being rediculous.";
            switch (intentName)
            {
                case "AskTeenageSonStatus":
                    return req.CreateResponse(HttpStatusCode.OK, new
                    {
                        version = "1.0",
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
                                title = "Teenage Son Says",
                                content = outputText
                            },
                            shouldEndSession = true
                        }
                    });
                case "AskTeenageSonOpinion":
                    string subject = data.request.intent.slots.Subject.value;
                    outputText = $"{subject} sucks";


                    if (subject == "mom" || subject == "dad" || subject == "mother" || subject == "father")
                        outputText = $"{subject} rules!";
                    return req.CreateResponse(HttpStatusCode.OK, new
                    {
                        version = "1.0",
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
                                title = "Teenage Son Says",
                                content = outputText
                            },
                            shouldEndSession = true
                        }
                    });
                case "AskTeenageSonParticipation":
                    string activity = data.request.intent.slots.Activity.value;
                    outputText = $"{activity} sucks";

                    if (activity == "mom" || activity == "dad" || activity == "mother" || activity == "father")
                        outputText = $"{activity} is the best!";

                    return req.CreateResponse(HttpStatusCode.OK, new
                    {
                        version = "1.0",
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
                                title = "Teenage Son Says",
                                content = outputText
                            },
                            shouldEndSession = true
                        }
                    });
                default:
                    return req.CreateResponse(HttpStatusCode.OK, new
                    {
                        version = "1.0",
                        sessionAttributes = new { },
                        response = new
                        {
                            outputSpeech = new
                            {
                                type = "PlainText",
                                text = "Say something like\nTell teenage son good morning.\nAsk teenage son if he wants to go to soccer.\n Ask teenage son what he thinks of movies"
                            },
                            card = new
                            {
                                type = "Simple",
                                title = "Teenage Son Says",
                                content = "Say something like\nTell teenage son good morning.\nAsk teenage son if he wants to go to soccer.\n Ask teenage son what he thinks of movies"
                            },
                            shouldEndSession = true
                        }
                    });
            }
        }

    }
}