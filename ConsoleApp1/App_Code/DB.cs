using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace ConsoleApp1.App_Code
{
    class DB
    {
        public static string ConnetionString()
        {
            string DSN = "Server=192.168.75.18;DataBase=EG_MagicCube;Uid=vip;Pwd=vip;";
            return DSN;

        }

        public static DataTable DBQuery(string SQL, Dictionary<string, object> param)
        {
            string DBstring = DB.ConnetionString();
            DataTable dt = new DataTable();

            if (IsServerConnected() == false)
                return dt;

            SqlDataAdapter da = new SqlDataAdapter(SQL, DBstring);
            foreach (KeyValuePair<string, object> item in param)
                da.SelectCommand.Parameters.AddWithValue("@" + item.Key, item.Value);

            da.Fill(dt);
            return dt;
        }

        public static int ExecuteNonQuery(string query, Dictionary<string, object> dict = null)
        {
            int result = 0;
            string connString = DB.ConnetionString();

            if (IsServerConnected() == false)
                return result;


            SqlConnection _conn = new SqlConnection(connString);
            DataTable dt = new DataTable();
            using (SqlCommand cmd = new SqlCommand(query, _conn))
            {
                _conn.Open();
                SqlTransaction transaction;
                transaction = _conn.BeginTransaction();
                cmd.Transaction = transaction;
                if (cmd.Connection.State != ConnectionState.Open) cmd.Connection.Open();

                foreach (KeyValuePair<string, object> item in dict)
                {
                    cmd.Parameters.AddWithValue("@" + item.Key, item.Value);
                }
                try
                {
                    result = cmd.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (Exception ex){
                    string msg = ex.ToString();
                    transaction.Rollback();
                }

                if (cmd.Connection.State != ConnectionState.Closed) cmd.Connection.Close();
            }

            return result;
        }

        public static bool IsServerConnected()
        {
            using (var l_oConnection = new SqlConnection(DB.ConnetionString()))
            {
                try
                {
                    l_oConnection.Open();
                    return true;
                }
                catch (SqlException)
                {
                    return false;
                }
            }
        }

        #region "加密／解密"

        private static string gStrKey = "CASPER59";

        private static string gStrIV = "TEMPLATE";

        /// <summary>字串編碼</summary>
        /// <param name="strSource">原始字串</param>
        /// <returns>編碼後的結果字串</returns>
        /// <remarks></remarks>
        public static string EnCrypt(string pStrSource)
        {
            byte[] tBuffer = Encoding.Default.GetBytes(pStrSource);
            MemoryStream ms = new MemoryStream();
            DESCryptoServiceProvider tdes = new DESCryptoServiceProvider();
            CryptoStream tEncStream = new CryptoStream(ms, tdes.CreateEncryptor(Encoding.Default.GetBytes(gStrKey), Encoding.Default.GetBytes(gStrIV)), CryptoStreamMode.Write);
            tEncStream.Write(tBuffer, 0, tBuffer.Length);
            tEncStream.FlushFinalBlock();
            ms.Close();
            tEncStream.Close();
            return Convert.ToBase64String(ms.ToArray());
        }

        /// <summary>字串解碼</summary>
        /// <param name="strSource">編碼過的字串</param>
        /// <returns>未編碼的原始字串</returns>
        /// <remarks></remarks>
        public static string DeCrypt(string pStrSource)
        {
            byte[] tBuffer = Convert.FromBase64String(pStrSource);
            MemoryStream tMS = new MemoryStream();
            DESCryptoServiceProvider tDES = new DESCryptoServiceProvider();
            CryptoStream tEncStream = new CryptoStream(tMS, tDES.CreateDecryptor(Encoding.Default.GetBytes(gStrKey), Encoding.Default.GetBytes(gStrIV)), CryptoStreamMode.Write);
            tEncStream.Write(tBuffer, 0, tBuffer.Length);
            tEncStream.FlushFinalBlock();
            tMS.Close();
            tEncStream.Close();
            return Encoding.Default.GetString(tMS.ToArray());
        }

        #endregion
    }
}

