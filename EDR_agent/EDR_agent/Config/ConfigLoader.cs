using System;
using System.IO;
using System.Text.Json;

namespace EDR_agent.Config
{
    public static class ConfigLoader
    {
        private const string DEFAULT_CONFIG_FILENAME = "agent_config.json";

        public static AgentOptions LoadOrDefault(string appDataFolder)
        {
            var cfgPath = Path.Combine(appDataFolder, DEFAULT_CONFIG_FILENAME);

            var options = new AgentOptions();

            if (File.Exists(cfgPath))
            {
                try
                {
                    var json = File.ReadAllText(cfgPath);
                    var loaded = JsonSerializer.Deserialize<AgentOptions>(json);
                    if (loaded != null)
                    {
                        options = loaded;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to load config '{cfgPath}': {ex.Message}. Using defaults.");
                }
            }
            else
            {
                Console.WriteLine($"Config file not found at: {cfgPath}. Using defaults. To customize, create this JSON file.");
            }

            return options;
        }
    }
}