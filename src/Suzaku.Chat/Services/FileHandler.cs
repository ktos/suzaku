namespace Suzaku.Chat.Services
{
    public class FileHandler
    {
        public async Task<string> HandleUploadAsync(Stream stream, string name)
        {
            var extension = Path.GetExtension(name);
            var random = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            var newName = $"{random}{extension}";
            using var fileStream = File.OpenWrite(Path.Combine("uploads", newName));
            await stream.CopyToAsync(fileStream);

            return newName;
        }
    }
}
