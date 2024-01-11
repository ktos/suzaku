using System.Xml.Linq;

namespace Suzaku.Chat.Services
{
    public class FileHandler
    {
        private readonly HttpClient client;

        public FileHandler(IHttpClientFactory httpClientFactory)
        {
            client = httpClientFactory.CreateClient();
        }

        public async Task<string> HandleUploadAsync(Stream stream, string name)
        {
            var extension = Path.GetExtension(name);
            var random = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            var newName = $"{random}{extension}";
            using var fileStream = File.OpenWrite(Path.Combine("uploads", newName));
            await stream.CopyToAsync(fileStream);

            return newName;
        }

        public async Task<string?> HandleAttachmentFromUri(string uri)
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
