// See https://aka.ms/new-console-template for more information

using Azure;
using Azure.AI.Language.QuestionAnswering;
using Microsoft.Extensions.Azure;

Uri endpoint = new Uri("https://myaccount.api.cognitive.microsoft.com");
AzureKeyCredential credential = new AzureKeyCredential("{api-key}");

QuestionAnsweringClient client = new QuestionAnsweringClient(endpoint, credential, new QuestionAnsweringClientOptions()
{
    
});

client.GetAnswersFromText(new AnswersFromTextOptions("How long should my surface battery last?",
    new List<TextDocument>()));

string projectName = "FAQ";
string deploymentName = "prod";

QueryKnowledgeBaseOptions options = new Know(projectName, deploymentName, "How long should my Surface battery last?");
client.AddQuestionAnsweringClient()
Response<Know> response = client.QueryKnowledgeBase(options);

foreach (KnowledgeBaseAnswer answer in response.Value.Answers)
{
    Console.WriteLine($"({answer.ConfidenceScore:P2}) {answer.Answer}");
    Console.WriteLine($"Source: {answer.Source}");
    Console.WriteLine();
}

// Ask a follow-up question (chit-chat)

string projectName = "FAQ";
string deploymentName = "prod";
// Answers are ordered by their ConfidenceScore so assume the user choose the first answer below:
KnowledgeBaseAnswer previousAnswer = answers.Answers.First();
QueryKnowledgeBaseOptions options = new QueryKnowledgeBaseOptions(projectName, deploymentName "How long should charging take?")
{
    Context = new KnowledgeBaseAnswerRequestContext(previousAnswer.Id.Value)
};

Response<KnowledgeBaseAnswers> response = client.QueryKnowledgeBase(options);

foreach (KnowledgeBaseAnswer answer in response.Value.Answers)
{
    Console.WriteLine($"({answer.ConfidenceScore:P2}) {answer.Answer}");
    Console.WriteLine($"Source: {answer.Source}");
    Console.WriteLine();
}