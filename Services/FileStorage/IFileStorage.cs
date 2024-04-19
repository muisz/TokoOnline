namespace TokoOnline.Services
{
    public interface IFileStorage
    {
        public Task<string> UploadFromBase64(string data, string filename);
    }
}