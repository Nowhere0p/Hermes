using System.Text.Json.Serialization;

namespace Hermes.Common;

public class HermesConfiguration {
    [JsonPropertyName("emailConfigs")]
    public EmailConfigs? EmailConfigs { get; set; }
}

public class EmailConfigs {
    [JsonPropertyName("host")]
    public string? Host { get; set; }
    [JsonPropertyName("port")]
    public int? Port { get; set; }
    [JsonPropertyName("username")]
    public string? Username { get; set; }
    [JsonPropertyName("password")]
    public string? Password { get; set; }
    [JsonPropertyName("useSSL")]
    public bool? UseSSL { get; set; }
}