using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.App_Code
{
    class Vendors
    {
        public static void Update()
        {
            int TotalCount = 0;

            try {

                List<Z_MM_QUBE_LIFNR> Z_MM_QUBE_LIFNR_List = new List<Z_MM_QUBE_LIFNR>();
                ConsoleApp1.RFC.serviceSoapClient ws = new ConsoleApp1.RFC.serviceSoapClient();
                string StartDate = "20160101";
                string EndDate = DateTime.Now.ToString("yyyyMMdd");
                string JsonString = ws.GetAllVendors(StartDate, EndDate);
                Z_MM_QUBE_LIFNR_List = JsonConvert.DeserializeObject<List<Z_MM_QUBE_LIFNR>>(JsonString);

                foreach (var item in Z_MM_QUBE_LIFNR_List)
                {
                    string SqlCommand = " ";
                    string ExcuteResult = "";
                    DataTable dt = new DataTable();
                    Dictionary<string, object> param = new Dictionary<string, object>();
                    param.Add("LIFNR", item.LIFNR);
                    param.Add("AuthorsCName", item.NAME1);
                    param.Add("LAND1", item.LAND1);

                    dt = DB.DBQuery("SELECT * FROM [dbo].[Authors] where LIFNR=@LIFNR ", param);

                    if (dt.Rows.Count > 0)
                    {
                        SqlCommand = "Update Authors set AuthorsCName=@AuthorsCName,LAND1=@LAND1,ModifyUser='system',ModifyDate=GETDATE()  where LIFNR=@LIFNR ";
                        ExcuteResult = string.Format("更新供應商：{0}", item.NAME1);
                    }
                    else
                    {
                        SqlCommand = "Insert Into Authors (AuthorsCName,LAND1,CreateUser,CreateDate,ModifyUser,ModifyDate,,Rating,WorksAmount,LIFNR) VALUES (@AuthorsCName,@LAND1,'system',GETDATE(),'system',GETDATE(),0,0,@LIFNR); ";
                        ExcuteResult = string.Format("新增供應商：{0}", item.NAME1);
                    }

                    int num = DB.ExecuteNonQuery(SqlCommand, param);
                    TotalCount += num;
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

                Console.WriteLine(string.Format("共完成{0}筆供應商資料異動", TotalCount));

            }
            catch (Exception e)
            {
                //發送通知信
                string strMailTitle = "系統發生錯誤";
                string str_mailbody = e.ToString();
                Mail.Send(strMailTitle, str_mailbody);
                Console.WriteLine(string.Format("系統發生錯誤：{0}", e.ToString()));
            }
                   
        }

        public class Z_MM_QUBE_LIFNR
        {
            public string LIFNR = "";
            public string NAME1 = "";
            public string ZZAUTH = "";
            public string LAND1 = "";
            public string SORTL = "";
            public string ERDAT = "";
            public string UDATE = "";
        }
    }
}
