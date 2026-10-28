namespace EntranceManager.Configuration
{
    public class OpenAIOptions
    {
        public string ApiKey { get; set; } = string.Empty;

        public string Model { get; set; } = "gpt-4.1-mini";

        public int RequestTimeoutSeconds { get; set; } = 60;
    }
}