namespace ProTrack.Configuration
{
    /// <summary>
    /// Configuration options for AI-generated time entry descriptions.
    /// </summary>
    public class AiDescriptionOptions
    {
        /// <summary>
        /// Set to true to enable the AI description service.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Friendly name of the provider (e.g., OpenAI, AzureOpenAI).
        /// </summary>
        public string Provider { get; set; } = "OpenAI";

        /// <summary>
        /// Base URL for the AI provider.
        /// </summary>
        public string BaseUrl { get; set; } = "https://api.openai.com/";

        /// <summary>
        /// Endpoint path for chat completions.
        /// </summary>
        public string Endpoint { get; set; } = "v1/chat/completions";

        /// <summary>
        /// Model or deployment name to use.
        /// </summary>
        public string Model { get; set; } = "gpt-4o-mini";

        /// <summary>
        /// API key or token used for authentication.
        /// </summary>
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>
        /// Optional API version (used by Azure OpenAI).
        /// </summary>
        public string? ApiVersion { get; set; }

        /// <summary>
        /// Authentication scheme for the API key header (e.g., Bearer, ApiKey).
        /// </summary>
        public string AuthenticationScheme { get; set; } = "Bearer";

        /// <summary>
        /// Temperature value controlling creativity.
        /// </summary>
        public double Temperature { get; set; } = 0.3;

        /// <summary>
        /// Maximum tokens for the response.
        /// </summary>
        public int MaxTokens { get; set; } = 300;

        /// <summary>
        /// Optional custom system prompt override.
        /// </summary>
        public string? SystemPrompt { get; set; }
    }
}

