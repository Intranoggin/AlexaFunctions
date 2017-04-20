# Alexa Ask Teenager

This is the current verison of the Alexa Skill, Ask Teenager. 

The skill is written in C# and hosted on Azure Functions. 

There are two gender specific skills (Teenage Daughter and Teenage Son) registered with Amazon. Both of those skills ultimate point to this code. I use Azure Function Proxies to create two unique urls that I register with Amazon, and when calls come in over those urls. In some cases I need to pass back a gender specific response (such as in help), for this I use the proxes to push some additoinal query string parameters to this function to determine which skill was the caller.


The creation of this skill is described on [Intranoggin.com](http://intranoggin.com "Blog") over several articles released beginning in February, 2017.



### Update 4/20/2017
Going forward, I'm combining the functionality from both the Alexa Ask Teenage Son and Alexa Ask Teenage Daughter skills into a single AlexaAskTeenager skill.

