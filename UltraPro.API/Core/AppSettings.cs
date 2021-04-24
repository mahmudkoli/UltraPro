using System;

namespace UltraPro.API.Core
{
    public class AppSettings
    {
        public string TokenSecretKey { get; set; }
        public int TokenExpiresHours { get; set; }
        public string AppHeaderSecretKey { get; set; }
        public string EncryptionDecryptionKey { get; set; }
        public string UserDefaultPassword { get; set; }
        public string ResetPasswordUrl { get; set; }
    }
}