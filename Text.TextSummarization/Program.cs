// See https://aka.ms/new-console-template for more information

using Azure;
using Azure.AI.TextAnalytics;

namespace Text.TextSummarization;

internal abstract class Program
{
    private static async Task Main(string[] args)
    {
        var client = new TextAnalyticsClient(Endpoint, Credentials);
        await TextSummarizationExample(client);
    }
    
    private static readonly AzureKeyCredential Credentials = new(Environment.GetEnvironmentVariable("AZURE_COGNITIVE_TOKEN") ?? string.Empty);
    private static readonly Uri Endpoint = new(Environment.GetEnvironmentVariable("AZURE_COGNITIVE_ENDPOINT") ?? string.Empty);

    // Example method for summarizing text
    private static async Task TextSummarizationExample(TextAnalyticsClient client)
    {
        const string document =
            @"The extractive summarization feature in Text Analytics uses natural language processing techniques to locate key sentences in an unstructured text document. 
                These sentences collectively convey the main idea of the document. This feature is provided as an API for developers. 
                They can use it to build intelligent solutions based on the relevant information extracted to support various use cases. 
                In the public preview, extractive summarization supports several languages. It is based on pretrained multilingual transformer models, part of our quest for holistic representations. 
                It draws its strength from transfer learning across monolingual and harness the shared nature of languages to produce models of improved quality and efficiency.";

        // Prepare analyze operation input. You can add multiple documents to this list and perform the same
        // operation to all of them.
        var batchInput = new List<string>
        {
            document
        };

        var actions = new TextAnalyticsActions()
        {
            ExtractKeyPhrasesActions = new List<ExtractKeyPhrasesAction> {new()}
        };

        // Start analysis process.
        var operation = await client.StartAnalyzeActionsAsync(batchInput, actions);
        await operation.WaitForCompletionAsync();
        // View operation status.
        Console.WriteLine($"AnalyzeActions operation has completed");
        Console.WriteLine();

        Console.WriteLine($"Created On   : {operation.CreatedOn}");
        Console.WriteLine($"Expires On   : {operation.ExpiresOn}");
        Console.WriteLine($"Id           : {operation.Id}");
        Console.WriteLine($"Status       : {operation.Status}");

        Console.WriteLine();
        // View operation results.
        await foreach (var documentsInPage in operation.Value)
        {
            var summaryResults = documentsInPage.ExtractKeyPhrasesResults;

            foreach (var summaryActionResults in summaryResults)
            {
                if (summaryActionResults.HasError)
                {
                    Console.WriteLine("  Error!");
                    Console.WriteLine($"  Action error code: {summaryActionResults.Error.ErrorCode}.");
                    Console.WriteLine($"  Message: {summaryActionResults.Error.Message}");
                    continue;
                }

                foreach (var documentResults in summaryActionResults.DocumentsResults)
                {
                    if (documentResults.HasError)
                    {
                        Console.WriteLine("  Error!");
                        Console.WriteLine($"  Document error code: {documentResults.Error.ErrorCode}.");
                        Console.WriteLine($"  Message: {documentResults.Error.Message}");
                        continue;
                    }

                    Console.WriteLine($"  Extracted the following {documentResults.KeyPhrases.Count} sentence(s):");
                    Console.WriteLine();

                    foreach (var sentence in documentResults.KeyPhrases)
                    {
                        Console.WriteLine($"  Key phrase: {sentence}");
                        Console.WriteLine();
                    }
                }
            }
        }
    }
}