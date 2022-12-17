// See https://aka.ms/new-console-template for more information

using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Web;
using Newtonsoft.Json.Linq;

namespace Text.Translate;

public class Program
{
    private static string key = "";
    private static string baseUrl = "";
    
    static async Task Main(string[] args)
    {
        var userText = "";
        while (userText.ToLower() != "quit")
        {
            Console.WriteLine("Enter quit to stop");
            userText = Console.ReadLine();
            if (userText.ToLower() != "quit")
            {
                await GetLanguage(userText);
            }
        }
    }

    static async Task GetLanguage(string text)
    {
        try
        {
            var jsonBody = new JObject(new JProperty("documents"),
                new JArray(new JObject(new JProperty("id", 1), new JProperty("text"), text)));

            var utf8 = new UTF8Encoding(true, true);
            var encoded = utf8.GetBytes(jsonBody.ToString());

            Console.WriteLine(utf8.GetString(encoded, 0, encoded.Length));

            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);

            var uri = $"{baseUrl}/text/analytics/v3.0/languages?" + queryString;

            HttpResponseMessage response;
            using (var content = new ByteArrayContent(encoded))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);
            }

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var results = JObject.Parse(responseContent);
                Console.WriteLine(results.ToString());

                foreach (var document in results["documents"]!)
                {
                    Console.WriteLine($"\nLanguage: {(string) document["detectedLanguage"]?["name"]!}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}