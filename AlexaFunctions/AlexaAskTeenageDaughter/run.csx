#r "..\bin\AlexaFunctions.RequestValidation.dll"
#r "Newtonsoft.Json"

using System.Net;
using System.Net.Http;
using AlexaFunctions.RequestValidation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log, IAsyncCollector<string> alexaAskTeenageRequestQueue)
{
    log.Info("line 8-");
    log.Info($"Request={req}");


    log.Info("line 11");
    string requestContent = await req.Content.ReadAsStringAsync();//ReadAsAsync<JObject>(); 

    //JObject reqContentOb = JsonConvert.DeserializeObject<JObject>(requestContent, new JsonSerializerSettings() { MissingMemberHandling = MissingMemberHandling.Ignore });
    JObject reqContentOb = JsonConvert.DeserializeObject<JObject>(requestContent);


    Task <ValidationResult> getValidationResult = AlexaFunctions.RequestValidation.RequestValidator.ValidateRequest(req.Headers, requestContent, log);
    log.Info("line 13");
    //no need to await these because nothing depends on the return value.
    alexaAskTeenageRequestQueue.AddAsync(Convert.ToString(req));
    log.Info("line 16");

    ValidationResult validationResult = await getValidationResult;
    log.Info("line 19");
    if (validationResult != ValidationResult.OK)
    {
        log.Info("line 22");
        log.Info($"validationResult={validationResult.ToString()}");
        log.Info("line 24");
        return new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            ReasonPhrase = validationResult.ToString()
        };
    }
    log.Info("line 30");
    //// Get request body
    ////dynamic data = await getDataTask;
    ////Task<dynamic> getDataTask = req.Content.ReadAsAsync<object>();

    ////dynamic data = await getDataTask;


    log.Info($"requestContent={requestContent.ToString()}");
    log.Info("line 34");
    alexaAskTeenageRequestQueue.AddAsync(requestContent.ToString());
    log.Info("line 36");
    log.Info($"reqContentOb={reqContentOb.ToString()}");
    log.Info("line 37");
    // Set name to query string or body data
    string intentName = (string)reqContentOb["request"]["intent"]["name"];// requestContent.Value<string>("intent");
    log.Info($"intentName={intentName}");
    string outputText = "Growl";
    
    switch (intentName)
    {
        case "AskTeenageDaughterStatus":
            log.Info("line 38");
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
                        title = "Teenage Daughter Says",
                        content = outputText
                    },
                    shouldEndSession = true
                }
            });
            break;
        case "AskTeenageDaughterOpinion":
            string subject = (string)reqContentOb["request"]["intent"]["slots"]["Subject"]["value"];
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
                        title = "Teenage Daughter Says",
                        content = outputText
                    },
                    shouldEndSession = true
                }
            });
            break;
        case "AskTeenageDaughterParticipation":
            string activity = (string)reqContentOb["request"]["intent"]["slots"]["Activity"]["value"];
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
                        title = "Teenage Daughter Says",
                        content = outputText
                    },
                    shouldEndSession = true
                }
            });
            break;
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
            break;
    }
}
