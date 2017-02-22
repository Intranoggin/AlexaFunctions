using System;

namespace AlexaFunctions.RequestValidation
{
    public enum ValidationResult
    {
        OK = 0,
        NoSignatureHeader = 1,
        NoCertHeader = 2,
        InvalidSignature = 4,
        InvalidTimestamp = 8,
        InvalidJson = 16
    }
}
