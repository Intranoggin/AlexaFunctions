using System.Net;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    //log.Info($"Request={req}");

    // Get request body
    dynamic data = await req.Content.ReadAsAsync<object>();

    if (data == null)
    {
        log.Info($"Request={req}");
        return null;
    }
    log.Info($"Content={data}");
    // Set name to query string or body data
    string intentName = data.request.intent.name;
    log.Info($"intentName={intentName}");
    string outputText = "Growl";

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
            string subject = data.request.intent.slots.Subject.value;
            outputText = $"{subject} sucks";


            if (subject=="mom" || subject=="dad" || subject == "mother" || subject == "father")
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
            string activity = data.request.intent.slots.Activity.value;
            outputText = $"{activity} sucks";


            if (subactivityject == "mom" || activity == "dad" || activity == "mother" || activity == "father")
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
