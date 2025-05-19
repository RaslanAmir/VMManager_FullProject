using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VMManager.Core.Interfaces;

namespace VMManager.Services.Repositories
{
    /// <summary>
    /// Implements file-based storage using JSON serialization.
    /// </summary>
    public class FileRepository : IFileRepository
    {
        /// <inheritdoc />
        public async Task<T?> ReadAsync<T>(string path) where T : class
        {
            if (!File.Exists(path))
                return null;

            var json = await File.ReadAllTextAsync(path, Encoding.UTF8);
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <inheritdoc />
        public async Task WriteAsync<T>(string path, T content) where T : class
        {
            var json = JsonConvert.SerializeObject(content, Formatting.Indented);
            var directory = Path.GetDirectoryName(path);

            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            await File.WriteAllTextAsync(path, json, Encoding.UTF8);
        }

        /// <inheritdoc />
        public Task<bool> ExistsAsync(string path)
        {
            return Task.FromResult(File.Exists(path));
        }
    }
}
