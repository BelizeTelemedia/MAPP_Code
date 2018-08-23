using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Diagnostics;


using System.Xml.Serialization;

[WebService(Namespace = "https://tst.belizetelemedia.net:8443/MAPP/MID_MAPP/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]

public class Service : System.Web.Services.WebService
{
    [WebMethod]

    public VO_RESPONSE INV_FN_SET_RANDOM_CODE(string VI_USERNAME, string VI_PASSWORD, string VI_MSISDN)
    {
        VO_RESPONSE response = new VO_RESPONSE();
        DBConnection MID = new DBConnection();
        try
        {
            MID.OpenDB();
            if (!MID.FN_LOGIN(VI_USERNAME, VI_PASSWORD))
            {
                response.VO_RESULT = -1;
                response.VO_MESSAGE = "Invalid Username/Password system login";
                return response;
            }
            response = MID.FN_SET_RANDOM_CODE(VI_MSISDN);

            return response;
        }
        catch (Exception ex)
        {
            response.VO_RESULT = -9001;
            response.VO_MESSAGE = string.Concat("WSDL EXCEPTION: ",ex.Message);
            return response;
        }

    }

    [WebMethod]

    public VO_RESPONSE INV_FN_SET_CONFIRM_REG(string VI_USERNAME, string VI_PASSWORD, string VI_MSISDN, string VI_SUB_PASSWORD, string VI_EMAIL, int VI_CODE)
    {
        VO_RESPONSE response = new VO_RESPONSE();
        DBConnection MID = new DBConnection();
        try
        {
            MID.OpenDB();
            if (!MID.FN_LOGIN(VI_USERNAME, VI_PASSWORD))
            {
                response.VO_RESULT = -1;
                response.VO_MESSAGE = "Invalid Username/ Password system login";
                return response;
            }
            response = MID.FN_SET_CONFIRM_REG(VI_MSISDN, VI_SUB_PASSWORD, VI_EMAIL, VI_CODE);

            return response;
        }
        catch (Exception ex)
        {
            response.VO_RESULT = -9001;
            response.VO_MESSAGE = string.Concat("WSDL EXCEPTION: ", ex.Message);
            return response;
        }

    }

    [WebMethod]

    public VO_RESPONSE SET_SELFCARE_LOGIN (string VI_USERNAME, string VI_PASSWORD, string VI_MSISDN, int VI_CODE)
    {
        VO_RESPONSE response = new VO_RESPONSE();
        DBConnection MID = new DBConnection();
        try
        {
            MID.OpenDB();
            if (!MID.FN_LOGIN(VI_USERNAME, VI_PASSWORD))
            {
                response.VO_RESULT = -1;
                response.VO_MESSAGE = "Invalid Username/ Password system login";
                return response;
            }
            response = MID.SET_SELFCARE_LOGIN(VI_MSISDN, VI_CODE);

            return response;
        }
        catch (Exception ex)
        {
            response.VO_RESULT = -9001;
            response.VO_MESSAGE = string.Concat("WSDL EXCEPTION: ", ex.Message);
            return response;
        }

    }

    public struct VO_RESPONSE
    {
        public int VO_RESULT;
        public string VO_MESSAGE;        
    }

    private class DBConnection
    {
        //constructor with Oracle TYTAN Connection String Definition
        public DBConnection()
        {

        }
        //Definition of TYTANConnections Class variables
        public string Logon;
        public string Response_FN;
        public DataSet ds = new DataSet(); //
        /* --dev database 
        private static string OracleServer = "Data Source=(DESCRIPTION="
                + "(ADDRESS=(PROTOCOL=TCP)(HOST=172.21.56.30)(PORT=1521))"
                + "(CONNECT_DATA=(SERVICE_NAME=mdlwdev)));"
                + "User Id=midware;Password=midware";*/

        /*  --test database  */
        private static string OracleServer = "Data Source=(DESCRIPTION="
                + "(ADDRESS=(PROTOCOL=TCP)(HOST=172.21.56.33)(PORT=1521))"
                + "(CONNECT_DATA=(SERVICE_NAME=mdlwtst)));"
                + "User Id=midware;Password=midware";

        /*  --prod database  
        private static string OracleServer = "Data Source=(DESCRIPTION="
                + "(ADDRESS=(PROTOCOL=TCP)(HOST=172.21.52.55)(PORT=1521))"
                + "(CONNECT_DATA=(SERVICE_NAME=mdlwprd)));"
                + "User Id=midware;Password=midware";*/

        private OracleConnection conn = new OracleConnection(OracleServer);

        //Function used to Open database connectivity

        public bool OpenDB()
        {
            try
            {
                conn.Open();
                Logon = "Database connection successful";//Message is Displayed on Label in asp page to verfiy connection to database
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception");
                Logon = "Unable to Connect to Database " + ex;//Message is displays error in Label when unable to connect
                return false;
            }
        }

        //Function used to Close database connectivity
        public void Close()
        {
            conn.Close();
            conn.Dispose();
        }

        public DataSet SelectExecute(string sql)
        {

            OracleDataAdapter da = new OracleDataAdapter(sql, conn);
            da.Fill(ds, "results");

            return ds;
        }

        public VO_RESPONSE FN_SET_RANDOM_CODE(string vi_msisdn)
        {
            VO_RESPONSE Response_FN = new VO_RESPONSE();
            string vo_message = "";
            int vo_result = -1;
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandText = "PCK_MAPP.SET_RANDOM_CODE";
            cmd.CommandType = CommandType.StoredProcedure;


            cmd.Parameters.Add("VO_RES", OracleDbType.Int32, 1);
            cmd.Parameters["VO_RES"].Direction = ParameterDirection.ReturnValue;

            cmd.Parameters.Add("VI_MSISDN", OracleDbType.Varchar2, 100, vi_msisdn, ParameterDirection.Input);
            cmd.Parameters.Add("VO_MESSAGE", OracleDbType.Varchar2, 200, vo_message, ParameterDirection.Output);
            cmd.Parameters.Add("VO_RESULT", OracleDbType.Int32, 1, vo_result, ParameterDirection.Output);

            cmd.ExecuteNonQuery();
            Response_FN.VO_MESSAGE = cmd.Parameters["VO_MESSAGE"].Value.ToString();
            Response_FN.VO_RESULT = int.Parse(cmd.Parameters["VO_RESULT"].Value.ToString());
            cmd.Dispose();
            Close();
            return Response_FN;
        }

        public VO_RESPONSE FN_SET_CONFIRM_REG(string vi_msisdn, string vi_password, string vi_email, int vi_code)
        {
            VO_RESPONSE Response_FN = new VO_RESPONSE();
            string vo_message = "";
            int vo_result = -1;
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandText = "PCK_MAPP.SET_CONFIRM_REG";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("VO_RES", OracleDbType.Int32, 1);
            cmd.Parameters["VO_RES"].Direction = ParameterDirection.ReturnValue;

            cmd.Parameters.Add("VI_MSISDN", OracleDbType.Varchar2, 100, vi_msisdn, ParameterDirection.Input);
            cmd.Parameters.Add("VI_PASSWORD", OracleDbType.Varchar2, 100, vi_password, ParameterDirection.Input);
            cmd.Parameters.Add("VI_EMAIL", OracleDbType.Varchar2, 100, vi_email, ParameterDirection.Input);
            cmd.Parameters.Add("VI_CODE", OracleDbType.Int32, 4, vi_code, ParameterDirection.Input);
            cmd.Parameters.Add("VO_MESSAGE", OracleDbType.Varchar2, 200, vo_message, ParameterDirection.Output);
            cmd.Parameters.Add("VO_RESULT", OracleDbType.Int32, 1, vo_result, ParameterDirection.Output);
            cmd.ExecuteNonQuery();
            Response_FN.VO_MESSAGE = cmd.Parameters["VO_MESSAGE"].Value.ToString();
            Response_FN.VO_RESULT = int.Parse(cmd.Parameters["VO_RESULT"].Value.ToString());
            cmd.Dispose();
            Close();
            return Response_FN;

        }

        public VO_RESPONSE SET_SELFCARE_LOGIN(string vi_msisdn, int vi_code)
        {
            VO_RESPONSE Response_FN = new VO_RESPONSE();
            string vo_message = "";
            int vo_result = -1;
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandText = "PCK_MAPP.SET_SELFCARE_LOGIN";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("VO_RES", OracleDbType.Int32, 1);
            cmd.Parameters["VO_RES"].Direction = ParameterDirection.ReturnValue;

            cmd.Parameters.Add("VI_MSISDN", OracleDbType.Varchar2, 100, vi_msisdn, ParameterDirection.Input);
            cmd.Parameters.Add("VI_CODE", OracleDbType.Int32, 4, vi_code, ParameterDirection.Input);
            cmd.Parameters.Add("VO_MESSAGE", OracleDbType.Varchar2, 200, vo_message, ParameterDirection.Output);
            cmd.Parameters.Add("VO_RESULT", OracleDbType.Int32, 1, vo_result, ParameterDirection.Output);
            cmd.ExecuteNonQuery();
            Response_FN.VO_MESSAGE = cmd.Parameters["VO_MESSAGE"].Value.ToString();
            Response_FN.VO_RESULT = int.Parse(cmd.Parameters["VO_RESULT"].Value.ToString());
            cmd.Dispose();
            Close();
            return Response_FN;

        }

        public bool FN_LOGIN(String VI_USERNAME, String VI_PASSWORD)
        {

            int RES = -1;
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandText = "PCK_MIDDLE.MID_LOGIN";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("VO_RES", OracleDbType.Int32, RES, ParameterDirection.ReturnValue);

            Debug.WriteLine("USERNAME: |" + VI_USERNAME + "|");
            Debug.WriteLine("Password: |" + VI_PASSWORD + "|");
            cmd.Parameters.Add("VI_USERNAME", OracleDbType.Varchar2, 100, VI_USERNAME, ParameterDirection.Input);
            cmd.Parameters.Add("VI_PASSWORD", OracleDbType.Varchar2, 100, VI_PASSWORD, ParameterDirection.Input);


            cmd.ExecuteNonQuery();

            Debug.WriteLine("RES: " + int.Parse(cmd.Parameters["VO_RES"].Value.ToString()));
            RES = int.Parse(cmd.Parameters["VO_RES"].Value.ToString());
            if (RES < 0)
            {
                return false;
            }
            return true;
        }

    }

}