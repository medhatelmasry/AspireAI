using System;

namespace AiLibrary;

public class Helper
{
    public static (string endpoint, string key) ParseAiConnectionString(string connectionString)
    {
        var parts = connectionString.Split(';');
        var endpoint = parts.FirstOrDefault(part => part.StartsWith("Endpoint="))?.Split('=')[1];
        var key = parts.FirstOrDefault(part => part.StartsWith("Key="))?.Split('=')[1];
        return (endpoint!, key!);
    }
}
