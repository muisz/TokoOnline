namespace TokoOnline.Services
{
    public class LocalStorage : IFileStorage
    {
        private readonly string _directory;
        private readonly IHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        public LocalStorage(IHostEnvironment environment, IConfiguration configuration)
        {
            _environment = environment;
            _configuration = configuration;
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

        public Task<string> GetAccessibleUrl(string file)
        {
            string url = _configuration["Urls"] ?? "";
            return Task.FromResult($"{url}/media/{file}");
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