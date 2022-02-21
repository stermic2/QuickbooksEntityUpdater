using System;

namespace QboProductUpdater
{
    public class Token
    {
        public string RefreshToken { get; set; }
        public string AccessToken { get; set; }
        public DateTime LastAccessTokenRefresh { get; set; }
        public DateTime LastRefreshTokenRefresh { get; set; }
    }
}