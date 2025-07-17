using CloudContentSourceManagementApp.CommonHelper;
using CloudContentSourceManagementApp.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CloudContentSourceManagementApp.Services
{
    public class GoogleDriveService
    {
        private GoogleAppProfile _googleAppProfile;
        private DriveService _service;
        public GoogleDriveService (GoogleAppProfile googleAppProfile)
        {
            _googleAppProfile = googleAppProfile;
            InitDriveService();
        }

        public async Task<List<Drive>> GetSharedDrivesByNameAsync(string keyword)
        {
            var result = new List<Drive>();
            string pageToken = null;

            do
            {
                var request = _service.Drives.List();
                request.PageSize = 100;
                request.Fields = "*";
                request.Q = null;
                request.PageToken = pageToken;

                var response = await request.ExecuteAsync();

                // Lọc bằng LINQ vì Google API v3 chưa support Q cho Drives.List
                //result.AddRange(response.Drives.Where(d => d.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase)));
                var list = response.Drives.Where(d => d.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();
                result.AddRange(list);
                pageToken = response.NextPageToken;
            }
            while (!string.IsNullOrEmpty(pageToken));

            return result;
        }

        private void InitDriveService()
        {
            string decryptedKey = PasswordHelper.Decrypt(_googleAppProfile.PrivateKey);
            var googleCredential = new
            {
                type = "service_account",
                project_id = "opus-project",
                private_key_id = "0123456789abcdef",
                private_key = decryptedKey.Trim(),
                client_email = _googleAppProfile.ClientEmail,
                client_id = _googleAppProfile.ClientId,
                auth_uri = "https://accounts.google.com/o/oauth2/auth",
                token_uri = "https://oauth2.googleapis.com/token",
                auth_provider_x509_cert_url = "https://www.googleapis.com/oauth2/v1/certs",
                client_x509_cert_url = $"https://www.googleapis.com/robot/v1/metadata/x509/{_googleAppProfile.ClientEmail}"
            };
            var json = JsonSerializer.Serialize(googleCredential, new JsonSerializerOptions { WriteIndented = true });
            var credential = GoogleCredential.FromJson(json).CreateScoped(DriveService.Scope.Drive).CreateWithUser(_googleAppProfile.UserEmail);
            _service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = _googleAppProfile.ProfileName
            });
        }
    }
}