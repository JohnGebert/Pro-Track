using System.Threading.Tasks;

namespace ProTrack.Services
{
    /// <summary>
    /// Contract for generating professional time entry descriptions with AI assistance.
    /// </summary>
    public interface IAiDescriptionService
    {
        /// <summary>
        /// Generates a billable-ready description based on the supplied context.
        /// </summary>
        /// <param name="request">Request payload containing the user's prompt and optional metadata.</param>
        /// <returns>Generation result including the suggested description or any error information.</returns>
        Task<AiDescriptionResult> GenerateDescriptionAsync(AiDescriptionRequest request);
    }

    /// <summary>
    /// Request payload used when generating descriptions.
    /// </summary>
    public class AiDescriptionRequest
    {
        public string Prompt { get; set; } = string.Empty;
        public string? ProjectName { get; set; }
        public decimal? DurationHours { get; set; }
        public string? AdditionalContext { get; set; }
    }

    /// <summary>
    /// Result returned after attempting to generate a description.
    /// </summary>
    public class AiDescriptionResult
    {
        public bool Success { get; init; }
        public string? Description { get; init; }
        public string? Error { get; init; }

        public static AiDescriptionResult FromSuccess(string description) =>
            new AiDescriptionResult { Success = true, Description = description };

        public static AiDescriptionResult FromError(string error) =>
            new AiDescriptionResult { Success = false, Error = error };
    }
}

