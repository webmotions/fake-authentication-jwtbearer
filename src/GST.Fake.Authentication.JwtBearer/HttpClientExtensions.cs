using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;

namespace GST.Fake.Authentication.JwtBearer
{
    public static class HttpClientExtensions
    {

        /// <summary>
        /// Define a Token with a custom object
        /// </summary>
        /// <param name="client"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static HttpClient SetFakeBearerToken(this HttpClient client, object token)
        {
            client.SetToken("Bearer", JsonConvert.SerializeObject(token));

            return client;
        }

        /// <summary>
        /// Define a Token with juste a Username
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
        /// Set Raw Tocken
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
