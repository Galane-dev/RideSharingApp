using System.Text.Json;

namespace RideSharing.Infrastructure.Persistence
{
    /// <summary>
    /// Handles generic JSON serialisation and deserialisation for file-based persistence.
    /// All file I/O is isolated here so repositories remain free of IO concerns.
    /// </summary>
    public class JsonFileService
    {
        private static readonly JsonSerializerOptions SerialiserOptions =
            new() { WriteIndented = true };

        /// <summary>
        /// Loads a list of entities from the specified JSON file.
        /// Returns an empty list when the file does not yet exist.
        /// </summary>
        public List<T> Load<T>(string path)
        {
            if (!File.Exists(path))
                return new List<T>();

            var json = File.ReadAllText(path);

            return JsonSerializer.Deserialize<List<T>>(json, SerialiserOptions)
                   ?? new List<T>();
        }

        /// <summary>
        /// Serialises the provided collection and writes it to the specified file.
        /// Creates any missing directories automatically.
        /// </summary>
        public void Save<T>(string path, List<T> data)
        {
            EnsureDirectoryExists(path);

            var json = JsonSerializer.Serialize(data, SerialiserOptions);

            File.WriteAllText(path, json);
        }

        private static void EnsureDirectoryExists(string filePath)
        {
            var directory = Path.GetDirectoryName(filePath);

            if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);
        }
    }
}
