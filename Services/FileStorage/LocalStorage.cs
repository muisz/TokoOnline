namespace TokoOnline.Services
{
    public class LocalStorage : IFileStorage
    {
        private readonly string _directory;
        private readonly IHostEnvironment _environment;

        public LocalStorage(IHostEnvironment environment)
        {
            _environment = environment;
            _directory = Path.Combine(environment.ContentRootPath, "Media");
        }

        public Task<string> UploadFromBase64(string data, string filename)
        {
            CreateDirectoryIfNotExist();
            filename = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString() + filename;

            byte[] bytes = Convert.FromBase64String(data);
            FileStream file = File.Create(Path.Combine(_directory, filename));
            foreach (byte chunk in bytes)
            {
                file.WriteByte(chunk);
            }
            file.Close();
            return Task.FromResult(filename);
        }

        public void CreateDirectoryIfNotExist()
        {
            if (!Directory.Exists(_directory))
            {
                Directory.CreateDirectory(_directory);
            }
        }
    }
}