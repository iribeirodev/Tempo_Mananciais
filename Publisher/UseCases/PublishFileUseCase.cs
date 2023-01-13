using static System.IO.File;
using static System.IO.Path;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Infrastructure.Response;
using Publisher.Services.Interfaces;

namespace Publisher.UseCases
{
    public class PublishFileUseCase
    {
        private readonly IPublishService _publishService;
        private readonly string _basePath = GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public PublishFileUseCase(IPublishService publishService) => _publishService = publishService;

        public async Task<ProcessResponse> Process(IFormFile fileName)
        {
            var filePath = Combine(_basePath, "Uploaded", fileName.FileName);

            Delete(filePath);
            using (var fileStream = Create(filePath))
            {
                await fileName.CopyToAsync(fileStream);
                fileStream.Flush();
            }
            
            await _publishService.UploadToYoutube(filePath);

            return new ProcessResponse
            {
                Message = "Arquivo recebido e enviado para streaming."
            };
        }

    }
}
