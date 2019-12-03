using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace QnAMakerMessagesDemo
{
    public class MessageSender
    {
        public static void SendMessage(string message, string fromId, string toId, IConfiguration config, string type)
        {
            const string MESSAGING_URL = @"https://api.nexmo.com/v0.1/messages";
            try
            {
                var jwt = TokenGenerator.GenerateToken(config);

                var requestObject = new MessageRequest()
                {
                    to = new MessageRequest.To()
                    {
                        type = type
                    },
                    from = new MessageRequest.From()
                    {
                        type = type
                    },
                    message = new MessageRequest.Message()
                    {
                        content = new MessageRequest.Message.Content()
                        {
                            type = "text",
                            text = message
                        }
                    }
                };
                if (type == "messenger")
                {
                    requestObject.message.messenger = new MessageRequest.Message.Messenger()
                    {
                        category = "RESPONSE"
                    };
                    requestObject.to.id = toId;
                    requestObject.from.id = fromId;
                }
                else
                {
                    requestObject.to.number = toId;
                    requestObject.from.number = fromId;
                }
                var requestPayload = JsonConvert.SerializeObject(requestObject, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore });
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(MESSAGING_URL);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Accept = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.PreAuthenticate = true;
                httpWebRequest.Headers.Add("Authorization", "Bearer " + jwt);
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(requestPayload);
                }
                using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        Console.WriteLine(result);
                        Console.WriteLine("Message Sent");
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }
    }
}
