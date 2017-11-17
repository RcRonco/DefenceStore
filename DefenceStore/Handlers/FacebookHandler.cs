using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Facebook;
using System.Dynamic;
using System.IO;
using System.Text;
using System.Collections.Specialized;
using System.Net;
using System.Web.Mvc;
using DefenceStore.Models;
using Newtonsoft.Json;

namespace DefenceStore.Handlers
{
    public class FacebookHandler
    {
        private const string FacebookApiID = "127515004591775";
        private const string FacebookApiSecret = "837b99f237fd3fd17e99fec9161a8985";
        private const string PageID = "358735974554546";
        private const string fb_exchange_token = "EAABzZBWaNXp8BALOIiAwBN1VfB9mBLNUiw0qfsEwZBqDCUEZALCnDnstXmF4Db9AVhP5f8NMOtNYKDDNQV7QV7S5U5oqZCiMnCRdAnY50PJSQZBb6a5Ocut6Jt9GPviXW4a8fP0nDMunftU891I03MDjZBQC9CeZA8s0KXpkIFGON62OhJwuB5u";
        //private const string fb_exchange_token = "EAABzZBWaNXp8BAPKSdP5AxZCZAwL4bzIpDp9HRfAHqmHmWt7zOgd75bd7EF6avGO7ArNIZBpOI26UzR9DBVbwV9LpBGk6yhr4kQ9Q6ryLdnSKRQ9ZAxjr4QU2bn5QLsZBCeRVwfeCRt2ktrZATFtHrfPqYnHuABs41ZAmyshUVvNCgE0QnwhULVvMaScoroZBtsgZD";

        private const string AuthenticationUrlFormat =
            "https://graph.facebook.com/oauth/access_token?grant_type=fb_exchange_token&client_id={0}&client_secret={1}&fb_exchange_token={2}";

        /*
        private const string AuthenticationUrlFormat ="https://graph.facebook.com/oauth/{0}?fields=access_token";
        private const string AuthenticationUrlFormat =
        https://graph.facebook.com/oauth/access_token?client_id={0}&client_secret={1}&grant_type=client_credentials&scope=manage_pages,offline_access,publish_stream;
        https://graph.facebook.com/oauth/access_token?grant_type=fb_exchange_token&client_id={app-id}&client_secret={app-secret}&fb_exchange_token={step-3-token}
        */

        static string GetAccessToken(string apiID, string apiSecret, string pageID)
        {
            string accessToken = string.Empty;
            //string url = string.Format(AuthenticationUrlFormat, apiID, apiSecret);
            string url = string.Format(AuthenticationUrlFormat, apiID, apiSecret, fb_exchange_token);

            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();

            using (System.IO.Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.Default);
                String responseString = reader.ReadToEnd();

                //NameValueCollection query = HttpUtility.ParseQueryString(responseString);
                //accessToken = query["access_token"];
                dynamic stuff = JsonConvert.DeserializeObject(responseString);

                accessToken = stuff["access_token"];
            }

            if (accessToken.Trim().Length == 0)
                throw new Exception("There is no Access Token");

            return accessToken;
        }

        public static void PostMessage(Product product)
        {
            try
            {
                //string accessToken = GetAccessToken(FacebookApiID, FacebookApiSecret, PageID);
                string accessToken = fb_exchange_token;
                FacebookClient facebookClient = new FacebookClient(accessToken);

                dynamic messagePost = new ExpandoObject();
                messagePost.access_token = accessToken;
                messagePost.message = "Check Out The New Product In Our Store: " + product.Name +
                                      " ( " + product.QuantityInStock + " left in stock) " + "\n Now Only In: " + product.Price + "₪";
                //if (product.Image != null)
                //{
                //    messagePost.picture = product.Image;
                //    messagePost.link = "localhost";
                //}
                //messagePost.name = "[SOME_NAME]";
                //messagePost.caption = "my caption"; 
                //messagePost.description = "my description";

                string url = string.Format("/{0}/feed", PageID);
                var result = facebookClient.Post(url, messagePost);
            }
            catch (FacebookOAuthException ex)
            {
                throw new HttpException(401, ex.Message);
            }
            catch (Exception ex)
            {
                throw new HttpException(400, ex.Message);

            }

        }

    }
}