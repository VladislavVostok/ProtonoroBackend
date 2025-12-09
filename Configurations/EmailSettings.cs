namespace ProtonoroBackend.Configurations
{
    public class EmailSettings
    {
        public string SMTPServer { get; set; }
        public int Port { get; set; }
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool EnableSSL { get; set; }
    }
}
