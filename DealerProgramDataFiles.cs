using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;

namespace Dealer_Programs_Uploads
{
    class DealerProgramDataFiles
    {
        DataTable m_dtResults;
        bool m_isError;
        string m_errorMessage;
        string m_outputFilePath;
        string m_outputFileName;
        string m_connectionString;
        int m_queryRunTimeSeconds;
        int m_rowsCreated;

        public DealerProgramDataFiles()
        {
            ClearError();
        }

        public bool CreateGFKFile()
        {
            m_rowsCreated = 0;
            m_dtResults = new DataTable();
            DataAccess objDA = new DataAccess();
            DateTime dtStartMark = DateTime.Now;

            m_dtResults = objDA.GetDataTable(m_connectionString, "EXEC Dealer_Programs.dbo.up_DailyUploads_GFK");
            m_queryRunTimeSeconds = TotalSeconds(dtStartMark); 

            if (objDA.IsError)
            {
                SetError(objDA.ErrorMessage);
                return false;
            }
            else
            {
                try
                {
                    StreamWriter sw = File.CreateText(PathWack(m_outputFilePath) + m_outputFileName);
                    StringBuilder sbLine = new StringBuilder();
                    string tbDelim = "\t";

                    foreach (DataRow dr in m_dtResults.Rows)
                    {
                        sbLine.Clear();
                        sbLine.Append(dr["StoreNumber"].ToString() + tbDelim);
                        sbLine.Append(dr["InvoiceNumber"].ToString() + tbDelim);
                        sbLine.Append(dr["Week_Number"].ToString() + tbDelim);
                        sbLine.Append(dr["InvDate"].ToString() + tbDelim);
                        sbLine.Append(dr["CustomerClass"].ToString() + tbDelim);

                        sbLine.Append(dr["ProductClass"].ToString() + tbDelim);
                        sbLine.Append(dr["ProductNumber"].ToString() + tbDelim);
                        sbLine.Append(dr["ProductDescription"].ToString() + tbDelim);
                        sbLine.Append(dr["Quantity"].ToString() + tbDelim);
                        sbLine.Append(dr["Price"].ToString() + tbDelim);

                        sbLine.Append(dr["DeleteCode"].ToString() + tbDelim);
                        sbLine.Append(dr["MakeOfCar"].ToString() + tbDelim);
                        sbLine.Append(dr["ModelOfCar"].ToString() + tbDelim);
                        sbLine.Append(dr["YearOfCar"].ToString() + tbDelim);
                        sbLine.Append(dr["Mileage"].ToString() + tbDelim);

                        sbLine.Append(dr["VinNumber"].ToString() + tbDelim);
                        sbLine.Append(dr["Vin2nd8Characters"].ToString());
                 
                        sw.WriteLine(sbLine.ToString());
                        m_rowsCreated++;
                    }
                    sw.Close();
                }
                catch(Exception ex)
                { SetError(ex.Message); }
            }

            return !m_isError;
        }

        public bool CreateBFSTier1File()
        {
            m_rowsCreated = 0;
            m_dtResults = new DataTable();
            DataAccess objDA = new DataAccess();
            DateTime dtStartMark = DateTime.Now;

            m_dtResults = objDA.GetDataTable(m_connectionString, "EXEC Dealer_Programs.dbo.up_DailyUploads_BFSTier1");
            m_queryRunTimeSeconds = TotalSeconds(dtStartMark); 

            if (objDA.IsError)
            {
                SetError(objDA.ErrorMessage);
                return false;
            }
            else
            {
                try
                {
                    StreamWriter sw = File.CreateText(PathWack(m_outputFilePath) + m_outputFileName);
                    StringBuilder sbLine = new StringBuilder();

                    foreach(DataRow dr in m_dtResults.Rows)
                    {
                        sbLine.Clear();
                        sbLine.Append(dr["tihhdteinv"].ToString() + ",");
                        sbLine.Append(dr["cudealernum"].ToString() + ",");
                        sbLine.Append(dr["pdmfgprdno"].ToString() + ",");
                        sbLine.Append(dr["tihlqty"].ToString() + ",");
                        sbLine.Append(dr["tihlnuminv"].ToString());
                        sw.WriteLine(sbLine.ToString());
                        m_rowsCreated++;
                    }
                    sw.Close();
                }
                catch(Exception ex)
                { SetError(ex.Message); }
                return !IsError;
            }
        }

        public bool CreateBFSSelloutFile()
        {
            m_rowsCreated = 0;
            m_dtResults = new DataTable();
            DataAccess objDA = new DataAccess();
            DateTime dtStartMark = DateTime.Now;

            m_dtResults = objDA.GetDataTable(m_connectionString, "EXEC Dealer_Programs.dbo.up_DailyUploads_BFSSellout");
            m_queryRunTimeSeconds = TotalSeconds(dtStartMark);

            if (objDA.IsError)
            {
                SetError(objDA.ErrorMessage);
                return false;
            }
            else
            {
                try
                {
                    StreamWriter sw = File.CreateText(PathWack(m_outputFilePath) + m_outputFileName);
                    StringBuilder sbLine = new StringBuilder();
                    
                    foreach(DataRow dr in m_dtResults.Rows)
                    {
                        sbLine.Clear();
                        sbLine.Append(Convert.ToDateTime(dr["Invoice_Date"].ToString()).ToShortDateString() + ",");
                        sbLine.Append(dr["Manufacturer_Product_Number"].ToString() + ",");
                        sbLine.Append(dr["QUantity"].ToString() + ",");
                        sbLine.Append(dr["Invoice_Number"].ToString() + ",");
                        sbLine.Append(dr["BSF_Location"].ToString());
                        sw.WriteLine(sbLine.ToString());
                        m_rowsCreated++;
                    }
                    sw.Close();
                }
                catch(Exception ex)
                { SetError(ex.Message); }
            }
            return !IsError;
        }

    
        public bool CreateHankookFile()
        {
            m_rowsCreated = 0;
            m_dtResults = new DataTable();
            DataAccess objDA = new DataAccess();
            DateTime dtStartMark = DateTime.Now;

            m_dtResults = objDA.GetDataTable(m_connectionString, "EXEC Dealer_Programs.dbo.up_DailyUploads_Hankook");
            m_queryRunTimeSeconds = TotalSeconds(dtStartMark);

            if (objDA.IsError)
            {
                SetError(objDA.ErrorMessage);
                return false;
            }
            else
            {
                try
                {
                    StreamWriter sw = File.CreateText(PathWack(m_outputFilePath) + m_outputFileName);
                    StringBuilder sbLine = new StringBuilder();

                    sw.WriteLine("SoldTo_Distributor_Number,ShipTo_Distributor_Number,Invoice_Number,Date_of_Sale,Hankook_ONE_Dealer_Number,Dealer_Name,Material_Number,Quantity");

                    foreach (DataRow dr in m_dtResults.Rows)
                    {
                        sbLine.Clear();
                        sbLine.Append(dr["SoldTo_Distributor_Number"].ToString() + ",");
                        sbLine.Append(dr["ShipTo_Distributor_Number"].ToString() + ",");
                        sbLine.Append(dr["Invoice_Number"].ToString() + ",");
                        sbLine.Append(dr["Date_Of_Sale"].ToString() + ",");
                        sbLine.Append(dr["HK_ONE_Dealer_Number"].ToString() + ",");
                        sbLine.Append(dr["Associate_Dealer_Name"].ToString() + ",");
                        sbLine.Append(dr["Material_Number"].ToString() + ","); 
                        sbLine.Append(dr["Quantity"].ToString());
                        sw.WriteLine(sbLine.ToString());
                        m_rowsCreated++;
                    }
                    sw.Close();
                }
                catch(Exception ex)
                {
                    SetError(ex.Message);
                }
            }
            return !IsError;
        }

        public bool CreateGoodyearFile()
        {
            m_rowsCreated = 0;
            m_dtResults = new DataTable();
            DataAccess objDA = new DataAccess();
            DateTime dtStartMark = DateTime.Now;

            m_dtResults = objDA.GetDataTable(m_connectionString, "EXEC Dealer_Programs.dbo.up_DailyUploads_Goodyear");
            m_queryRunTimeSeconds = TotalSeconds(dtStartMark);

            if (objDA.IsError)
            {
                SetError(objDA.ErrorMessage);
                return false;
            }
            else
            {
                try
                {
                    StreamWriter sw = File.CreateText(PathWack(m_outputFilePath) + m_outputFileName);
                    StringBuilder sbLine = new StringBuilder();
                    foreach(DataRow dr in m_dtResults.Rows)
                    {
                        sbLine.Clear();
                        sbLine.Append(dr["NON_SIG"].ToString() + ",");
                        sbLine.Append(dr["Product_Code"].ToString() + ",");
                        sbLine.Append(DateTime.Today.ToShortDateString() + ",");
                        sbLine.Append(DateTime.Now.ToLongTimeString() + ",");
                        sbLine.Append(dr["Quantity"].ToString());
                        sw.WriteLine(sbLine.ToString());
                        m_rowsCreated++;
                    }
                    sw.Close();
                }
                catch(Exception ex)
                { SetError(ex.Message); }

            }
            return !IsError;
        }

        public bool CreatePirelliFile()
        {
            m_rowsCreated = 0;
            m_dtResults = new DataTable();
            DataAccess objDA = new DataAccess();
            DateTime dtStartMark = DateTime.Now;
            
            m_dtResults = objDA.GetDataTable(m_connectionString, "EXEC Dealer_Programs.dbo.up_DailyUploads_Pirelli");
            m_queryRunTimeSeconds = TotalSeconds(dtStartMark);

            if (objDA.IsError)
            {
                SetError(objDA.ErrorMessage);
                return false;
            }
                                   
            if (objDA.IsError)
                SetError(objDA.ErrorMessage);
            else
            {
                try
                {
                    StreamWriter sw = File.CreateText(PathWack(m_outputFilePath) + m_outputFileName);
                    StringBuilder sb = new StringBuilder();
                    sb.Append("Pirelli Customer Code,Customer Material Code,Pirelli Material Code,Material Description,Quantity delivered,Reference Date,Ship-to Dealer Customer Code,Ship-to Customer Company Name,Ship-to Address,Ship-to City,Ship-to Postal Code,Ship-to Country,EAN Material Code,Document Type,Document Number,Document Date,Delivery Number");
                    sw.WriteLine(sb.ToString());

                    foreach (DataRow dr in m_dtResults.Rows)
                    {
                        sb.Clear();
                        sb.Append(dr["PI_Customer_Code"].ToString() + ",");
                        sb.Append(dr["tihlprd"].ToString() + ",");
                        sb.Append(dr["pdmfgprdno"].ToString() + ",");
                        sb.Append(dr["tihlprddsc"].ToString() + ",");
                        sb.Append(dr["tihlqty"].ToString() + ",");
                        sb.Append(dr["tihldteinp"].ToString() + ",");
                        sb.Append(dr["sxcvstrcv"].ToString() + ",");
                        sb.Append(dr["ssnamstore"].ToString() + ",");
                        sb.Append(dr["ssadr1"].ToString() + ",");
                        sb.Append(dr["sscity"].ToString() + ",");
                        sb.Append(dr["sszip"].ToString() + ",");
                        sb.Append(dr["Ship_To_Country"].ToString() + ",");
                        sb.Append(dr["EAN_Material_Code"].ToString() + ",");
                        sb.Append(dr["Document_Type"].ToString() + ",");
                        sb.Append(dr["tihhnuminv"].ToString() + ",");
                        sb.Append(dr["tihhdteinv"].ToString() + ",");
                        sw.WriteLine(sb.ToString());
                        m_rowsCreated++;
                    }
                    sw.Close();
                }
                catch(Exception ex)
                {
                    SetError(ex.Message);
                }                              
            }
            return !IsError;
        }

        public bool CreatePodiumFile()
        {
            m_rowsCreated = 0;
            m_dtResults = new DataTable();
            DataAccess objDA = new DataAccess();
            DateTime dtStartMark = DateTime.Now;

            m_dtResults = objDA.GetDataTable(m_connectionString, "EXEC Dealer_Programs.dbo.up_DailyUploads_Podium");
            m_queryRunTimeSeconds = TotalSeconds(dtStartMark);

            if(objDA.IsError)
            {
                SetError(objDA.ErrorMessage);
                return false;
            }
            else
            {
                try
                {
                    StreamWriter sw = File.CreateText(PathWack(m_outputFilePath) + m_outputFileName);
                    StringBuilder sbLine = new StringBuilder();

                    sw.WriteLine("InvoiceDate,CustomerName,CustomerClass,CellNumber,PrimaryNumber,BusinessNumber,StoreNumber,StoreName,Address1,City,State,Zip");

                    foreach(DataRow dr in m_dtResults.Rows)
                    {
                        sbLine.Clear();
                        sbLine.Append(dr["InvoiceDate"].ToString() + ",");
                        sbLine.Append(dr["CustomerName"].ToString() + ",");
                        sbLine.Append(dr["CustomerClass"].ToString() + ",");
                        sbLine.Append(dr["CELLPHONE"].ToString() + ",");
                        sbLine.Append(dr["HOMEPHONE"].ToString() + ",");
                        sbLine.Append(dr["WORKPHONE"].ToString() + ",");
                        sbLine.Append(dr["StoreNumber"].ToString() + ",");
                        sbLine.Append(dr["StoreName"].ToString() + ",");
                        sbLine.Append(dr["Address1"].ToString() + ",");
                        sbLine.Append(dr["City"].ToString() + ",");
                        sbLine.Append(dr["State"].ToString() + ",");
                        sbLine.Append(dr["Zip"].ToString());
                        sw.WriteLine(sbLine.ToString());
                        m_rowsCreated++;
                    }
                    sw.Close();
                }
                catch(Exception ex)
                {
                    SetError(ex.Message);
                }

            }

            return !IsError;
        }

        public bool CreateMavisFile()
        {
            m_rowsCreated = 0;
            m_dtResults = new DataTable();
            DataAccess objDA = new DataAccess();
            DateTime dtStartMark = DateTime.Now;

            m_dtResults = objDA.GetDataTable(m_connectionString, "EXEC Dealer_Programs.dbo.up_DailyUploads_Mavis");
            m_queryRunTimeSeconds = TotalSeconds(dtStartMark);

            if(objDA.IsError)
            {
                SetError(objDA.ErrorMessage);
                return false;
            }
            else
            {
                try
                {
                    StreamWriter sw = File.CreateText(PathWack(m_outputFilePath) + m_outputFileName);
                    StringBuilder sbLine = new StringBuilder();
                    sbLine.Append("TransactionType,Billing Number,Billing Name,ShipToNumber,ShipToName,StoreNumber,DistributionCenter,");
                    sbLine.Append("OrderNumber,OrderDate,InvoiceNumber,InvoiceDate,PONumber,Brand,ItemNumber,ItemDescription,OrderQuantity,");
                    sbLine.Append("QuantityShipped,InvoiceQuantity,UnitPrice,ExtendedPrice,Discount,TotalInvoiceCost,FetPerUnit,TaxAmount,");
                    sbLine.Append("DeliveryNote,ContainerNumber,ShipDate,SealNumber,PortOfDischarge,ETADoschargePort,ShippingLine,BillOfLading");
                    sbLine.Append("ServiceRendered,SignatureName,SignatureTime");
                    sw.WriteLine(sbLine.ToString());
                    
                    foreach(DataRow dr in m_dtResults.Rows)
                    {
                        sbLine.Clear();
                        sbLine.Append(dr["TRANSACTIONTYPE"].ToString());
                        sbLine.Append(dr["BILLTONUMBER"].ToString());
                        sbLine.Append(dr["BILLTONAME"].ToString());
                        sbLine.Append(dr["SHIPTONUMBER"].ToString());
                        sbLine.Append(dr["SHIPTONAME"].ToString());
                        sbLine.Append(dr["STORENUMBER"].ToString());
                        sbLine.Append(dr["DISTRIBUTIONCENTER"].ToString());
                        sbLine.Append(dr["ORDERNUMBER"].ToString());
                        sbLine.Append(dr["ORDERDATE"].ToString());
                        sbLine.Append(dr["INVOICENUMBER"].ToString());
                        sbLine.Append(dr["INVOICEDATE"].ToString());
                        sbLine.Append(dr["PONUMBER"].ToString());
                        sbLine.Append(dr["PRODUCTBRAND"].ToString());
                        sbLine.Append(dr["ITEMNUMBER"].ToString());
                        sbLine.Append(dr["ITEMDESCRIPTION"].ToString());
                        sbLine.Append(dr["QUANTITYORDERED"].ToString());
                        sbLine.Append(dr["QUANTITYSHIPPED"].ToString());
                        sbLine.Append(dr["QUANTITYBILLED"].ToString());
                        sbLine.Append(dr["UNITPRICE"].ToString());
                        sbLine.Append(dr["EXTENDEDPRICE"].ToString());
                        sbLine.Append(dr["ITEMDISCOUNT"].ToString());
                        sbLine.Append(dr["TOTALINVOICECOST"].ToString());
                        sbLine.Append(dr["FETPERUNIT"].ToString());
                        sbLine.Append(dr["TAXAMOUNT"].ToString());
                        sbLine.Append(dr["DELIVERYNOTE"].ToString());
                        sbLine.Append(dr["CONTAINERNUMBER"].ToString());
                        sbLine.Append(dr["SHIPDATE"].ToString());
                        sbLine.Append(dr["SEALNUMBER"].ToString());
                        sbLine.Append(dr["PORTOFDISCHARGE"].ToString());
                        sbLine.Append(dr["ETADISCHARGEPORT"].ToString());
                        sbLine.Append(dr["SHIPPINGLINE"].ToString());
                        sbLine.Append(dr["BOL"].ToString());
                        sbLine.Append(dr["DELIVERYDATE"].ToString());
                        sw.WriteLine(sbLine.ToString());
                        m_rowsCreated++;
                    }
                    sw.Close();
                }
                catch(Exception ex)
                {
                    SetError(ex.Message);
                }
            }
            return !IsError;
        }

        public bool CreateMetaMaddenFile()
        {
            m_rowsCreated = 0;
            m_dtResults = new DataTable();
            DataAccess objDA = new DataAccess();
            DateTime dtStartMark = DateTime.Now;

            m_dtResults = objDA.GetDataTable(m_connectionString, "EXEC MetaViewerIntegration.dbo.POMPS_GetExport");
            m_queryRunTimeSeconds = TotalSeconds(dtStartMark);

            if (objDA.IsError)
            {
                SetError(objDA.ErrorMessage);
                return false;
            }
            else
            {
                try
                {
                    StreamWriter sw = File.CreateText(PathWack(m_outputFilePath) + m_outputFileName);
                    StringBuilder sbLine = new StringBuilder();

                    foreach (DataRow dr in m_dtResults.Rows)
                    {
                        sbLine.Clear();
                        sbLine.Append(SetMetaLen(15, dr["VendorNumber"].ToString()));
                        sbLine.Append(SetMetaLen(15,dr["InvoiceNumber"].ToString()));
                        sbLine.Append(SetMetaLen(10,dr["PaymentNumber"].ToString()));
                        sbLine.Append(SetMetaLen(10,dr["SequenceNumber"].ToString()));
                        sbLine.Append(SetMetaLen(20,dr["InvoiceDescription"].ToString()));
                        sbLine.Append(SetMetaLen(15,dr["ExtendedPrice"].ToString()));
                        sbLine.Append(SetMetaLen(10,dr["TotalDiscount"].ToString()));
                        sbLine.Append(SetMetaLen(10,dr["InvoiceDate"].ToString()));
                        sbLine.Append(SetMetaLen(10,dr["DueDate"].ToString()));
                        sbLine.Append(SetMetaLen(10,dr["CheckDate"].ToString()));
                        sbLine.Append(SetMetaLen(10,dr["CheckNumber"].ToString()));
                        sbLine.Append(SetMetaLen(10,dr["HoldFlag"].ToString()));
                        sbLine.Append(SetMetaLen(10,dr["DiscountTakenCode"].ToString()));
                        sbLine.Append(SetMetaLen(10,dr["SelectCode"].ToString()));
                        sbLine.Append(SetMetaLen(10,dr["BankCode"].ToString()));
                        sbLine.Append(SetMetaLen(10,dr["Division"].ToString()));
                        sbLine.Append(SetMetaLen(10,dr["YearPosted"].ToString()));
                        sbLine.Append(SetMetaLen(10,dr["PeriodPosted"].ToString()));
                        sbLine.Append(SetMetaLen(10,dr["GLReference"].ToString()));
                        sbLine.Append(SetMetaLen(20,dr["LineDescription"].ToString()));
                        sbLine.Append(SetMetaLen(15,dr["Amount"].ToString()));
                        sbLine.Append(SetMetaLen(10,dr["Quantity"].ToString()));
                        sbLine.Append(SetMetaLen(10,dr["GL_AccountNumber"].ToString()));
                        sbLine.Append(SetMetaLen(10,dr["GL_Division"].ToString()));
                        sbLine.Append(SetMetaLen(10,dr["Department"].ToString()));
                        sw.WriteLine(sbLine.ToString());
                        m_rowsCreated++;
                    }
                    sw.Close();
                }
                catch (Exception ex)
                {
                    SetError(ex.Message);
                }
            }
            return !m_isError;
        }

        public string SetMetaLen(int Len, String sValue)
        {
            char sPadChar = ' ';
            if (sValue is null)
                sValue = "";
            else if (sValue.Length > Len)
                sValue = sValue.Substring(0, Len);
            else
                sValue = sValue.PadRight(Len, sPadChar);

            return sValue;
        }

        public void UpdateNotNewDealerProfiles()
        {
            // flips the newdealer flag in dealerprofile table in DB. 
            // Changing flag to 0 will cause future reporting to use default date of program
            // instead of dealer start date.
            try
            {
                DataAccess objDA = new DataAccess();
                String sql = "EXEC Dealer_Programs.dbo.up_DailyUploads_UpdateNotNewDealers ";
                objDA.ExecNonQuery(m_connectionString, sql);
                if (objDA.IsError)
                    throw new ApplicationException(objDA.ErrorMessage);
            }
            catch(Exception ex)
            { SetError(ex.Message); }
        }
 
        private string PathWack(string SomePath)
        {
            if (SomePath.EndsWith("\\") == false)
                SomePath += "\\";
            return SomePath;
        }

        private readonly Func<DateTime,int> TotalSeconds = startDatetime => (DateTime.Now.Subtract(startDatetime).Minutes * 60) + DateTime.Now.Subtract(startDatetime).Seconds; 

        private void ClearError()
        {
            m_isError = false;
            m_errorMessage = "";
        }

        private void SetError(string msg)
        {
            m_isError = true;
            if (m_errorMessage.Length > 0)
                m_errorMessage += ";";

            m_errorMessage += msg;
        }

        public bool IsError
        { get => m_isError; }

        public string ErrorMessage
        { get => m_errorMessage; }

        public string ConnectionString
        { set => m_connectionString = value; }

        public string OutputFilePath
        {
            set => m_outputFilePath = value;
            get => m_outputFilePath;
        }

        public string OutputFileName
        {
            set => m_outputFileName = value;
            get => m_outputFileName;
        }

        public int QueryRunTimeSeconds { get => m_queryRunTimeSeconds; }

        public int RowsCreated { get => m_rowsCreated; }

        public DataTable DataFileOutput { get => m_dtResults; }

    }
}
