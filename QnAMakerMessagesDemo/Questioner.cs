using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace QnAMakerMessagesDemo
{
    public class Questioner
    {
        //TODO: fill in with Knowledge base ID
        const string kb_id = "YOUR_KNOWLEDGE_BASE_ID";

        //TODO: fill in with Knowledge base Endpoint key
        const string ENDPOINT_KEY = "YOUR_KNOWLEDGE_BASE_ENDPOINT_KEY";

        const string QUESTION_FORMAT = @"{{'question': '{0}'}}";

        //TODO fill in base url
        static readonly string URI = $"https://AZURE_APP_NAME.azurewebsites.net/qnamaker/knowledgebases/{kb_id}/generateAnswer";

        public static async Task AskQuestion(string to, string from, string type, string question, IConfiguration config)
        {
            try
            {
                question = HttpUtility.JavaScriptStringEncode(question);
                var response = await RequestAnswer(question);
                MessageSender.SendMessage(response, from, to, config, type);
            }
            catch (Exception e)
            {

                Debug.WriteLine(e.ToString());
            }  
        }

        public static async Task<string> RequestAnswer(string question)
        {
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(URI);
                var formatted_question = string.Format(QUESTION_FORMAT, question);
                request.Content = new StringContent(formatted_question, Encoding.UTF8, "application/json");
                request.Headers.Add("Authorization", "EndpointKey " + ENDPOINT_KEY);
                var response = await client.SendAsync(request);
                var jsonResponse = await response.Content.ReadAsStringAsync();
                JObject obj = JObject.Parse(jsonResponse);
                var answer = ((JArray)obj["answers"])[0]["answer"];
                return answer.ToString();
            }
        }
    }
}
