using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace System.Net
{
    /// <summary>
    /// Allow to set username, roles or anything useful for faking a user
    /// </summary>
    public static class HttpClientExtensions
    {
        /// <summary>
        /// Define a Token without paraments
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static HttpClient SetFakeBearerToken(this HttpClient client)
        {
            client.SetFakeBearerToken(new
            {
                sub = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10)
            });
            return client;
        }

        /// <summary>
        /// Define a Token with a custom object
        /// </summary>
        /// <param name="client"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static HttpClient SetFakeBearerToken(this HttpClient client, object token)
        {
            client.SetToken("FakeBearer", JsonConvert.SerializeObject(token));

            return client;
        }

        /// <summary>
        /// Define a Token with just a Username
        /// </summary>
        /// <param name="client"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public static HttpClient SetFakeBearerToken(this HttpClient client, string username)
        {
            client.SetFakeBearerToken(new
            {
                sub = username
            });

            return client;
        }

        /// <summary>
        /// Define a Token with a Username and some roles
        /// </summary>
        /// <param name="client"></param>
        /// <param name="username"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public static HttpClient SetFakeBearerToken(this HttpClient client, string username, string[] roles)
        {

            client.SetFakeBearerToken(new
            {
                sub = username,
                role = roles
            });

            return client;
        }

        /// <summary>
        /// Define a Token with a Username and some roles and other claims
        /// </summary>
        /// <param name="client"></param>
        /// <param name="username"></param>
        /// <param name="roles"></param>
        /// <param name="claim"></param>
        /// <returns></returns>
        public static HttpClient SetFakeBearerToken(this HttpClient client, string username, string[] roles, dynamic claim)
        {
            claim.sub = username;
            claim.role = roles;

            client.SetFakeBearerToken((object) claim);

            return client;
        }

        /// <summary>
        /// Set raw token
        /// </summary>
        /// <param name="client"></param>
        /// <param name="scheme"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static HttpClient SetToken(this HttpClient client, string scheme, string token)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, token);

            return client;
        }
    }
}
