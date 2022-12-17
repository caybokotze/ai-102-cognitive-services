// See https://aka.ms/new-console-template for more information

using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Text.UnderstandConversationalLanguage;

public abstract class Program
{
    private static HttpClient? _client;
    private static string? _baseUri;
    public static async Task Main()
    {
        // Setup
        // _baseUri = "https://westus2.api.cognitive.microsoft.com/language/customize-conversation";
        // _baseUri = "https://cluresource.cognitiveservices.azure.com/language/customize-conversation";
        _baseUri = $"https://{Environment.GetEnvironmentVariable("AZURE_COGNITIVE_ENDPOINT")?.TrimEnd('/')}/language/customize-conversation";

        _client = new HttpClient();
        _client.DefaultRequestHeaders.Add("ocp-apim-subscription-key", Environment.GetEnvironmentVariable("AZURE_COGNITIVE_TOKEN"));

        var projectName = "mytestproject";

        // Create new project by importing a project's JSON file.
        var projectJson = File.ReadAllText("my_clu_project.json");
        await ImportProjectAsync(projectName, projectJson);

        // Train the project.
        await TrainProjectAsync(projectName, "myfirstmodel");

        // Deploy the trained model.
        await DeployProjectAsync(projectName, "myfirstmodel","production");
    }

    private static async Task ImportProjectAsync(string projectName, string projectJson)
    {
        string? pollingUrl;

        // Submit the import request.
        using (var content = new StringContent(projectJson, Encoding.UTF8,"application/json"))
        {
            // Specify the format of the file being imported. 
            content.Headers.Add("format", "clu");

            var requestUri = $"{_baseUri}/projects/{projectName}/import?api-version=DEFINE-API-VERSION";
            using (var response = await _client?.PostAsync(requestUri, content)!)
            {
                response.EnsureSuccessStatusCode();
                pollingUrl = response.Headers.Location?.AbsoluteUri;
            }
        }

        // Wait for the import to finish.
        bool importSucceeded;
        do
        {
            using (var response = await _client.GetAsync(pollingUrl))
            {
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();

                var jobDetails = JsonSerializer.Deserialize<JobDetails>(responseBody);
                if (jobDetails?.Status == JobStatus.Failed)
                {
                    throw new Exception($"Deployment failed. JobId: {jobDetails.JobId}");
                }

                importSucceeded = jobDetails?.Status == JobStatus.Succeeded;
            }

            await Task.Delay(TimeSpan.FromSeconds(1));
        }
        while (!importSucceeded);
    }

    private static async Task TrainProjectAsync(string projectName, string modelName)
    {
        var trainingRequestPayload = new TrainingRequestPayload { TrainingModelName = modelName };

        string? pollingUrl;

        // Submit the training request.
        var body = JsonSerializer.Serialize(trainingRequestPayload);
        using (var content = new StringContent(body, Encoding.UTF8,"application/json"))
        {
            var requestUri = $"{_baseUri}/projects/{projectName}/train?api-version=2021-07-15-preview";
            using (var response = await _client?.PostAsync(requestUri, content)!)
            {
                response.EnsureSuccessStatusCode();
                pollingUrl = response.Headers.Location?.AbsoluteUri;
            }
        }

        // Wait for the training to finish. This may take some time.
        bool trainingSucceeded;
        do
        {
            using (var response = await _client.GetAsync(pollingUrl))
            {
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();

                var jobDetails = JsonSerializer.Deserialize<JobDetails>(responseBody);
                if (jobDetails?.Status == JobStatus.Failed)
                {
                    throw new Exception($"Training failed. JobId: {jobDetails.JobId}");
                }

                trainingSucceeded = jobDetails?.Status == JobStatus.Succeeded;
            }

            await Task.Delay(TimeSpan.FromSeconds(1));
        }
        while (!trainingSucceeded);
    }

    private static async Task DeployProjectAsync(string projectName, string modelName, string deploymentName)
    {
        var deploymentRequestPayload = new DeploymentRequestPayload { TrainingModelName = modelName };

        string? pollingUrl;

        // Create or update the deployment to point to our latest deployed model.
        var body = JsonSerializer.Serialize(deploymentRequestPayload);
        using (var content = new StringContent(body, Encoding.UTF8, "application/json"))
        {
            var requestUri = $"{_baseUri}/projects/{projectName}/deployments/{deploymentName}?api-version=2021-07-15-preview";
            using (var response = await _client?.PutAsync(requestUri, content)!)
            {
                response.EnsureSuccessStatusCode();
                pollingUrl = response.Headers.Location?.AbsoluteUri;
            }
        }

        // Wait for the deployment to finish.
        bool deploymentSucceeded;
        do
        {
            using (var response = await _client.GetAsync(pollingUrl))
            {
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();

                var jobDetails = JsonSerializer.Deserialize<JobDetails>(responseBody);
                if (jobDetails?.Status == JobStatus.Failed)
                {
                    throw new Exception($"Deployment failed. JobId: {jobDetails.JobId}");
                }

                deploymentSucceeded = jobDetails?.Status == JobStatus.Succeeded;
            }

            await Task.Delay(TimeSpan.FromSeconds(1));
        }
        while (!deploymentSucceeded);
    }

    private class TrainingRequestPayload
    {
        [JsonPropertyName("trainingModelName")]
        public string? TrainingModelName { get; set; }
    }

    private class DeploymentRequestPayload
    {
        [JsonPropertyName("trainingModelName")]
        public string? TrainingModelName { get; set; }
    }

    private class JobDetails
    {
        /// <summary>
        /// Gets or sets the Job ID.
        /// </summary>
        [JsonPropertyName("jobId")]
        public string? JobId { get; set; }

        /// <summary>
        /// Gets or sets the Job Created Date Time.
        /// </summary>
        [JsonPropertyName("createdDateTime")]
        public DateTime CreatedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the Job Last Updated Date Time.
        /// </summary>
        [JsonPropertyName("lastUpdatedDateTime")]
        public DateTime LastUpdatedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the Job Expiration Date Time.
        /// </summary>
        [JsonPropertyName("expirationDateTime")]
        public DateTime? ExpirationDateTime { get; set; }

        /// <summary>
        /// Gets or sets the Job Status.
        /// </summary>
        [JsonPropertyName("status")]
        public JobStatus Status { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    private enum JobStatus
    {
        NotStarted,
        InProgress,
        Succeeded,
        PartialSuccess = 4,
        Failed
    }
}