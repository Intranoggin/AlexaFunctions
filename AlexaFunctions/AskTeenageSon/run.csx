using System.Net;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    //log.Info($"Request={req}");

    // Get request body
    dynamic data = await req.Content.ReadAsAsync<object>();

    log.Info($"Content={data}");
    // Set name to query string or body data
    string intentName = data.request.intent.name;
    log.Info($"intentName={intentName}");

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
                        text = "You're being rediculous."
                    },
                    card = new
                    {
                        type = "Simple",
                        title = "Teenage Son Says",
                        content = "You're being rediculous."
                    },
                    shouldEndSession = true
                }
            });
            break;
        case "AskTeenageSonOpinion":
            string subject = data.request.intent.slots.Subject.value;
            string outputText = $"{subject} sucks";


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
            break;
        case "AskTeenageSonParticipation":
            string activity = data.request.intent.slots.Activity.value;
            return req.CreateResponse(HttpStatusCode.OK, new
            {
                version = "1.0",
                sessionAttributes = new { },
                response = new
                {
                    outputSpeech = new
                    {
                        type = "PlainText",
                        text = $"{activity} sucks"
                    },
                    card = new
                    {
                        type = "Simple",
                        title = "Teenage Son Says",
                        content = $"{activity} sucks"
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
            break;
    }
}
