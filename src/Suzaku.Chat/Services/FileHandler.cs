using System.Xml.Linq;

namespace Suzaku.Chat.Services
{
    /// <summary>
    /// A service responsible for handling files
    /// </summary>
    public class FileHandler
    {
        private readonly HttpClient client;

        public FileHandler(IHttpClientFactory httpClientFactory)
        {
            client = httpClientFactory.CreateClient();
        }

        /// <summary>
        /// Handles uploading files into a chat
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="name"></param>
        /// <returns>A name for an uploaded file in the /uploads directory</returns>
        public async Task<string> HandleUploadAsync(Stream stream, string name)
        {
            var extension = Path.GetExtension(name);
            var random = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            var newName = $"{random}{extension}";
            using var fileStream = File.OpenWrite(Path.Combine("uploads", newName));
            await stream.CopyToAsync(fileStream);

            return newName;
        }

        /// <summary>
        /// Handles downloading files from the internet into a chat
        /// </summary>
        /// <param name="uri"></param>
        /// <returns>A name for an uploaded file in the /uploads directory</returns>
        public async Task<string?> HandleAttachmentFromUriAsync(string uri)
        {
            try
            {
                Uri u = new Uri(uri);
                var extension = Path.GetExtension(u.LocalPath);
                var random = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
                var newName = $"{random}{extension}";
                using var fileStream = File.OpenWrite(Path.Combine("uploads", newName));

                var stream = await client.GetStreamAsync(uri);
                await stream.CopyToAsync(fileStream);

                return newName;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
