# AlexaKeepAlive

This is a simple timer trigger function that calls the http functions every 15 minutes. 

## How it works
It requires two environmental variables to exist,  AlexaAskTeenageDaughterURL and AlexaAskTeenageSonURL. It reads these variables in and makes a webClient request to them. The schedule is set to every 15 minutes because that is the point at which Azure Functions are removed from memory if they are dormant.

## Learn more

[Alexa on Azure: Part 7–Keep it Alive](http://http://intranoggin.com/Blog/March-2017/Alexa-on-Azure-Part-7–Keep-it-Alive.aspx "Alexa on Azure: Part 7–Keep it Alive")