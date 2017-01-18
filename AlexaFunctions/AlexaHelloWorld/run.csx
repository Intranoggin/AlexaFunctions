using System.Net;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info($"C# HTTP trigger function processed a request. RequestUri={req.RequestUri}");
    // parse query parameter
    string name = req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "name", true) == 0)
        .Value;

    // Get request body
    dynamic data = await req.Content.ReadAsAsync<object>();

    // Set name to query string or body data
    name = name ?? data?.name;

    if (name == null)
        return req.CreateResponse(HttpStatusCode.OK, new
        {
            version = "1.0",
            sessionAttributes = new { },
            response = new
            {
                outputSpeech = new
                {
                    type = "PlainText",
                    text = "What up, dude? Do you know how much Dad Rocks?"
                },
                card = new
                {
                    type = "Simple",
                    title = "HelloWorldIntent",
                    content = "Dad Rocks!"
                },
                shouldEndSession = true
            }
        });
    else
        return req.CreateResponse(HttpStatusCode.OK, new
        {
            version = "1.0",
            sessionAttributes = new { },
            response = new
            {
                outputSpeech = new
                {
                    type = "PlainText",
                    text = "What up, " + name + "? Do you know how much Dad Rocks?"
                },
                card = new
                {
                    type = "Simple",
                    title = "HelloWorldIntent",
                    content = "Dad Rocks!"
                },
                shouldEndSession = true
            }
        });
}