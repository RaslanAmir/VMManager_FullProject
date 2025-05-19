using System.Threading.Tasks;

namespace VMManager.Core.Interfaces
{
    /// <summary>
    /// Defines a contract for asynchronous file-based persistence operations.
    /// </summary>
    public interface IFileRepository
    {
        /// <summary>
        /// Reads and deserializes a file's content to the specified type.
        /// </summary>
        /// <typeparam name="T">The type to deserialize to.</typeparam>
        /// <param name="path">The full or relative file path.</param>
        /// <returns>Deserialized object, or null if not found or deserialization fails.</returns>
        Task<T?> ReadAsync<T>(string path) where T : class;

        /// <summary>
        /// Serializes and writes an object to the specified file path.
        /// </summary>
        /// <typeparam name="T">The type of content to serialize.</typeparam>
        /// <param name="path">The file path to write to.</param>
        /// <param name="content">The object to serialize.</param>
        Task WriteAsync<T>(string path, T content) where T : class;

        /// <summary>
        /// Checks whether a file exists at the given path.
        /// </summary>
        /// <param name="path">The file path to check.</param>
        /// <returns>True if the file exists; otherwise, false.</returns>
        Task<bool> ExistsAsync(string path);
    }
}
