using System;
using System.IO;
using System.Net;
using Microsoft.Extensions.Logging;

namespace UploadService.Utilities.Clients
{
    /// <summary>
    /// FtpClient class implements methods for communication with FTP server
    /// Implements IServerClient interface
    /// </summary>
    public class FtpClient : IServerClient
    {
        private string _host;
        private string _user;
        private string _pass;
        private int _port;
        private FtpWebRequest _ftpRequest;
        private FtpWebResponse _ftpResponse;
        private Stream _ftpStream;
        private int _bufferSize = 2048;
        private ILogger<Worker> _logger;


        public FtpClient(string hostIp, string userName, string password, int port, ILogger<Worker> logger)
        {
            _host = hostIp;
            _user = userName;
            _pass = password;
            _port = port;
            _logger = logger;
        }

        /// <summary>
        /// UploadFile method uploads file from local file system to remote FTP server
        /// </summary>
        /// <param name="remoteFile">string</param>
        /// <param name="localFFile">string</param>
        /// <param name="overwrite">bool</param>
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
                            
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex.ToString());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
            finally
            {
                _ftpRequest = null;
            }

            return;
        }

        /// <summary>
        /// Delete method deletes file from remote FTP server
        /// </summary>
        /// <param name="deleteFile">string</param>
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
                _logger.LogError(ex.ToString());
            }
            finally
            {
                _ftpRequest = null;
            }

            return;
        }

        /// <summary>
        /// CheckIfFileExists method checks if file exists on remote FTP server
        /// </summary>
        /// <param name="filePath">string</param>
        /// <returns>bool</returns>
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

        /// <summary>
        /// DirectoryExists method checks if directory exists on remote FTP server
        /// </summary>
        /// <param name="directory">string</param>
        /// <returns>bool</returns>
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