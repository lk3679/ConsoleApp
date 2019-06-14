using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.App_Code
{
    class Stocks
    {
        public static void Update()
        {
            int TotalCount = 0;

            try
            {
                List<Z_MM_QUBE_MENGE> Z_MM_QUBE_MENGE_List = new List<Z_MM_QUBE_MENGE>();
                ConsoleApp1.RFC.serviceSoapClient ws = new ConsoleApp1.RFC.serviceSoapClient();
                string P_DATE = DateTime.Now.ToString("yyyyMMdd");
                string JsonString = ws.GetAllStock(P_DATE);
                Z_MM_QUBE_MENGE_List = JsonConvert.DeserializeObject<List<Z_MM_QUBE_MENGE>>(JsonString);

                foreach (var item in Z_MM_QUBE_MENGE_List)
                {
                    string SqlCommand = "";
                    string ExcuteResult = "";
                    DataTable dt = new DataTable();
                    Dictionary<string, object> param = new Dictionary<string, object>();
                    param.Add("MaterialsID", item.MATNR);
                    param.Add("WERKS", item.WERKS);
                    param.Add("MENGE", Double.Parse(item.MENGE));

                    dt = DB.DBQuery("SELECT * from WorksStocks where MaterialsID=@MaterialsID and WERKS=@WERKS ", param);

                    if (dt.Rows.Count > 0)
                    {
                        SqlCommand = "UPDATE WorksStocks set MENGE=@MENGE,ModifyDate=GetDate()  where MaterialsID=@MaterialsID and WERKS=@WERKS; ";
                        ExcuteResult = string.Format("更新庫存數：{0}_{1}", item.MATNR, item.WERKS);
                    }
                    else
                    {
                        SqlCommand = "Insert Into WorksStocks (MaterialsID,WERKS,MENGE,CreateDate) VALUES (@MaterialsID,@WERKS,@MENGE,GetDate()); ";
                        ExcuteResult = string.Format("新增庫存數：{0}_{1}", item.MATNR, item.WERKS);
                    }

                    //更新該商品的庫存總數
                    SqlCommand += "UPDATE works set TotalInventory=(SELECT isnull(sum(MENGE),0) FROM [dbo].[WorksStocks] where MaterialsID=@MaterialsID)  ";
                    SqlCommand += "Where MaterialsID =@MaterialsID; ";
                    int num = DB.ExecuteNonQuery(SqlCommand, param);
                    if (num > 0)
                    {
                        TotalCount += num;
                        Console.WriteLine(ExcuteResult);
                    }
                    else
                    {
                        Console.WriteLine("庫存資料異動失敗");
                    }
                }

                Console.WriteLine(string.Format("共完成{0}筆庫存資料異動", TotalCount));

            } catch(Exception e)
            {
                //發送通知信
                string strMailTitle = "系統發生錯誤";
                string str_mailbody = e.ToString();
                Mail.Send(strMailTitle, str_mailbody);
                Console.WriteLine(string.Format("系統發生錯誤：{0}", e.ToString()));
            }
        
        }

    }

    public class Z_MM_QUBE_MENGE
    {
        public string MATNR = "";
        public string MENGE = "";
        public string WERKS = "";

    }
}
