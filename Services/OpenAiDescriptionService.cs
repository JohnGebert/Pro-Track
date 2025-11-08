using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProTrack.Configuration;

namespace ProTrack.Services
{
    /// <summary>
    /// Default implementation of <see cref="IAiDescriptionService"/> using chat-completion APIs.
    /// </summary>
    public class OpenAiDescriptionService : IAiDescriptionService
    {
        private readonly HttpClient _httpClient;
        private readonly AiDescriptionOptions _options;
        private readonly ILogger<OpenAiDescriptionService> _logger;

        private static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        private const string DefaultSystemPrompt = """
You are an assistant that writes professional, billable time entry descriptions for client invoices.
Summarize the work in 2-3 concise sentences, highlight deliverables, mention tools or documents reviewed when relevant, and avoid first-person language.
""";

        public OpenAiDescriptionService(
            HttpClient httpClient,
            IOptions<AiDescriptionOptions> options,
            ILogger<OpenAiDescriptionService> logger)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _logger = logger;

            if (!string.IsNullOrWhiteSpace(_options.BaseUrl))
            {
                _httpClient.BaseAddress = new Uri(_options.BaseUrl, UriKind.Absolute);
            }
        }

        public async Task<AiDescriptionResult> GenerateDescriptionAsync(AiDescriptionRequest request)
        {
            if (!_options.Enabled)
            {
                return AiDescriptionResult.FromError("AI description assistance is disabled. Ask your administrator to enable it in configuration.");
            }

            if (string.IsNullOrWhiteSpace(_options.ApiKey))
            {
                _logger.LogWarning("AI description service attempted to run without an API key configured.");
                return AiDescriptionResult.FromError("AI description service is not fully configured. Please provide an API key.");
            }

            if (string.IsNullOrWhiteSpace(request.Prompt))
            {
                return AiDescriptionResult.FromError("A prompt is required to generate a description.");
            }

            try
            {
                using var message = BuildHttpRequest(request);
                using var response = await _httpClient.SendAsync(message);

                if (!response.IsSuccessStatusCode)
                {
                    var errorPayload = await response.Content.ReadAsStringAsync();
                    _logger.LogError("AI provider returned a non-success status {StatusCode}. Payload: {Payload}", response.StatusCode, errorPayload);
                    return AiDescriptionResult.FromError("The AI provider returned an error while generating the description.");
                }

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ChatCompletionResponse>(json, SerializerOptions);

                var content = result?.Choices?[0]?.Message?.Content;
                if (string.IsNullOrWhiteSpace(content))
                {
                    _logger.LogWarning("AI provider returned an empty response.");
                    return AiDescriptionResult.FromError("The AI provider returned an empty response.");
                }

                return AiDescriptionResult.FromSuccess(content.Trim());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while generating AI description.");
                return AiDescriptionResult.FromError("An unexpected error occurred while generating the description.");
            }
        }

        private HttpRequestMessage BuildHttpRequest(AiDescriptionRequest request)
        {
            var endpoint = string.IsNullOrWhiteSpace(_options.Endpoint) ? "v1/chat/completions" : _options.Endpoint;
            var requestUri = BuildRequestUri(endpoint);

            var systemPrompt = string.IsNullOrWhiteSpace(_options.SystemPrompt)
                ? DefaultSystemPrompt
                : _options.SystemPrompt;

            var userPromptBuilder = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(request.ProjectName))
            {
                userPromptBuilder.AppendLine($"Project: {request.ProjectName}");
            }

            if (request.DurationHours.HasValue)
            {
                userPromptBuilder.AppendLine($"Duration: approximately {request.DurationHours.Value:F2} hours.");
            }

            if (!string.IsNullOrWhiteSpace(request.AdditionalContext))
            {
                userPromptBuilder.AppendLine(request.AdditionalContext.Trim());
            }

            userPromptBuilder.AppendLine(request.Prompt.Trim());

            var payload = new
            {
                model = _options.Model,
                temperature = _options.Temperature,
                max_tokens = _options.MaxTokens,
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = userPromptBuilder.ToString().Trim() }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(payload, SerializerOptions), Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = content
            };

            var scheme = string.IsNullOrWhiteSpace(_options.AuthenticationScheme)
                ? "Bearer"
                : _options.AuthenticationScheme;

            if (string.Equals(scheme, "api-key", StringComparison.OrdinalIgnoreCase))
            {
                requestMessage.Headers.Add("api-key", _options.ApiKey);
            }
            else
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue(scheme, _options.ApiKey);
            }

            return requestMessage;
        }

        private Uri BuildRequestUri(string endpoint)
        {
            Uri requestUri;

            if (_httpClient.BaseAddress != null)
            {
                requestUri = new Uri(_httpClient.BaseAddress, endpoint);
            }
            else if (Uri.TryCreate(endpoint, UriKind.Absolute, out var absoluteUri))
            {
                requestUri = absoluteUri;
            }
            else
            {
                requestUri = new Uri(new Uri("https://api.openai.com/"), endpoint);
            }

            if (!string.IsNullOrWhiteSpace(_options.ApiVersion) &&
                !requestUri.Query.Contains("api-version", StringComparison.OrdinalIgnoreCase))
            {
                var queryPrefix = string.IsNullOrEmpty(requestUri.Query) ? string.Empty : requestUri.Query.TrimStart('?') + "&";
                var builder = new UriBuilder(requestUri)
                {
                    Query = $"{queryPrefix}api-version={Uri.EscapeDataString(_options.ApiVersion)}"
                };
                requestUri = builder.Uri;
            }

            return requestUri;
        }

        private sealed class ChatCompletionResponse
        {
            public Choice[]? Choices { get; set; }
        }

        private sealed class Choice
        {
            public Message? Message { get; set; }
        }

        private sealed class Message
        {
            public string? Content { get; set; }
        }
    }
}

