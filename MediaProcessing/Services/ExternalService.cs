using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using System;
using System.IO;
using static System.IO.Path;
using Domain.Requests;
using System.Reflection;

namespace MediaProcessing.Services
{
    public class ExternalService
    {
        public async Task PostData()
        {
            try
            {
                var url = Environment.GetEnvironmentVariable("REMOTE_PUBLISHER_API");
                var filePath = Combine(GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Work", "output.mp4");

                using var form = new MultipartFormDataContent();
                using var fileContent = new ByteArrayContent(await File.ReadAllBytesAsync(filePath));

                form.Add(fileContent, "arquivo", Path.GetFileName(filePath));

                var httpClient = new HttpClient()
                {
                    BaseAddress = new Uri(url)
                };

                var response = await httpClient.PostAsync(url, form);
                //response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine("response :" + responseContent);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
    }
}
