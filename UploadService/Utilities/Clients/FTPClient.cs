using System;
using System.IO;
using System.Net;

namespace UploadService.Utilities.Clients
{
    public class FTPClient : IServerClient
    {
        private string host = null;
        private string user = null;
        private string pass = null;
        private FtpWebRequest ftpRequest = null;
        private FtpWebRequest ftpRequestDelete = null;
        private FtpWebResponse ftpResponse = null;
        private FtpWebResponse ftpResponseDelete = null;
        private Stream ftpStream = null;
        private int bufferSize = 2048;
        
        /* Construct Object */
        public FTPClient(string hostIP, string userName, string password)
        {
            host = hostIP;
            user = userName;
            pass = password;
        }
        
        /* Upload File */

        public void UploadFile(string remoteFile, string localFFile,bool overwrite)
        {
            try
            {
                if (overwrite)
                {
                    delete(remoteFile);
                }
                /* Create an FTP Request*/
                ftpRequest = (FtpWebRequest) FtpWebRequest.Create(host + "/"+ remoteFile);
                
                /* Log in to the FTP Server*/
                ftpRequest.Credentials = new NetworkCredential(user,pass);
                
                /* When in doubt use these options*/
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                
                /* Specify the Type of FTP request */
                ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
                
                /* Establish Return communication with the FTP server */
                ftpStream = ftpRequest.GetRequestStream();
                
                /* Open a File Stream to Read the File for Upload */
                FileStream localFileStream  = new FileStream(localFFile, FileMode.OpenOrCreate);
                
                /* Buffer for the Downloaded Data */
                byte[] byteBuffer = new byte[bufferSize];
                int bytesSent = localFileStream.Read(byteBuffer, 0, bufferSize);

              

                /* Upload the File by Sending the Buffered Data Until the Transfer is Complete */
                try
                {
                    while (bytesSent != 0)
                    {
                        ftpStream.Write(byteBuffer, 0, bytesSent);
                        bytesSent = localFileStream.Read(byteBuffer, 0, bufferSize);
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
        public void delete(string deleteFile)
        {
           
            try
            {
                /* Create an FTP Request */
                ftpRequest = (FtpWebRequest) WebRequest.Create(host + "/" + deleteFile);
                /* Log in to the FTP Server with the User Name and Password Provided */
                ftpRequest.Credentials = new NetworkCredential(user, pass);
                /* When in doubt, use these options */
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                /* Specify the Type of FTP Request */
                ftpRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                /* Establish Return Communication with the FTP Server */
                ftpResponse = (FtpWebResponse) ftpRequest.GetResponse();
                /* Resource Cleanup */
                ftpResponse.Close();
                ftpRequest = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                
                throw;
                
            }
            return;
        }

        public bool checkIfFileExists(string filePath)
        {
            bool exists = false;
            var request = (FtpWebRequest)WebRequest.Create(host + "/" + filePath);
            request.Credentials = new NetworkCredential(user, pass);
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
        
        public bool directoryExists(string directory)
        {
            /* Create an FTP Request */
            ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host + "/" + directory);
            /* Log in to the FTP Server with the User Name and Password Provided */
            ftpRequest.Credentials = new NetworkCredential(user, pass);
            /* Specify the Type of FTP Request */
            ftpRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            try
            {
                using (FtpWebResponse response = (FtpWebResponse)ftpRequest.GetResponse())
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
                ftpRequest = null;
            }
        }
    }
}