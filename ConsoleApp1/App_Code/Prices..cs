using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.App_Code
{
    class Prices
    {
        public static void Update()
        {
            int TotalCount = 0;

            try
            {
                List<Z_MM_QUBE_VKP0> Z_MM_QUBE_VKP0_List = new List<Z_MM_QUBE_VKP0>();
                ConsoleApp1.RFC.serviceSoapClient ws = new ConsoleApp1.RFC.serviceSoapClient();

                string P_DATE = DateTime.Now.ToString("yyyyMMdd");
                string JsonString = ws.GetAllPrice(P_DATE);
                Z_MM_QUBE_VKP0_List = JsonConvert.DeserializeObject<List<Z_MM_QUBE_VKP0>>(JsonString);

                foreach (var item in Z_MM_QUBE_VKP0_List)
                {
                    string SqlCommand = " ";
                    string ExcuteResult = "";
                    DataTable dt = new DataTable();
                    Dictionary<string, object> param = new Dictionary<string, object>();
                    param.Add("MaterialsID", item.MATNR);
                    param.Add("Currency", item.WAERS);
                    param.Add("Price", Double.Parse(item.BRTWR));
                    param.Add("PricingDate", item.DATAB);
                    param.Add("Source", "SAP");


                    dt = DB.DBQuery("SELECT * FROM WorksPrices where MaterialsID=@MaterialsID and Currency=@Currency ", param);

                    if (dt.Rows.Count > 0)
                    {
                        SqlCommand = "UPDATE WorksPrices set Price=@Price,PricingDate=@PricingDate,ModifyDate=GetDate(),Source=@Source  where MaterialsID=@MaterialsID and Currency=@Currency; ";
                        ExcuteResult = string.Format("更新售價：{0}_{1}", item.MATNR, item.WAERS);
                    }
                    else
                    {
                        SqlCommand = "Insert Into WorksPrices (MaterialsID,Currency,Price,PricingDate,CreateDate,Source) VALUES (@MaterialsID,@Currency,@Price,@PricingDate,GetDate(),@Source); ";
                        ExcuteResult = string.Format("更新售價：{0}_{1}", item.MATNR, item.WAERS);
                    }

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

                Console.WriteLine(string.Format("共完成{0}筆售價資料異動", TotalCount));

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

    public class Z_MM_QUBE_VKP0
    {
        public string MATNR = "";
        public string WAERS = "";
        public string BRTWR = "";
        public string DATAB = "";
        public string DATBI = "";
    }
}
