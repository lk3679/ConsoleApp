using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.App_Code
{
    class Warehouses
    {
        public static void Update()
        {
            int TotalCount = 0;

            try {

                List<Z_MM_QUBE_WERKS> Z_MM_QUBE_WERKS_List = new List<Z_MM_QUBE_WERKS>();
                ConsoleApp1.RFC.serviceSoapClient ws = new ConsoleApp1.RFC.serviceSoapClient();
                string JsonString = ws.GetAllWarehouses();

                if (string.IsNullOrEmpty(JsonString) == false)
                {
                    string SqlCommand = " ";
                    Z_MM_QUBE_WERKS_List = JsonConvert.DeserializeObject<List<Z_MM_QUBE_WERKS>>(JsonString);

                    foreach (var item in Z_MM_QUBE_WERKS_List)
                    {
                        DataTable dt = new DataTable();
                        Dictionary<string, object> param = new Dictionary<string, object>();
                        param.Add("werks", item.WERKS);
                        param.Add("WareTypeName", item.NAME1);
                        dt = DB.DBQuery("SELECT * FROM [dbo].[Menu_WareType] where werks=@werks ", param);
                        //寫入到Cube的DB

                        if (dt.Rows.Count > 0)
                        {
                            SqlCommand = "UPDATE Menu_WareType set WareTypeName=@WareTypeName  where werks=@werks ;";
                            Console.WriteLine(string.Format("更新倉庫：{0}", item.NAME1));

                        }
                        else
                        {
                            SqlCommand = "Insert Into Menu_WareType (WareTypeName,Werks) VALUES(@WareTypeName,@werks) ;";
                            Console.WriteLine(string.Format("新增倉庫：{0}", item.NAME1));
                        }

                        int num = DB.ExecuteNonQuery(SqlCommand, param);
                        TotalCount += num;
                    }
                }

                Console.WriteLine(string.Format("共完成{0}筆倉庫資料異動", TotalCount));



            } catch (Exception e) {

                //發送通知信
                string strMailTitle = "系統發生錯誤";
                string str_mailbody = e.ToString();
                Mail.Send(strMailTitle, str_mailbody);
                Console.WriteLine(string.Format("系統發生錯誤：{0}", e.ToString()));
            }
        }
        
    }

    public class Z_MM_QUBE_WERKS
    {
        public string WERKS = "";
        public string NAME1 = "";
        public string EKORG = "";

    }
}
