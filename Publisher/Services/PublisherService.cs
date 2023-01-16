using System;
using System.IO;
using static System.IO.Path;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.YouTube.v3.Data;
using Google.Apis.YouTube.v3;
using Publisher.Services.Interfaces;
using Google.Apis.Upload;

namespace Publisher.Services
{
    public class PublisherService : IPublishService
    {

        public async Task UploadToYoutube(string filePath)
        {
            UserCredential credential;
            var credentialFile = Combine(GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Credentials", "client_secrets.json");
            //using (var stream = new FileStream(credentialFile, FileMode.Open, FileAccess.Read))
            //{
            //credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
            //    GoogleClientSecrets.Load(stream).Secrets,
            //    // This OAuth 2.0 access scope allows an application to upload files to the
            //    // authenticated user's YouTube channel, but doesn't allow other types of access.
            //    new[] { YouTubeService.Scope.YoutubeUpload },
            //    "user",
            //    CancellationToken.None
            //);
            //}
            credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromFile(credentialFile).Secrets,
                    new[] {YouTubeService.Scope.YoutubeUpload},
                    "user",
                    CancellationToken.None
                );

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = Assembly.GetExecutingAssembly().GetName().Name
            });

            var video = new Video();
            video.Snippet = new VideoSnippet();
            video.Snippet.Title = "Default Video Title";
            video.Snippet.Description = "Default Video Description";
            video.Snippet.Tags = new string[] { "tag1", "tag2" };
            video.Snippet.CategoryId = "28"; // Science and Technology
            video.Status = new VideoStatus();
            video.Status.PrivacyStatus = "unlisted"; // or "private" or "public"

            using (var fileStream = new FileStream(filePath, FileMode.Open))
            {
                var videosInsertRequest = youtubeService.Videos.Insert(video, "snippet,status", fileStream, "video/*");
                videosInsertRequest.ProgressChanged += videosInsertRequest_ProgressChanged;
                videosInsertRequest.ResponseReceived += videosInsertRequest_ResponseReceived;

                await videosInsertRequest.UploadAsync();
            }
        }

        void videosInsertRequest_ProgressChanged(Google.Apis.Upload.IUploadProgress progress)
        {
            switch (progress.Status)
            {
                case UploadStatus.Uploading:
                    Console.WriteLine("{0} bytes sent.", progress.BytesSent);
                    break;

                case UploadStatus.Failed:
                    Console.WriteLine("An error prevented the upload from completing.\n{0}", progress.Exception);
                    break;
            }
        }

        void videosInsertRequest_ResponseReceived(Video video)
        {
            Console.WriteLine("Video id '{0}' was successfully uploaded.", video.Id);
        }
    }
}
