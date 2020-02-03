using System;
using System.IO;
using System.Net;

namespace UploadService.Utilities.Clients
{
    public class FtpClient : IServerClient
    {
        private string _host = null;
        private string _user = null;
        private string _pass = null;
        private int _port = 0;
        private FtpWebRequest _ftpRequest = null;
        private FtpWebRequest _ftpRequestDelete = null;
        private FtpWebResponse _ftpResponse = null;
        private FtpWebResponse _ftpResponseDelete = null;
        private Stream _ftpStream = null;
        private int _bufferSize = 2048;
        
        /* Construct Object */
        public FtpClient(string hostIP, string userName, string password, int port)
        {
            _host = hostIP;
            _user = userName;
            _pass = password;
            _port = port;
        }
        
        /* Upload File */

        public void UploadFile(string remoteFile, string localFFile,bool overwrite)
        {
            try
            {
                if (overwrite)
                {
                    Delete(remoteFile);
                }

    
                /* Create an FTP Request*/
                _ftpRequest = (FtpWebRequest) FtpWebRequest.Create(_host+remoteFile);
                
                /* Log in to the FTP Server*/
                _ftpRequest.Credentials = new NetworkCredential(_user,_pass);
                
                /* When in doubt use these options*/
                _ftpRequest.UseBinary = true;
                _ftpRequest.UsePassive = true;
                _ftpRequest.KeepAlive = true;
                
                /* Specify the Type of FTP request */
                _ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
                
                /* Establish Return communication with the FTP server */
                _ftpStream = _ftpRequest.GetRequestStream();
                
                /* Open a File Stream to Read the File for Upload */
                FileStream localFileStream  = new FileStream(localFFile, FileMode.OpenOrCreate);
                
                /* Buffer for the Downloaded Data */
                byte[] byteBuffer = new byte[_bufferSize];
                int bytesSent = localFileStream.Read(byteBuffer, 0, _bufferSize);

              

                /* Upload the File by Sending the Buffered Data Until the Transfer is Complete */
                try
                {
                    while (bytesSent != 0)
                    {
                        _ftpStream.Write(byteBuffer, 0, bytesSent);
                        bytesSent = localFileStream.Read(byteBuffer, 0, _bufferSize);
                    }
                    //Console.WriteLine(localFFile);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    
                }
                
                /* Resource CleanUp */

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                
            }
            return;
            
        }

       

        public void Delete(string deleteFile)
        {
           
            try
            {
                /* Create an FTP Request */
                _ftpRequest = (FtpWebRequest) WebRequest.Create(_host + deleteFile);
                /* Log in to the FTP Server with the User Name and Password Provided */
                _ftpRequest.Credentials = new NetworkCredential(_user, _pass);
                /* When in doubt, use these options */
                _ftpRequest.UseBinary = true;
                _ftpRequest.UsePassive = true;
                _ftpRequest.KeepAlive = true;
                /* Specify the Type of FTP Request */
                _ftpRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                /* Establish Return Communication with the FTP Server */
                _ftpResponse = (FtpWebResponse) _ftpRequest.GetResponse();
                /* Resource Cleanup */
                _ftpResponse.Close();
                _ftpRequest = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                
                throw;
                
            }
            return;
        }

        public bool CheckIfFileExists(string filePath)
        {
            bool exists = false;
            var request = (FtpWebRequest)WebRequest.Create(_host + filePath);
            request.Credentials = new NetworkCredential(_user, _pass);
            request.Method = WebRequestMethods.Ftp.GetFileSize;
            
            try
            {
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                exists = true;
            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode ==
                    FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    //Does not exist
                }
            }

            return exists;
        }
        
        public bool DirectoryExists(string directory)
        {
            /* Create an FTP Request */
            _ftpRequest = (FtpWebRequest)FtpWebRequest.Create(_host  + directory);
            /* Log in to the FTP Server with the User Name and Password Provided */
            _ftpRequest.Credentials = new NetworkCredential(_user, _pass);
            /* Specify the Type of FTP Request */
            _ftpRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            try
            {
                using (FtpWebResponse response = (FtpWebResponse)_ftpRequest.GetResponse())
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            /* Resource Cleanup */
            finally
            {
                _ftpRequest = null;
            }
        }
    }
}