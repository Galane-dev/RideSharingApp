using System.Text.Json;

namespace RideSharing.Infrastructure.Persistence
{
    /// <summary>
    /// Handles generic JSON serialisation and deserialisation for file-based persistence.
    /// Uses a named system Mutex per file so multiple running instances of the application
    /// can safely read and write the same JSON files without collisions or partial reads.
    /// </summary>
    public class JsonFileService
    {
        private static readonly JsonSerializerOptions SerialiserOptions =
            new() { WriteIndented = true };

        /// <summary>
        /// Loads a list of entities from the specified JSON file.
        /// Acquires a named Mutex for the duration of the read to block concurrent writers.
        /// Returns an empty list when the file does not yet exist.
        /// </summary>
        public List<T> Load<T>(string path)
        {
            using var mutex = AcquireMutex(path);

            if (!File.Exists(path))
                return new List<T>();

            // Open with ReadWrite share so other processes can still open the file,
            // but the Mutex above ensures only one process is inside this block at a time.
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var reader = new StreamReader(stream);

            var json = reader.ReadToEnd();

            return JsonSerializer.Deserialize<List<T>>(json, SerialiserOptions)
                   ?? new List<T>();
        }

        /// <summary>
        /// Serialises the provided collection and writes it to the specified file.
        /// Acquires a named Mutex for the duration of the write to block concurrent readers and writers.
        /// Creates any missing directories automatically.
        /// </summary>
        public void Save<T>(string path, List<T> data)
        {
            EnsureDirectoryExists(path);

            using var mutex = AcquireMutex(path);

            var json = JsonSerializer.Serialize(data, SerialiserOptions);

            // Write to a temp file first, then atomically replace the target.
            // This prevents a crash mid-write from leaving a corrupt JSON file.
            var tempPath = path + ".tmp";
            File.WriteAllText(tempPath, json);
            File.Move(tempPath, path, overwrite: true);
        }

        /// <summary>
        /// Creates and acquires a named system Mutex scoped to the given file path.
        /// The Mutex name is derived from the path so each file gets its own independent lock.
        /// The caller is responsible for disposing the returned Mutex to release the lock.
        /// </summary>
        private static Mutex AcquireMutex(string filePath)
        {
            // Replace characters that are invalid in Mutex names.
            var safeName = "RideSharing_" + filePath.Replace('/', '_').Replace('\\', '_').Replace(':', '_');
            var mutex = new Mutex(initiallyOwned: false, name: safeName);

            // Wait indefinitely for the lock. In a production system a timeout would be appropriate.
            mutex.WaitOne();

            return mutex;
        }

        private static void EnsureDirectoryExists(string filePath)
        {
            var directory = Path.GetDirectoryName(filePath);

            if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);
        }
    }
}
