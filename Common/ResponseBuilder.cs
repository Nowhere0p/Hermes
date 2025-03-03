using System.Text.Json;

namespace Hermes.Common;

public static class DummyResponseBuilder {
    public static T DeserializeFromFile<T>(string path) {
        try {
            var json = File.ReadAllText(path);
            T result = JsonSerializer.Deserialize<T>(json);
            return result;
        } catch(FileNotFoundException e) {
            throw new HermesException(HermesException.NotFound, "File not found", e.Message);

        } catch(JsonException e) {
            throw new HermesException(HermesException.NotFound, "Json Error", e.Message);

        } catch (Exception e) {
            throw new HermesException(HermesException.InternalServerError, "Error deserializing file", e.Message);
        }
    }
}