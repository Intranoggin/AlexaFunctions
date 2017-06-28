using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using AlexaSkillsKit.Speechlet;
using AlexaSkillsKit.UI;
using AlexaSkillsKit.Slu;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using AlexaSkillsKit.Authentication;
using AlexaSkillsKit.Json;

namespace AlexaFunctions
{
    public class AskTeenagerSpeechlet : SpeechletAsync
    {
        #region Constants and Private Members
        const string PROTECTEDWORDS = "mom, mother, mommy, dad, father, daddy";
        string[] SUBJECTSLOTS = { "Actor", "Artist", "Athlete", "Author", "Book", "Subject" };
        #endregion

        #region Properties
        public TraceWriter Logger { get; set; }
        public IAsyncCollector<string> AskTeenageQueue { get; set; }
        public string CardTitle { get; set; }
        public bool SkipValidation { get; set; }
        #endregion

        #region Constructor
        public AskTeenagerSpeechlet(TraceWriter log, IAsyncCollector<string> alexaAskTeenagerRequestQueue, string cardTitle)
        {
            Logger = log;
            AskTeenageQueue = alexaAskTeenagerRequestQueue;
            CardTitle = cardTitle;
            SkipValidation = false;
        }
        #endregion

        #region Public Overrides
        public override bool OnRequestValidation(SpeechletRequestValidationResult result, DateTime referenceTimeUtc, SpeechletRequestEnvelope requestEnvelope)
        {
            if (SkipValidation)
                return true;

            return base.OnRequestValidation(result, referenceTimeUtc, requestEnvelope);
        }

        public override async Task OnSessionStartedAsync(SessionStartedRequest request, Session session)
        {
            Task t = AskTeenageQueue.AddAsync(JsonConvert.SerializeObject(request));
            Logger.Info($"OnSessionStarted requestId={request.RequestId}, sessionId={session.SessionId}");
            await t;
        }

        public override async Task OnSessionEndedAsync(SessionEndedRequest request, Session session)
        {
            Task t = AskTeenageQueue.AddAsync(JsonConvert.SerializeObject(request));
            Logger.Info($"OnSessionStarted requestId={request.RequestId}, sessionId={session.SessionId}");
            await t;
        }

        public override async Task<SpeechletResponse> OnLaunchAsync(LaunchRequest request, Session session)
        {
            try
            {
                await AskTeenageQueue.AddAsync(JsonConvert.SerializeObject(request));
            }
            catch (Exception ex)
            {
                Logger.Error($"Exception: {ex.ToString()}");
            }
            Logger.Info($"OnSessionStarted requestId={request.RequestId}, sessionId={session.SessionId}");
            return await GetWelcomeResponseAsync();
        }

        public override async Task<SpeechletResponse> OnIntentAsync(IntentRequest request, Session session)
        {
            // Get intent from the request object.
            Intent intent = request.Intent;
            string intentName = (intent != null) ? intent.Name : null;

            Logger.Info($"OnIntent intentName={intentName} requestId={request.RequestId}, sessionId={session.SessionId}");
            //var tasks = new List<Task>();
            //tasks.Add(AskTeenageQueue.AddAsync(JsonConvert.SerializeObject(request)));
            //tasks.Add(AskTeenageQueue.AddAsync(JsonConvert.SerializeObject(session)));


            // Note: If the session is started with an intent, no welcome message will be rendered;
            // but, the intent specific response will be returned and the session terminated.

            switch (intentName)
            {
                case "AMAZON.CancelIntent":
                    return await BuildAskTeenagerExitResponseAsync(intent, session);
                case "AMAZON.StopIntent":
                    return await BuildAskTeenagerExitResponseAsync(intent, session);
                case "AMAZON.HelpIntent":
                    return await BuildAskTeenagerHelpResponseAsync(intent, session);
                case "AskTeenagerOpinion":
                    return await BuildAskTeenagerOpinionResponseAsync(intent, session);
                case "AskTeenagerStatus":
                    return await BuildAskTeenagerStatusResponseAsync(intent, session);
                default:
                    throw new SpeechletException("Invalid Intent");
            }
        }
        #endregion

        #region Private Methods
        private async Task<SpeechletResponse> GetWelcomeResponseAsync()
        {
            // Create the welcome message.
            string speechOutput =
                "Say something like\nGo to bed.\nAre you ready for school?\n What do you think of jogging?\n You can also ask for help.";

            // Here we are setting shouldEndSession to false to not end the session and
            // prompt the user for input
            return await BuildSpeechletResponseAsync(CardTitle, speechOutput, false);
        }
        private async Task<SpeechletResponse> BuildSpeechletResponseAsync(string title, string output, bool shouldEndSession)
        {
            // Create the Simple card content.
            SimpleCard card = new SimpleCard();
            card.Title = title;
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;            
            card.Content = textInfo.ToTitleCase(output);

            // Create the plain text output.
            PlainTextOutputSpeech speech = new PlainTextOutputSpeech();            
            speech.Text = output;

            // Create the speechlet response.
            SpeechletResponse response = new SpeechletResponse();
            response.ShouldEndSession = shouldEndSession;
            response.OutputSpeech = speech;
            response.Card = card;
            return response;
        }

        private async Task<SpeechletResponse> BuildAskTeenagerOpinionResponseAsync(Intent intent, Session session)
        {
            var subject = GetSubjectFromAnySlot(intent);
            if (string.IsNullOrEmpty(subject))
                return await BuildSpeechletResponseAsync(CardTitle, intent.Slots["Question"].Value, session.IsNew);
            else
            {
                string speechOutput = (PROTECTEDWORDS.Contains(subject)) ?
                    $"{subject} rules." :
                    $"{subject} sucks.";
                return await BuildSpeechletResponseAsync(CardTitle, speechOutput, session.IsNew);
            }
        }
        private string GetSubjectFromAnySlot(Intent intent)
        {
            foreach(var slotName in SUBJECTSLOTS)
            {
                if (!string.IsNullOrEmpty(intent.Slots[slotName].Value))
                    return intent.Slots[slotName].Value;

            }
            return string.Empty;
        }
        private async Task<SpeechletResponse> BuildAskTeenagerStatusResponseAsync(Intent intent, Session session)
        {
            string speechOutput = "Growl";
            return await BuildSpeechletResponseAsync(CardTitle, speechOutput, session.IsNew);
        }
        private async Task<SpeechletResponse> BuildAskTeenagerExitResponseAsync(Intent intent, Session session)
        {
            string speechOutput = "Whatever";
            return await BuildSpeechletResponseAsync(CardTitle, speechOutput, true);
        }
        private async Task<SpeechletResponse> BuildAskTeenagerHelpResponseAsync(Intent intent, Session session)
        {
            string speechOutput =
                "Talk to me like a teenager.\n Ask me about my day.\n Say something like\nGood Morning.\nDo you want to go to soccer?\n How is life?\n" +
                " Just like a real teenager, I'll continue to respond to additional questions until you tell me to stop, cancel, or ignore me for a while.";
            return await BuildSpeechletResponseAsync(CardTitle, speechOutput, false);
        }
    }
#endregion
}