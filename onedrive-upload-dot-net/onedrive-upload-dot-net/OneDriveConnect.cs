using Azure.Identity;
using Microsoft.Graph;
namespace onedrive_upload_dot_net
{
    public class OneDriveConnect :IOneDriveConnect
    {
        #region Fields
        private readonly GraphServiceClient _GraphClinet;
        private readonly string UserName;
        #endregion

        #region Constructor
        public OneDriveConnect(string TennantId,string ClientId ,string ClientSecret,string Username)
        {
            var scopes = new[] { "https://graph.microsoft.com/.default" };
            var options = new TokenCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
            };  
            this.UserName = Username;
            var credentials = new ClientSecretCredential(TennantId, ClientId, ClientSecret, options);
            _GraphClinet = new GraphServiceClient(credentials, scopes);
            
        }
        #endregion

        #region Methord
        public async Task<string> getUploadFileUrl(string filePath,string UploadFolderName)
        {
            try
            {


               
               
                var filename = Path.GetFileName(filePath);
                var key = $"{UploadFolderName}/{filename}";


                /* var fileName = "test.txt";*/
                /*    var combine = Path.Combine(filePath, filename);*/
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException("This file was not found.");

                }
                FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);
                //Accessing Drive using Graphclient
                var drive = await _GraphClinet.Users[UserName].Drive.GetAsync();
                if (drive.Id == null)
                {
                    throw new Exception("Drive not found");
                }
                //Uploading File to One Drive 
                await _GraphClinet.Drives[drive.Id].Root.ItemWithPath(key).Content.PutAsync(file);
                //Making File Editable and For accessible to anyone that is uploaded
                Microsoft.Graph.Drives.Item.Items.Item.CreateLink.CreateLinkPostRequestBody body = new()
                {
                    Type = "edit",
                    Scope = "anonymous",

                };
                //creating shareable link of the file uploaded
                var result = await _GraphClinet.Drives[drive.Id].Root.ItemWithPath(key).CreateLink.PostAsync(body);
                return result.Link.WebUrl;
            }
            catch 
            {
                throw;
            }
        }
        #endregion
    }
}