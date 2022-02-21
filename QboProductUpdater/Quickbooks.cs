using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using QuickBooksSharp;

namespace QboProductUpdater
{
    public static class Quickbooks
    {
        private static AuthenticationService _authenticationService;
        public static DataService DataService;
        private static string _clientId;
        private static string _clientSecret;
        private static Token _tokensInMemory;
        private static long _realmId;
        private static Token TokensOnDisk
        {
            get
            {
                var currentDirectory = Directory.GetCurrentDirectory().Replace(@"\bin\Debug\netcoreapp3.0", "");
                using var r = new StreamReader(currentDirectory + @"\tokens.txt");
                var json = r.ReadToEnd();
                var tokenFile = JsonConvert.DeserializeObject<Token>(json);
                return tokenFile;
            }
            set
            {
                var currentDirectory = Directory.GetCurrentDirectory().Replace(@"\bin\Debug\netcoreapp3.0", "");
                using var file = File.CreateText(currentDirectory + @"\tokens.txt");
                var serializer = new JsonSerializer();
                serializer.Serialize(file, value);
            }
        }
        public static void InitializeQuickbooksClient()
        {
            _clientId = "ABX8lGN17H2mIOuTC0GwFMmdSoMsfIIIyeWSf0znl8HFAfEyhh";
            _clientSecret = "OdnXvdIpvUaYNcWgb2owI1bWDKQbCxmBRHO38Wjk";
            _realmId = 123146221174169;
            _authenticationService = new AuthenticationService();
            _tokensInMemory = TokensOnDisk;
            DataService = new DataService(_tokensInMemory.AccessToken, _realmId, false);
        }

        public static async Task RefreshTokens()
        {
            if (_tokensInMemory.LastAccessTokenRefresh < DateTime.Now.Subtract(TimeSpan.FromMinutes(59)))
                await ForceRefresh();
        }

        public static async Task ForceRefresh()
        {
            var tokenResponse =
                await _authenticationService.RefreshOAuthTokenAsync(_clientId, _clientSecret, _tokensInMemory.RefreshToken);
            _tokensInMemory.LastAccessTokenRefresh = DateTime.Now;
            _tokensInMemory.LastRefreshTokenRefresh = _tokensInMemory.RefreshToken == tokenResponse.refresh_token
                ? DateTime.Now
                : _tokensInMemory.LastRefreshTokenRefresh;
            _tokensInMemory.AccessToken = tokenResponse.access_token;
            _tokensInMemory.RefreshToken = tokenResponse.refresh_token;
            TokensOnDisk = _tokensInMemory;
            DataService = new DataService(_tokensInMemory.AccessToken, _realmId, Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development");
        }
    }
}