using Azure;
using Azure.AI.OpenAI;
using IntellAccount.Constants;
using IntellAccount.Models;
using Microsoft.Extensions.Options;

namespace IntellAccount.Services;

public class OpenAiService
{
    private readonly OpenAiConfig _openAiCredentials;
    public OpenAiService(OpenAiConfig config)
    {
        _openAiCredentials = config;
    }
    public OpenAiService(IOptions<OpenAiConfig> config)
    {
        _openAiCredentials = config.Value;
    }
    private IList<ChatMessage> messages = new List<ChatMessage>();
    public OpenAiService()
    {
        messages.Add(new ChatMessage(ChatRole.System, PromptConstants.DefaultPrompt));
    }

    public string GetResponseFromQuestion(string question)
    {
        OpenAIClient client = new(new Uri(_openAiCredentials.Endpoint), new AzureKeyCredential(_openAiCredentials.Key));

        if (question != string.Empty)
        {
            messages.Add(new ChatMessage(ChatRole.User, question));

            var chatCompletionsOptions = new ChatCompletionsOptions(messages);

            Response<ChatCompletions> response = client.GetChatCompletions(
                deploymentOrModelName: _openAiCredentials.DeploymentName,
                chatCompletionsOptions);

            messages.Add(new ChatMessage(ChatRole.Assistant, response.Value.Choices[0].Message.Content));
            return response.Value.Choices[0].Message.Content;
        }
        return string.Empty;
    }
}
