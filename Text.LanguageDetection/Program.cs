// See https://aka.ms/new-console-template for more information

using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Text.Translate;

public class Program
{
    private static string Key = Environment.GetEnvironmentVariable("AZURE_COGNITIVE_TOKEN") ?? string.Empty;
    private static string BaseUrl = Environment.GetEnvironmentVariable("AZURE_COGNITIVE_ENDPOINT") ?? string.Empty;

    private static async Task Main(string[] args)
    {
        var userText = "";
        while (userText?.ToLower() != "quit")
        {
            Console.WriteLine("Enter quit to stop");
            userText = Console.ReadLine();
            if (userText?.ToLower() != "quit")
            {
                await GetLanguage(userText ?? string.Empty);
            }
        }
    }

    static async Task GetLanguage(string text)
    {
        try
        {
            /*
             * JObject
             * "documents": [{"id": 1, "text": text}]
             */
            var jsonBody = JsonConvert.SerializeObject(new QueryDocument
            {
                Documents = new DocumentItem[]
                {
                    new ()
                    {
                        Id = 1,
                        Text = text
                    }
                }
            }, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented, ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new KebabCaseNamingStrategy()
                }
            });

            var utf8 = new UTF8Encoding(true, true);
            var encoded = utf8.GetBytes(jsonBody);

            Console.WriteLine(utf8.GetString(encoded, 0, encoded.Length));

            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Key);

            var uri = $"{BaseUrl}/text/analytics/v3.0/languages?" + queryString;

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

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.ReasonPhrase);
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}

public class QueryDocument
{
    public DocumentItem[]? Documents { get; set; }
}

public class DocumentItem
{
    public int Id { get; set; }
    public string? Text { get; set; }
}