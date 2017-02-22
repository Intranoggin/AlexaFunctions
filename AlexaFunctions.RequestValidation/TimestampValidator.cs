using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AlexaFunctions.RequestValidation
{
    public class TimestampValidator
    {
        /// <summary>
        /// Verifies request timestamp
        /// </summary>
        public static bool VerifyRequestTimestamp(DateTime timeStamp, DateTime requestReceived)
        {
            try
            {
                // verify timestamp is within tolerance
                var diff = requestReceived - timeStamp;
                return (Math.Abs((decimal)diff.TotalSeconds) <= ControlConstants.TIMESTAMP_TOLERANCE_SEC);
            }
            catch
            {
                return false;
            }

        }
    }
}
