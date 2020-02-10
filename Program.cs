using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Dealer_Programs_Uploads
{
    class Program
    {
        private static Dictionary<string, string> colDealerSettings = new Dictionary<string, string>();
        private static string dealerProgramGroup;        
        private static DealerProgramDataFiles objDP;

        // Job logging metrics
        private static DateTime jobStart;
        private static DateTime jobCompleted;               
        private static int queryRuntime; // seconds
        private static int jobRuntime;   // seconds        
        private static int recordsCreated;
        private static string fileNameSent;
        private static string jobCompletedStatus;
        private static string jobMessages;
        
        static void Main(string[] args)
        {
            bool validSwitch = false;            
            bool testMode = true; // ********** Flip to false for prod *******************************
            
            foreach (string arg in args)
            {
                jobMessages = "";
                jobStart = System.DateTime.Now;
                Console.WriteLine("Starting " + arg.ToUpper() + " at " + jobStart);
                objDP = new DealerProgramDataFiles();
                dealerProgramGroup = arg.ToUpper();
                GetDealerAppSettings(dealerProgramGroup);
                objDP.ConnectionString = CONNECTIONSTRING_GBSQL01v2;
                validSwitch = false;
                                
                if(testMode)  // *********************************************************************
                    objDP.OutputFilePath = @"C:\temp\" + arg + @"\";                    
                else
                    objDP.OutputFilePath = colDealerSettings["LOCALPATH_OUTBOUND"];
                                                
                switch (arg.ToUpper())
                {
                    case "GOODYEAR":
                        {
                            validSwitch = true;                            
                            objDP.OutputFileName = "Goodyear_Inventory_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".txt";                                                 
                            objDP.CreateGoodyearFile();
                            break;
                        }
                    case "PIRELLI":
                        {
                            validSwitch = true;                            
                            objDP.OutputFileName = "SO_7091762_" + DateTime.Now.ToString("yyyyMMdd") + ".csv";   // added hhmm
                            objDP.CreatePirelliFile();                           
                            break;
                        }
                    case "PIRELLI_DOWNLOAD":
                        {                            
                            string m_fileNamePart = "Error_SO_7091762_" + DateTime.Now.ToString("yyyyMMdd");
                            List<string> sDloadedFiles = FTP_FileDownload(m_fileNamePart, false);
                            break;
                        }
                    case "HANKOOK":
                        {
                            validSwitch = true;                            
                            objDP.OutputFileName = "205545-" + DateTime.Now.ToString("yyyyMMdd") + "-" + DateTime.Now.ToString("hhmm") + ".csv";  
                            objDP.CreateHankookFile();
                            break;
                        }
                    case "HANKOOK_DOWNLOAD":
                        {
                            string m_fileNamePart = "Error_205545-2019";
                            List<string> sDloadedFiles = FTP_FileDownload(m_fileNamePart, false);
                            break;
                        }
                    case "BFSSELLOUT":
                        {
                            validSwitch = true;
                            objDP.OutputFileName = "309354_" + DateTime.Now.ToString("yyyyMMdd") + "-" + DateTime.Now.ToString("hhmm") + ".csv";   // added hhmm     
                            objDP.CreateBFSSelloutFile();
                            break;
                        }
                    case "BFSTIER1":
                        {
                            validSwitch = true;
                            objDP.OutputFileName = "309354_" + DateTime.Now.ToString("yyyyMMdd") + "-" + DateTime.Now.ToString("hhmm") + ".csv";   // added hhmm            
                            objDP.CreateBFSTier1File();
                            break;
                        }
                    case "GFK":
                        {
                            validSwitch = true;
                            objDP.OutputFileName = GFK_FileNameNow();
                            objDP.CreateGFKFile();
                            break;
                        }
                }

                if (validSwitch)
                {
                    queryRuntime = objDP.QueryRunTimeSeconds;
                    recordsCreated = objDP.RowsCreated;

                    if (objDP.IsError)
                    {
                        jobCompletedStatus = "File Creation - Error Occured";
                        jobMessages = objDP.ErrorMessage;
                    }
                    else
                    {
                        jobCompletedStatus = "File Creation - No Errors";
                        Console.WriteLine("File Created: " + objDP.OutputFileName);
                    }

                    Console.WriteLine("Query/File Completed at " + DateTime.Now);                    
                    Console.WriteLine(recordsCreated.ToString() + " Records Created.");

                    if (recordsCreated == 0)
                        Console.WriteLine("Skipping FTP");
                    else
                    {
                        if (objDP.IsError)
                            Console.WriteLine("Skipping FTP");
                        else
                        {
                            Console.WriteLine("Executing FTP...");
                            FTP_FileUpload();
                        }
                    }

                    jobCompleted = System.DateTime.Now;
                    jobRuntime = jobCompleted.Subtract(jobStart).Seconds;

                    jobRuntime = (DateTime.Now.Subtract(jobStart).Minutes * 60) + DateTime.Now.Subtract(jobStart).Seconds;

                    Console.WriteLine("Writing to Job Activity Log...");
                    string updateKey = UpdateJobLog();

                    Console.WriteLine("Writing File Contents to Database...");
                    saveDataFile(arg.ToUpper(), updateKey, DateTime.Now.ToString());

                    Console.WriteLine("Query Run Time: " + queryRuntime.ToString() + " Seconds");
                    Console.WriteLine("Total Job Run Time: " + jobRuntime.ToString() + " Seconds");
                    Console.WriteLine("Job Completed At " + DateTime.Now);
                    Console.WriteLine(jobCompletedStatus);
                    Console.WriteLine(jobMessages);
                    Console.WriteLine("-----------------------------------------------------");
                }
                else
                {
                    Console.WriteLine("Unrecognized Commandline Parameter: " + arg);
                    Console.WriteLine("-----------------------------------------------------");
                }
            }

            Console.WriteLine("All Tasks Completed.");

            if (testMode)
            {
                Console.WriteLine("Press Any Key...");
                Console.ReadKey();
            }
        }

        private static string GFK_FileNameNow()
        {
            StringBuilder newFileName = new StringBuilder("");
            DateTime st = DateTime.Now.Subtract(new TimeSpan(7, 0, 0, 0, 0));
            DateTime en = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0, 0));
            newFileName.Append(st.Year.ToString() + "-");
            newFileName.Append(st.Month.ToString().PadLeft(2, '0') + "-");
            newFileName.Append(st.Day.ToString().PadLeft(2, '0') + "-");
            newFileName.Append(en.Year.ToString() + "-");
            newFileName.Append(en.Month.ToString().PadLeft(2, '0') + "-");
            newFileName.Append(en.Day.ToString().PadLeft(2, '0') + ".txt");
            return newFileName.ToString();
        }

        private static void saveDataFile(string dealerProgramGroup, string jobLogKey, string fileDateStamp)
        {
            String sSql = "EXEC Dealer_Programs.dbo.up_DealerPrograms_DataFilesInsert ";
            sSql += "@FK_JobActivityLogID = " + jobLogKey + ", ";
            sSql += "@DealerProgramGroup = '" + dealerProgramGroup + "', ";
            sSql += "@fileCreatedDateTime = '" + fileDateStamp + "'";

            DataAccess objDA = new DataAccess();
            StringBuilder sb = new StringBuilder();

            foreach(DataRow dr in objDP.DataFileOutput.Rows)
            {
                try
                {
                    sb.Clear();
                    for (int iX = 1; iX < objDP.DataFileOutput.Columns.Count; iX++)
                    {
                        sb.Append(",");
                        sb.Append("@DataCol_" + iX.ToString() + " ='" + dr[iX].ToString().Trim() + "'");
                    }
                    objDA.ExecNonQuery(CONNECTIONSTRING_GBSQL01v2, sSql + sb.ToString());
                }
                catch(Exception ex)
                { }
            }
        }
        
        private static List<string> FTP_FileDownload(string remoteFileName, bool matchExact)
        {
            FTP_POMPS objFtp = new FTP_POMPS();
            objFtp.RemoteServer = colDealerSettings["FTP_SERVER"].ToString();
            objFtp.RemoteFilePath = colDealerSettings["FTP_SUBFOLDER"].ToString();
            objFtp.RemoteUserID = colDealerSettings["FTP_USERID"].ToString();
            objFtp.RemotePassword = colDealerSettings["FTP_PASSWORD"].ToString();

            List<string> sFiles = objFtp.ListDirectory_SFTP();  // Get file names in remote directory
            List<string> sFilesToDownload = new List<string>();

            // Now that we have collection of remote file names, search through the error files for names that contain yesterdays date/time stamp
            foreach (string sFl in sFiles)
            {
                if (sFl.StartsWith(remoteFileName))
                {
                    // Add matches to another collection 
                    sFilesToDownload.Add(sFl);
                }
            }

            // Download each file that matched previous search
            foreach (string sFl in sFilesToDownload)
            {
                if (!objFtp.DownloadFile_SFTP(sFl, objDP.OutputFilePath))
                {
                    Console.WriteLine("Error Trying Downloading " + objFtp.ErrorMessage);                    
                    //sFilesToDownload.Remove(sFl);
                }
                else
                    Console.WriteLine("Downloaded File: " + sFl);
            }
            return sFilesToDownload;
        }

        private static void FTP_FileUpload()
        {
            FTP_POMPS objFtp = new FTP_POMPS();
            objFtp.RemoteServer = colDealerSettings["FTP_SERVER"].ToString();
            objFtp.RemoteFilePath = colDealerSettings["FTP_SUBFOLDER"].ToString();
            objFtp.RemoteUserID = colDealerSettings["FTP_USERID"].ToString();     
            objFtp.RemotePassword = colDealerSettings["FTP_PASSWORD"].ToString(); 
            objFtp.SourceFileName = objDP.OutputFileName;
            objFtp.SourceFilePath = objDP.OutputFilePath;
            objFtp.FTPENABLED = bool.Parse(colDealerSettings["FTP_ENABLED"].ToString());

            fileNameSent = objDP.OutputFileName;
            bool bRslt = false;

            if (bool.Parse(colDealerSettings["USE_SFTP"].ToString()))
                bRslt = objFtp.UploadFile_SFTP();
            else
                bRslt = objFtp.UploadFile_FTP();

            if (objFtp.FTPENABLED == false)
            {
                jobCompletedStatus += ";FTP Enabled Flag = False";

                if (bRslt)
                    jobCompletedStatus += ";No FTP Errors";
                else
                    jobCompletedStatus += ";FTP Error Occured";

                jobMessages += objFtp.ErrorMessage;
            }
            else
            {
                if (bRslt)
                {
                    jobCompletedStatus += "; FTP - No Errors";
                    jobMessages += objFtp.ErrorMessage;
                }
                else
                {
                    jobCompletedStatus += "; FTP - Error Occured";
                    jobMessages += objFtp.ErrorMessage;
                    fileNameSent += " - Status Unknown";
                }
            }
        }

        private static string CONNECTIONSTRING_GBSQL01v2
        { get => System.Configuration.ConfigurationManager.AppSettings["GBSQL01_v2"]; }

        private static void GetDealerAppSettings(string dealerProgramGroup)
        {
            DataAccess objDA = new DataAccess();
            DataTable dt = new DataTable();
            String sSql = "EXEC Dealer_Programs.dbo.up_DealerPrograms_GetAppSettings @DEALERPROGRAMGROUP = '" + dealerProgramGroup + "' ";   
            dt = objDA.GetDataTable(CONNECTIONSTRING_GBSQL01v2, sSql);
            if(dt.Rows.Count > 0)
            {
                colDealerSettings.Clear();
                DataRow dr = dt.Rows[0];
                string sKey = "";
                string sVal = "";
                foreach(DataColumn dc in dt.Columns)
                {
                    sKey = dc.ColumnName;
                    sVal = dr[sKey].ToString();
                    colDealerSettings.Add(sKey, sVal);
                }
            }
        }

        private static string UpdateJobLog()
        {
            StringBuilder sb = new StringBuilder("EXEC Dealer_Programs.dbo.up_DealerPrograms_JobsActivityLog_Update ");
            sb.Append("@DealerProgramGroup = '" + dealerProgramGroup + "', ");            
            sb.Append("@JobStartDateTime = '" + jobStart + "', ");
            sb.Append("@JobCompletedDateTime = '" + jobCompleted + "', ");
            sb.Append("@QueryRuntimeSeconds = " + queryRuntime.ToString() + ", ");
            sb.Append("@JobRuntimeSeconds = " + jobRuntime.ToString() + ", ");
            sb.Append("@RecordsCreated = " + recordsCreated.ToString() + ", ");
            sb.Append("@FileNameSent = '" + fileNameSent + "', ");
            sb.Append("@JobCompletedStatus = '" + jobCompletedStatus + "', ");
            sb.Append("@JobMessages = '" + jobMessages.Replace("'"," ") + "' ");
            DataAccess objDA = new DataAccess();
            object rtnVal = new object();
            rtnVal = objDA.GetScalarValue(CONNECTIONSTRING_GBSQL01v2, sb.ToString());

            if (objDA.IsError)
                Console.WriteLine("Error Writting To Job Activity Log: " + objDA.ErrorMessage);

            return rtnVal.ToString();
        }
    }
}
