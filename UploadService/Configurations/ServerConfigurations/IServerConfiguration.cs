namespace UploadService.Configurations.ServerConfigurations
{
    public interface IServerConfiguration
    {
        public string HostAddress { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int PortNumber { get; set; }
        public int MaxServerSessions { get; set; }
        public int IdleSessionTimeout { get; set; }
        
   
        
    }
}