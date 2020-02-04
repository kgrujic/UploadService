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

     
        public FtpClient(string hostIp, string userName, string password, int port)
        {
            _host = hostIp;
            _user = userName;
            _pass = password;
            _port = port;
        }

        /* Upload File */

        public void UploadFile(string remoteFile, string localFFile, bool overwrite)
        {
            try
            {
                if (overwrite)
                {
                    Delete(remoteFile);
                }

                /* Create an FTP Request*/
                _ftpRequest = (FtpWebRequest) FtpWebRequest.Create(_host + remoteFile);

                /* Log in to the FTP Server*/
                _ftpRequest.Credentials = new NetworkCredential(_user, _pass);

                /* When in doubt use these options*/
                _ftpRequest.UseBinary = true;
                _ftpRequest.UsePassive = true;
                _ftpRequest.KeepAlive = true;

                /* Specify the Type of FTP request */
                _ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;

                /* Establish Return communication with the FTP server */
                using (_ftpStream = _ftpRequest.GetRequestStream())
                {
                    /* Open a File Stream to Read the File for Upload */
                    using (FileStream localFileStream = new FileStream(localFFile, FileMode.OpenOrCreate))
                    {
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
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                _ftpRequest = null;
            }

            return;
        }


        private void Delete(string deleteFile)
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
                using (_ftpResponse = (FtpWebResponse) _ftpRequest.GetResponse())
                {
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                _ftpRequest = null;
            }

            return;
        }

        public bool CheckIfFileExists(string filePath)
        {
            var exists = false;
            _ftpRequest = (FtpWebRequest) WebRequest.Create(_host + filePath);
            _ftpRequest.Credentials = new NetworkCredential(_user, _pass);
            _ftpRequest.Method = WebRequestMethods.Ftp.GetFileSize;

            try
            {
                using (_ftpResponse = (FtpWebResponse) _ftpRequest.GetResponse())
                {
                    exists = true;
                }
            }
            catch (WebException ex)
            {
                using (_ftpResponse = (FtpWebResponse) ex.Response)
                {
                    if (_ftpResponse.StatusCode ==
                        FtpStatusCode.ActionNotTakenFileUnavailable)
                    {
                        //Does not exist
                    }
                }
            }
            finally
            {
                _ftpRequest = null;
            }

            return exists;
        }

        public bool DirectoryExists(string directory)
        {
            /* Create an FTP Request */
            _ftpRequest = (FtpWebRequest) FtpWebRequest.Create(_host + directory);
            /* Log in to the FTP Server with the User Name and Password Provided */
            _ftpRequest.Credentials = new NetworkCredential(_user, _pass);
            /* Specify the Type of FTP Request */
            _ftpRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            try
            {
                using (_ftpResponse = (FtpWebResponse) _ftpRequest.GetResponse())
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