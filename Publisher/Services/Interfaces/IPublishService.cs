using System.Threading.Tasks;

namespace Publisher.Services.Interfaces
{
    public interface IPublishService
    {
        Task<bool> UploadToYoutube(string filePath);
    }
}
