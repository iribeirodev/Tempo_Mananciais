using System.Threading.Tasks;
using Publisher.Services.Interfaces;

namespace Publisher.Services
{
    public class PublisherService : IPublishService
    {
        public Task<bool> UploadToYoutube(string filePath)
        {
            return Task.FromResult(true);
        }
    }
}
