namespace UploadService.Configurations.ServerConfigurations.Implementations
{
    /// <summary>
    /// FtpServerConfiguration class
    /// Implements IServerConfiguration Interface
    /// Contains server configuration properties
    /// </summary>
    public class FtpServerConfiguration : IServerConfiguration
    {
        public FtpServerConfiguration()
        {
        }

        /// <summary>
        /// FtpServerConfiguration Constructor with arguments
        /// </summary>
        /// <param name="hostAddress"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="portNumber"></param>
        public FtpServerConfiguration(string hostAddress, string username, string password, int portNumber)
        {
            HostAddress = hostAddress;
            Username = username;
            Password = password;
            PortNumber = portNumber;
        }

        
      
        public string HostAddress { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int PortNumber { get; set; }
        public int MaxServerSessions { get; set; }
        public int IdleSessionTimeout { get; set; }
    }
}