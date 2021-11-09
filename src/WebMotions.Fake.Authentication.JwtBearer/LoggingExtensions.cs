using System;
using Microsoft.Extensions.Logging;

namespace WebMotions.Fake.Authentication.JwtBearer
{
    internal static partial class LoggingExtensions
    {
        [LoggerMessage(1, LogLevel.Information, "Failed to validate the token.", EventName = "TokenValidationFailed")]
        public static partial void TokenValidationFailed(this ILogger logger, Exception ex);

        [LoggerMessage(2, LogLevel.Information, "Successfully validated the token.", EventName = "TokenValidationSucceeded")]
        public static partial void TokenValidationSucceeded(this ILogger logger);

        [LoggerMessage(3, LogLevel.Error, "Exception occurred while processing message.", EventName = "ProcessingMessageFailed")]
        public static partial void ErrorProcessingMessage(this ILogger logger, Exception ex);
    }
}