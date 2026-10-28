namespace EntranceManager.Configuration
{
    public class GeminiOptions
    {
        public string ApiKey { get; set; } = string.Empty;
        public string Model { get; set; } = "gemini-3.1-flash-lite";
        public int RequestTimeoutSeconds { get; set; } = 60;
    }
}