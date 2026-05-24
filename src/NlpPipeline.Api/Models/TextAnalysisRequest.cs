namespace NlpPipeline.Api.Models
{
    public class TextAnalysisRequest
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string RawText { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending"; // Pending, Processing, Completed, Failed
        public string? AnalysisResult { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
    }
}