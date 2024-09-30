using System.Text.Json;
using static System.Text.Json.JsonSerializer;

public static class ConfigService
{
    private static SurveyConfig config;

    static ConfigService()
    {
        string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "SurveyConfig.json");
        string json = File.ReadAllText(configPath);
        config = JsonSerializer.Deserialize<SurveyConfig>(json);
    }

    public static SurveyConfig GetConfig()
    {
        return config;
    }
}