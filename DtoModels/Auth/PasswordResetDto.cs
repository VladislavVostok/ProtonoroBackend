namespace ProtonoroBackend.DtoModels.Auth
{
    public class PasswordResetDto
    {
        public string Uuid { get; set; }
        public string OTP { get; set; }
        public string Password { get; set; }
        public string RefreshToken { get; set; }
    }
}
