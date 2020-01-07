using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;

namespace Dealer_Programs_Uploads
{
    class DataAccess
    {
        private bool m_iserror;
        private string m_errormessage;
        private int m_queryExecTimeout;

        public DataAccess()
        {
            ClearError();
            m_queryExecTimeout = 30; //seconds, set as default, overridable by caller
        }

        public DataTable GetDataTable(string SqlConnectionString, string SQL)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlDataAdapter da = new SqlDataAdapter(SQL, SqlConnectionString);
                da.SelectCommand.CommandTimeout = m_queryExecTimeout * 1000; // convert seconds to milliseconds
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                SetError(ex.Message);
            }
            return dt;
        }

        public DataSet GetDataSet(string SqlConnectionString, string SQL)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlDataAdapter da = new SqlDataAdapter(SQL, SqlConnectionString);
                da.SelectCommand.CommandTimeout = m_queryExecTimeout * 1000; // convert seconds to milliseconds
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                SetError(ex.Message);
            }
            return ds;
        }

        public void ExecNonQuery(string SqlConnectionString, string SQL)
        {
            SqlCommand objComm = new SqlCommand(SQL, new SqlConnection(SqlConnectionString));
            try
            {
                objComm.CommandTimeout = m_queryExecTimeout * 1000; //
                objComm.Connection.Open();
                objComm.ExecuteNonQuery();
            }
            catch (Exception ex)
            { SetError(ex.Message); }
            finally
            { objComm.Connection.Close(); }
        }

        public object GetScalarValue(string SqlConnectionString, string SQL)
        {
            object rtnVal = new object();
            SqlCommand objComm = new SqlCommand(SQL, new SqlConnection(SqlConnectionString));
            try
            {
                objComm.CommandTimeout = m_queryExecTimeout * 1000; //
                objComm.Connection.Open();
                rtnVal = objComm.ExecuteScalar();
            }
            catch (Exception ex)
            { SetError(ex.Message); }
            finally
            { objComm.Connection.Close(); }
            return rtnVal;
        }

        public string SelectConnString(string dataSource, string initialCatalog, string userName = "", string passWord = "")
        {
            SqlConnectionStringBuilder sconnBuilder = new SqlConnectionStringBuilder();
            sconnBuilder.DataSource = dataSource;
            sconnBuilder.InitialCatalog = initialCatalog;
            if (userName.Length > 0)
            {
                sconnBuilder.UserID = userName;
                sconnBuilder.Password = passWord;
            }
            else
                sconnBuilder.IntegratedSecurity = true;

            sconnBuilder.ConnectTimeout = 30;                       
            return sconnBuilder.ConnectionString;
        }

        public int QueryExecTimeoutSeconds
        {
            get { return m_queryExecTimeout; }
            set { m_queryExecTimeout = value; }
        }

        private void SetError(string msg)
        {
            if (m_errormessage.Length > 0)
                m_errormessage += ";";

            m_errormessage += msg;
            m_iserror = true;
        }

        private void ClearError()
        {
            m_iserror = false;
            m_errormessage = "";
        }

        public bool IsError
        { get { return m_iserror; } }

        public string ErrorMessage
        { get { return m_errormessage; } }
    }
}
