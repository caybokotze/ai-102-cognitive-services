// See https://aka.ms/new-console-template for more information

using System.Text;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Rest;

Console.WriteLine("Starting the image recognition process");
var client = new ComputerVisionClient(new ApiKeyServiceClientCredentials("1fec08ae7bb8430bb024200427f80c0f"))
{
    Endpoint = "https://merge-cognitive-services.cognitiveservices.azure.com/"
};
var result = await client.DescribeImageWithHttpMessagesAsync("https://image.jimcdn.com/app/cms/image/transf/none/path/s86059e2b11eeffee/image/i47f1fbff67330f91/version/1518177349/image.jpg");
if (result.Response.IsSuccessStatusCode)
{
    var content = result.Response.Content;
    var stream = content.ReadAsStream();
    var ms = new MemoryStream();
    await stream.CopyToAsync(ms);
    var resultAsJson = Encoding.UTF8.GetString(ms.ToArray());
    Console.WriteLine(resultAsJson);
}

var result2 = await client.AnalyzeImageWithHttpMessagesAsync("https://image.jimcdn.com/app/cms/image/transf/none/path/s86059e2b11eeffee/image/i47f1fbff67330f91/version/1518177349/image.jpg");
if (result2.Response.IsSuccessStatusCode)
{
    var content = result2.Response.Content;
    var stream = content.ReadAsStream();
    var ms = new MemoryStream();
    await stream.CopyToAsync(ms);
    var resultAsJson = Encoding.UTF8.GetString(ms.ToArray());
    Console.WriteLine(resultAsJson);
}