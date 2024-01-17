# OneDriveUpload v1.0.1

![build](https://img.shields.io/badge/build-passing-d.svg)
![nuget](https://img.shields.io/badge/nuget-v1.0.1-blue.svg)
Onedrive-upload-dot-net helps You To Upload a file to One Drive Using Your Tennant ID , Client Id , ClientSecret , UserName has a clean and structured Api and supports .Net6

# Using Microsoft Graph and Azure Identity

It Uses Microsoft Graph and Azure Identity package to Connect to Microsoft One Drive Throuh ClientSecretCredential including all that is mentioned above.

## Getting Started

To begin, developers need to set up authentication using the `ClientSecretCredential` class. This involves creating an instance of this class with essential parameters such as `tennatId`, `ClientId`, `ClientSecret`, and `TokenCredential`. The `tennatId`, `ClientId`, and `ClientSecret` are unique identifiers associated with the `Azure AD or Microsoft Entra application`.

```c#
var clientCredential = new ClientSecretCredential(
    tenantId,
    clientId,
    clientSecret,
    options
);
```

The `TokenCredentialOptions` class is crucial for configuring the token acquisition process. Developers can fine-tune authentication by setting parameters within this class. The whole process is facilitated by the versatile `Microsoft.Azure.Identity` namespace.

### GraphClient for Microsoft Graph API

The cornerstone of `OneDriveUpload` is the `GraphClient` class. This class abstracts away the complexities of interacting with `Microsoft Graph APIs`, providing a simplified and efficient interface. Developers can use this class to perform various operations such as uploading files to `OneDrive`, accessing user information, and much more.

```c#
var graphClient = new GraphClient(clientCredential);
```

### Comprehensive Authentication with Azure Identity

`OneDriveUpload` relies on the robust authentication capabilities provided by `Azure Identity`. The ClientSecretCredential is a part of `Azure Identity` and ensures secure and authorized access to `Microsoft Graph APIs`.

```c#
var options = new TokenCredentialOptions
{
AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
};
```

### Implementing

Configure and instantiate the `GraphClient` with the appropriate `ClientSecretCredential`:

```c#
var clientCredential = new ClientSecretCredential(
    tenantId,
    clientId,
    clientSecret,
    options
);

var graphClient = new GraphClient(clientCredential);
```

After Graphclient is created using this to upload to one drive using `onedrive_upload_dot_net;` namespace into your controller and intializing

```c#
OneDriveConnect connect = new OneDriveConnect
(_configuration["GraphSetting:TennantId"],
_configuration["GraphSetting:ClientId"],
_configuration["GraphSetting:ClientSecret"],
_configuration["GraphSetting:UserName"]);
```

and using `OneDriveConnect` class to use methord `getUploadFileUrl` to get uploaded file url.To your one drive by specifing `filePath` and `UplaodFolderName` to `getUploadFileUrl` methord.

```c#
var url = await connect.getUploadFileUrl(@"E:\\test.txt",_configuration["GraphSetting:folderName"]);
```

### Sample Code

```c#
 public class OneDriveConnect :IOneDriveConnect
    {

        private readonly GraphServiceClient _GraphClinet;
        private readonly string UserName;
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
    }
```

# Create an Microsoft Entra App - Step-by-Step Guide

## Step 1: Microsoft Entra Portal

- Log in to the [Microsoft Entra Portal](https://entra.microsoft.com/) with your Microsoft account.

## Step 2: Navigate to Identity

- In the left-hand menu, click on "Identity."

## Step 3: Register an Application

1. Inside Identity, click on "App registrations."
2. Click on "New registration."
3. Fill in the required details:
   - **Name:** Give your app a name.
   - **Supported account types:** Choose the appropriate option based on your requirements.
4. Click "Register."
   ![App Registraion](/onedrive-upload-dot-net/Screenshot/Appregistraion.jpg)

## Step 4: Note Application (Client) ID and Directory (Tenant) ID

- After registration, note down the "Application (client) ID" and "Directory (tenant) ID" from the app overview page.
  ![App overview](/onedrive-upload-dot-net/Screenshot/App%20overview.jpg)

## Step 5: Create a Client Secret

1. In the left-hand menu, go to `Certificates & secrets`.
2. Under `Client secrets`, create a new `client secret`. Note down the secret value.
   ![ClientSecret](/onedrive-upload-dot-net/Screenshot/ClientSecret.png)

## Step 6: Configure API Permissions

1. In the left-hand menu, go to `API permissions`.
2. Click on `Add a permission`, choose the API your app needs access to, and select the required permissions.
3. Click "Grant admin consent for [your organization]" to consent to the permissions.
   ![Apipermission](/onedrive-upload-dot-net/Screenshot/Api%20permission.png)

## Step 7:Implementation of Noted Down Keys

1. Now you've got `client ID` , `Directory (tenant) ID` , `client secret`.
2. Now use these keys and `UserEmail` for uploading in onedrive.
