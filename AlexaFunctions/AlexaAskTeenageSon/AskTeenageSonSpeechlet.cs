using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using AlexaSkillsKit.Speechlet;
using AlexaSkillsKit.UI;
using AlexaSkillsKit.Slu;
using AlexaSkillsKit.Authentication;
using AlexaSkillsKit.Json;
using System.Collections.Generic;

namespace AlexaFunctions
{
    public class AskTeenageSonSpeechlet : SpeechletAsync
    {
        #region Constants and Private Members
        const string PROTECTEDWORDS = "mom, mother, mommy, dad, father, daddy";
        #endregion

        #region Properties
        public TraceWriter Logger { get; set; }
        public IAsyncCollector<string> AskTeenageQueue { get; set; }
        #endregion

        #region Constructor
        public AskTeenageSonSpeechlet(TraceWriter log, IAsyncCollector<string> alexaAskTeenageRequestQueue)
        {
            Logger = log;
            AskTeenageQueue = alexaAskTeenageRequestQueue;
        }
        #endregion

        #region Public Overrides
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
            var tasks = new List<Task>();
            tasks.Add(AskTeenageQueue.AddAsync(JsonConvert.SerializeObject(request)));
            tasks.Add(AskTeenageQueue.AddAsync(JsonConvert.SerializeObject(session)));

            // Note: If the session is started with an intent, no welcome message will be rendered;
            // rather, the intent specific response will be returned.

            switch (intentName)
            {
                case "AMAZON.CancelIntent":
                    return await BuildAskTeenageSonExitResponseAsync(intent, session);
                case "AMAZON.StopIntent":
                    return await BuildAskTeenageSonExitResponseAsync(intent, session);
                case "AMAZON.HelpIntent":
                    return await BuildAskTeenageSonHelpResponseAsync(intent, session);
                case "AskTeenageSonOpinion":
                    return await BuildAskTeenageSonOpinionResponseAsync(intent, session);
                case "AskTeenageSonParticipation":
                    return await BuildAskTeenageSonParticipationResponseAsync(intent, session);
                case "AskTeenageSonStatus":
                    return await BuildAskTeenageSonStatusResponseAsync(intent, session);
                default:
                    throw new SpeechletException("Invalid Intent");
            }
        }
        public override bool OnRequestValidation(SpeechletRequestValidationResult result, DateTime referenceTimeUtc, SpeechletRequestEnvelope requestEnvelope)
        {
            return base.OnRequestValidation(result, referenceTimeUtc, requestEnvelope);
        }
        #endregion

        #region Private Methods
        private async Task<SpeechletResponse> GetWelcomeResponseAsync()
        {
            // Create the welcome message.
            string speechOutput =
                "Say something like\nGood morning.\nDo you want to go to soccer?\n What do you think of baseball practice?";

            // Here we are setting shouldEndSession to false to not end the session and
            // prompt the user for input
            return await BuildSpeechletResponseAsync("Welcome", speechOutput, false);
        }
        private async Task<SpeechletResponse> BuildSpeechletResponseAsync(string title, string output, bool shouldEndSession)
        {
            // Create the Simple card content.
            SimpleCard card = new SimpleCard();
            card.Title = title;
            card.Content = output;

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

        private async Task<SpeechletResponse> BuildAskTeenageSonOpinionResponseAsync(Intent intent, Session session)
        {
            if (string.IsNullOrEmpty(intent.Slots["Subject"].Value))
                return await BuildSpeechletResponseAsync(intent.Name, intent.Slots["Question"].Value, false);
            else
            {
                string subject = intent.Slots["Subject"].Value;
                string speechOutput = (PROTECTEDWORDS.Contains(subject)) ?
                    $"{subject} rules." :
                    $"{subject} sucks.";
                return await BuildSpeechletResponseAsync(intent.Name, speechOutput, false);
            }

        }
        private async Task<SpeechletResponse> BuildAskTeenageSonParticipationResponseAsync(Intent intent, Session session)
        {
            string activity = intent.Slots["Activity"].Value;
            string speechOutput = (PROTECTEDWORDS.Contains(activity)) ?
                $"{activity} rules." :
                $"{activity} sucks.";
            return await BuildSpeechletResponseAsync(intent.Name, speechOutput, false);
        }
        private async Task<SpeechletResponse> BuildAskTeenageSonStatusResponseAsync(Intent intent, Session session)
        {
            string speechOutput = "You're being rediculous.";
            return await BuildSpeechletResponseAsync(intent.Name, speechOutput, false);
        }
        private async Task<SpeechletResponse> BuildAskTeenageSonExitResponseAsync(Intent intent, Session session)
        {
            string speechOutput = "Whatever";
            return await BuildSpeechletResponseAsync(intent.Name, speechOutput, true);
        }
        private async Task<SpeechletResponse> BuildAskTeenageSonHelpResponseAsync(Intent intent, Session session)
        {
            string speechOutput =
                "Talk to me like a teenager.\n Ask me about my day.\n Say something like\nGood Morning.\nDo you want to go to soccer?\n How is life?";
            return await BuildSpeechletResponseAsync(intent.Name, speechOutput, false);
        }
    }
    #endregion
}