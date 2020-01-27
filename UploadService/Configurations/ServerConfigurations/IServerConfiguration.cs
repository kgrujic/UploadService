namespace UploadService.Configurations.ServerConfigurations
{
    public interface IServerConfiguration
    {
        public string HostAddress { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int PortNumber { get; set; }
        public int MAX_FTP_SESSIONS { get; set; }
        public int IDLE_SESSION_TIMEOUT { get; set; }
        
   
        
    }
}