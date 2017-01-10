using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GST.Fake.Authentication.JwtBearer
{
    public static class HttpClientExtensions
    {

        /// <summary>
        /// Define a Token with a custom object
        /// </summary>
        /// <param name="client"></param>
        /// <param name="token"></param>
        public static void SetFakeBearerToken(this HttpClient client, object token)
        {
            client.SetToken("FakeBearer", JsonConvert.SerializeObject(token));
        }

        /// <summary>
        /// Define a Token with juste a Username
        /// </summary>
        /// <param name="client"></param>
        /// <param name="username"></param>
        public static void SetFakeBearerToken(this HttpClient client, string username)
        {
            client.SetFakeBearerToken(new
            {
                sub = username
            });
        }

        /// <summary>
        /// Define a Token with a Username and some roles
        /// </summary>
        /// <param name="client"></param>
        /// <param name="username"></param>
        /// <param name="roles"></param>
        public static void SetFakeBearerToken(this HttpClient client, string username, string[] roles)
        {

            client.SetFakeBearerToken(new
            {
                sub = username,
                role = roles
            });
        }


        public static void SetToken(this HttpClient client, string scheme, string token)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, token);
        }
    }
}
