using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using Renci.SshNet;

namespace Dealer_Programs_Uploads
{
    class FTP_POMPS
    {
        private string m_remoteServer;
        private int m_port;
        private string m_remoteUserID;
        private string m_remotePassword;
        private string m_remoteFilePath;
        private string m_remoteFileName;
        private string m_sourceFilePath;
        private string m_sourceFileName;
        private string m_statusResponse;
        private bool m_ftpEnabled;
        private bool m_isError;
        private string m_errorMessage;

        public FTP_POMPS()
        {
            m_remoteServer = "";
            m_port = 0;
            m_remoteUserID = "";
            m_remotePassword = "";
            m_remoteFileName = "";
            m_remoteFilePath = "";
            m_sourceFileName = "";
            m_sourceFilePath = "";
            m_statusResponse = "";
            m_ftpEnabled = false;
            ClearError();
        }

        public List<string> ListDirectory_SFTP()
        {

            int port = 22;  //default 
            if (m_port > 0)
                port = m_port;

            List<string> sFiles = new List<string>();

            try
            {

                using (SftpClient client = new SftpClient(m_remoteServer, port, m_remoteUserID, m_remotePassword))
                {
                    client.Connect();
                    var files = client.ListDirectory(m_remoteFilePath);
                    foreach (var file in files)
                    {
                        sFiles.Add(file.Name.ToString());
                    }
                    client.Disconnect();
                }
            }
            catch(Exception ex)
            { SetError(ex.Message); }

            return sFiles;
        }

        public bool DownloadFile_SFTP(string remoteFileName, string localPath)
        {
            int port = 22;  //default 
            if (m_port > 0)
                port = m_port;

            m_remoteFileName = remoteFileName;
            string localFileName = PathWack(localPath) + remoteFileName;

            try
            {
                using (var client = new SftpClient(m_remoteServer, port, m_remoteUserID, m_remotePassword))
                {
                    client.Connect();
                    using (var file = File.OpenWrite(localFileName))
                    {
                        if(m_remoteFilePath.Length > 0)
                            client.ChangeDirectory(m_remoteFilePath);
                        
                        client.DownloadFile(m_remoteFileName, file);
                    }
                    client.Disconnect();
                }
            }
            catch(Exception ex)
            { SetError(ex.Message); }

            return !m_isError;
        }

        public bool UploadFile_SFTP()
        {
            if (FTPENABLED == false)
            {
                m_statusResponse = "FTP_ENABLED Flag Is Set To False";
                return !IsError;
            }

            int port = 22;  //default 
            if (m_port > 0)
                port = m_port;

            try
            {                
                using (SftpClient client = new SftpClient(m_remoteServer, port, m_remoteUserID, m_remotePassword))
                {
                    client.Connect();

                    if (m_remoteFilePath.Length > 0)
                        client.ChangeDirectory(m_remoteFilePath);

                    using (FileStream fs = new FileStream(PathWack(m_sourceFilePath) + m_sourceFileName, FileMode.Open))
                    {
                        client.BufferSize = 4 * 1024;
                        client.UploadFile(fs, m_sourceFileName);
                    }
                }
            }
            catch (Exception ex)
            { SetError(ex.Message); }
            finally
            {
                if (!IsError)
                { m_statusResponse = "Completed With No Errors"; }
            }
            return !IsError;
        }
        public bool UploadFile_FTP()
        {
            //  For NON-sftp sites
            //  returns true if successful, false on error
            //  NOTE: The FTP subfolder path is CASE SENSITIVE. If the file is not dropping into the target
            //   folder, check the folder name string.
            //
            if(FTPENABLED == false)
            {
                m_statusResponse = "FTP_ENABLED Flag Is Set To False";
                return !IsError;
            }
            
            int port = 21;  //default 
            if (m_port > 0)
                port = m_port;

            try
            {
                Uri ur;
                ur = new Uri(m_remoteServer);

                // If a subfolder is not used, then add the forward slash between the URI and file name
                if (m_remoteFilePath.Length == 0)
                    m_remoteFilePath = "/";

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ur + m_remoteFilePath + m_sourceFileName);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(m_remoteUserID, m_remotePassword);
                request.Proxy = null;

                byte[] fileContents;
                using (StreamReader sourceStream = new StreamReader(PathWack(m_sourceFilePath) + m_sourceFileName))
                {
                    fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
                }

                request.ContentLength = fileContents.Length;

                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(fileContents, 0, fileContents.Length);
                }

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    m_statusResponse = response.StatusDescription;
                }
            }
            catch (Exception ex)
            { SetError(ex.Message); }
            return !IsError;
        }

        private string PathWack(string SomePath)
        {
            if (SomePath.EndsWith("\\") == false)
                SomePath += "\\";
            return SomePath;
        }

        private void SetError(string msg)
        {
            m_isError = true;

            if (m_errorMessage.Length > 0)
                m_errorMessage += ";";

            m_errorMessage += msg;
        }

        private void ClearError()
        {
            m_isError = false;
            m_errorMessage = "";
        }

        public string RemoteServer { get => m_remoteServer; set => m_remoteServer = value; }
        public string RemoteUserID { get => m_remoteUserID; set => m_remoteUserID = value; }
        public string RemotePassword { get => m_remotePassword; set => m_remotePassword = value; }
        public string RemoteFilePath { get => m_remoteFilePath; set => m_remoteFilePath = value; }
        public string RemoteFileName { get => m_remoteFileName; set => m_remoteFileName = value; }
        public string SourceFilePath { get => m_sourceFilePath; set => m_sourceFilePath = value; }
        public string SourceFileName { get => m_sourceFileName; set => m_sourceFileName = value; }
        public bool FTPENABLED { get => m_ftpEnabled; set => m_ftpEnabled = value; }
        public string StatusResponse { get => m_statusResponse; }
        public int Port { get => m_port; set => m_port = value; }
        public bool IsError { get => m_isError; }
        public string ErrorMessage { get => m_errorMessage; }
    }
}
