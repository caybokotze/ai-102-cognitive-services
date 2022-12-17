// See https://aka.ms/new-console-template for more information

using System.Collections.ObjectModel;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

var client = new FaceClient(new ApiKeyServiceClientCredentials(""))
{
    Endpoint = ""
};

client.Face.DetectWithUrlWithHttpMessagesAsync("", true, true,
    new Collection<FaceAttributeType>
    {
        FaceAttributeType.Age,
        FaceAttributeType.Gender,
        FaceAttributeType.Emotion
    });