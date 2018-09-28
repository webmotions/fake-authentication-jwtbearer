using System;
using System.Threading.Tasks;

namespace GST.Fake.Authentication.JwtBearer.Events
{
    /// <summary>
    /// Specifies events which the <see cref="FakeJwtBearerHandler"/> invokes to enable developer control over the authentication process.
    /// </summary>
    public class JwtBearerEvents
    {
        /// <summary>
        /// Invoked if exceptions are thrown during request processing. The exceptions will be re-thrown after this event unless suppressed.
        /// </summary>
        public Func<AuthenticationFailedContext, Task> OnAuthenticationFailed { get; set; } = context => Task.CompletedTask;

        /// <summary>
        /// Invoked when a protocol message is first received.
        /// </summary>
        public Func<MessageReceivedContext, Task> OnMessageReceived { get; set; } = context => Task.CompletedTask;

        /// <summary>
        /// Invoked after the security token has passed validation and a ClaimsIdentity has been generated.
        /// </summary>
        public Func<TokenValidatedContext, Task> OnTokenValidated { get; set; } = context => Task.CompletedTask;

        /// <summary>
        /// Invoked before a challenge is sent back to the caller.
        /// </summary>
        public Func<JwtBearerChallengeContext, Task> OnChallenge { get; set; } = context => Task.CompletedTask;

        /// <summary>
        /// Invoked when authentication failed
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual Task AuthenticationFailed(AuthenticationFailedContext context) => OnAuthenticationFailed(context);

        /// <summary>
        /// Invoked when message is received
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual Task MessageReceived(MessageReceivedContext context) => OnMessageReceived(context);

        /// <summary>
        /// Invoked for token validation
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual Task TokenValidated(TokenValidatedContext context) => OnTokenValidated(context);

        /// <summary>
        /// Invoked for challenge
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual Task Challenge(JwtBearerChallengeContext context) => OnChallenge(context);
    }
}