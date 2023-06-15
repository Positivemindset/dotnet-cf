using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace DotNetCf
{
    public static class Function
    {
        [FunctionName("Run")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            // Check URL parameters for "message" field
            string message = req.Query["message"];

            // If there's a body, parse it as JSON and check for "message" field.
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            if (!string.IsNullOrEmpty(requestBody))
            {
                try
                {
                    JsonElement json = JsonSerializer.Deserialize<JsonElement>(requestBody);
                    if (json.TryGetProperty("message", out JsonElement messageElement) &&
                        messageElement.ValueKind == JsonValueKind.String)
                    {
                        message = messageElement.GetString();
                    }
                }
                catch (JsonException parseException)
                {
                    log.LogError(parseException, "Error parsing JSON request");
                }
            }

            return new OkObjectResult(message ?? "Hello World!");
        }
    }
}
