namespace UploadService.Configurations.ServerConfigurations.Implementations
{
    /// <summary>
    /// 
    /// </summary>
    public class FtpServerConfiguration : IServerConfiguration
    {
        public FtpServerConfiguration()
        {
        }

        public FtpServerConfiguration(string hostAddress, string username, string password)
        {
            HostAddress = hostAddress;
            Username = username;
            Password = password;
        }

        public string HostAddress { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int PortNumber { get; set; }
        public int MaxFtpSessions { get; set; }
        public int IdleSessionTimeout { get; set; }
    }
}