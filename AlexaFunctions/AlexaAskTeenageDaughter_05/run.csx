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
        case "AskTeenageDaughterStatus":
            return req.CreateResponse(HttpStatusCode.OK, new
            {
                version = "1.0",
                sessionAttributes = new { },
                response = new
                {
                    outputSpeech = new
                    {
                        type = "PlainText",
                        text = "Growl"
                    },
                    card = new
                    {
                        type = "Simple",
                        title = "Teenage Daughter Says",
                        content = "Growl"
                    },
                    shouldEndSession = true
                }
            });
            break;
        case "AskTeenageDaughterOpinion":
            string subject = data.request.intent.slots.Subject.value;
            return req.CreateResponse(HttpStatusCode.OK, new
            {
                version = "1.0",
                sessionAttributes = new { },
                response = new
                {
                    outputSpeech = new
                    {
                        type = "PlainText",
                        text = $"{subject} sucks"
                    },
                    card = new
                    {
                        type = "Simple",
                        title = "Teenage Daughter Says",
                        content = $"{subject} sucks"
                    },
                    shouldEndSession = true
                }
            });
            break;
        case "AskTeenageDaughterParticipation":
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
                        title = "Teenage Daughter Says",
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
