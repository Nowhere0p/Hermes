namespace Hermes.Common;
public interface IConfigManager {
    Task<HermesConfiguration> GetConfigurationAsync();
}