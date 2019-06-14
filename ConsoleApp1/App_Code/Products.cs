using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.App_Code
{
    class Products
    {
        public static void Update()
        {
            int TotalCount = 0;

            try
            {
                List<Z_MM_QUBE_MATNR> Z_MM_QUBE_MATNR_List = new List<Z_MM_QUBE_MATNR>();
                ConsoleApp1.RFC.serviceSoapClient ws = new ConsoleApp1.RFC.serviceSoapClient();
                string StartDate = "20180101";
                string EndDate = DateTime.Now.ToString("yyyyMMdd");

                string JsonString = ws.GetAllProducts(StartDate, EndDate);
                Z_MM_QUBE_MATNR_List = JsonConvert.DeserializeObject<List<Z_MM_QUBE_MATNR>>(JsonString);

                foreach (var item in Z_MM_QUBE_MATNR_List)
                {
                    string SqlCommand = " ";
                    string ExcuteResult = "";
                    DataTable dt = new DataTable();
                    Dictionary<string, object> param = new Dictionary<string, object>();
                    param.Add("MaterialsID", item.MATNR);
                    param.Add("WorksName", item.MAKTX);
                    param.Add("AuthorsNo", item.LIFNR);
                    param.Add("GenreName", item.WGBEZ);
                    param.Add("Purchase", Double.Parse(item.NETPR));
                    param.Add("Cost", Double.Parse(item.DMBTR));
                    param.Add("Material", item.ZZMATU);
                    param.Add("Currency", item.WAERS);
                    param.Add("ZFVOLUME", item.ZFVOLUME);
                    param.Add("ZZPAGE", item.ZZPAGE);
                    param.Add("WorkSize", item.ZZSE);

                    dt = DB.DBQuery("SELECT * From Works where MaterialsID=@MaterialsID AND IsDel='' ", param);

                    if (dt.Rows.Count > 0)
                    {

                        //作品名稱、媒材中、組件、作品尺寸修改時不可從SAP覆帶到Cube
                        SqlCommand = "Update Works set " +
                            "AuthorsNo=@AuthorsNo," +
                            "Cost=@Cost," +
                            "Purchase=@Purchase," +
                            "Currency=@Currency," +
                            "GenreName=@GenreName," +
                            "ZZPAGE=@ZZPAGE," +
                            "WorkSize=@WorkSize," +
                            "ChangeState='Update', " +
                            "ModifyDate=GETDATE()," +
                            "ModifyUser='system' " +
                            "where MaterialsID=@MaterialsID ; ";

                        //更新藝術家作品總數
                        SqlCommand += "UPDATE Authors set WorksAmount=(SELECT isnull(count(*),0) from Works where AuthorsNo=@AuthorsNo AND IsDel='') where LIFNR=@AuthorsNo ; ";
                        ExcuteResult = string.Format("修改商品：{0}", item.MAKTX);
                    }
                    else
                    {
                        SqlCommand = "Insert Into Works (MaterialsID,WorksName,AuthorsNo,Cost,Purchase,Currency,GenreName,Material,ZFVOLUME,ZZPAGE,WorkSize,ChangeState,CreateUser,ModifyUser,ModifyDate) " +
                           "VALUES (@MaterialsID,@WorksName,@AuthorsNo,@Cost,@Purchase,@Currency,@GenreName,@Material,@ZFVOLUME,@ZZPAGE,@WorkSize,'Create','system','system',GETDATE()); ";

                        //更新藝術家作品總數
                        SqlCommand += "UPDATE Authors set WorksAmount=(SELECT isnull(count(*),0) from Works where AuthorsNo=@AuthorsNo AND IsDel='') where LIFNR=@AuthorsNo ; ";
                        ExcuteResult = string.Format("新增商品：{0}", item.MAKTX);
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

                Console.WriteLine(string.Format("共完成{0}筆商品資料異動", TotalCount));
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

    }


    public class Z_MM_QUBE_MATNR
    {
        //SAP料號
        public string MATNR = "";
        //商品中文名稱
        public string MAKTX = "";
        //作品名2
        public string ZZCT2 = "";
        //供應商編號
        public string LIFNR = "";
        //媒材
        public string ZZMATU = "";
        //容量(組件)
        public string ZFVOLUME = "";
        //頁數
        public string ZZPAGE = "";
        //作品類型
        public string WGBEZ = "";
        //年代
        public string ZFPRESERVE_D = "";
        //作品尺幅(寬)
        public string ZZSE = "";
        //採購價
        public string NETPR = "";
        //採購幣
        public string WAERS = "";
        //成本價
        public string DMBTR = "";
        //新增日期
        public string ERDAT = "";
        //修改日期
        public string UDATE = "";

    }

}

   

