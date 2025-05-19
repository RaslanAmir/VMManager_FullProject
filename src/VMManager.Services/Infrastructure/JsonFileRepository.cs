using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using VMManager.Core.Interfaces;

namespace VMManager.Services.Infrastructure
{
    /// <summary>
    /// Provides thread-safe JSON-based file persistence for application data.
    /// </summary>
    public sealed class JsonFileRepository : IFileRepository, IDisposable
    {
        private readonly SemaphoreSlim _lock = new(1, 1);

        private readonly JsonSerializerOptions _options = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        /// <summary>
        /// Reads and deserializes an object from a JSON file.
        /// </summary>
        /// <typeparam name="T">The type of object to deserialize.</typeparam>
        /// <param name="path">The file path to read from.</param>
        /// <returns>The deserialized object or null if the file doesn't exist.</returns>
        public async Task<T?> ReadAsync<T>(string path) where T : class
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Path cannot be null or empty.", nameof(path));

            if (!File.Exists(path))
                return null;

            await _lock.WaitAsync().ConfigureAwait(false);
            try
            {
                await using var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                return await JsonSerializer.DeserializeAsync<T>(stream, _options).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Optionally log
                throw new IOException($"Failed to read file '{path}': {ex.Message}", ex);
            }
            finally
            {
                _lock.Release();
            }
        }

        /// <summary>
        /// Serializes and writes an object to a JSON file.
        /// </summary>
        /// <typeparam name="T">The type of object to serialize.</typeparam>
        /// <param name="path">The file path to write to.</param>
        /// <param name="content">The object to serialize.</param>
        public async Task WriteAsync<T>(string path, T content) where T : class
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Path cannot be null or empty.", nameof(path));

            if (content == null)
                throw new ArgumentNullException(nameof(content));

            var tempPath = Path.GetTempFileName();

            await _lock.WaitAsync().ConfigureAwait(false);
            try
            {
                await using (var stream = File.Open(tempPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await JsonSerializer.SerializeAsync(stream, content, _options).ConfigureAwait(false);
                }

                File.Copy(tempPath, path, overwrite: true);
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to write file '{path}': {ex.Message}", ex);
            }
            finally
            {
                _lock.Release();
                File.Delete(tempPath);
            }
        }

        /// <summary>
        /// Determines if a file exists at the specified path.
        /// </summary>
        /// <param name="path">The file path to check.</param>
        public Task<bool> ExistsAsync(string path)
        {
            return Task.FromResult(File.Exists(path));
        }

        /// <summary>
        /// Releases all unmanaged resources held by this instance.
        /// </summary>
        public void Dispose()
        {
            _lock.Dispose();
        }
    }
}
