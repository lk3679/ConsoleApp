using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.App_Code
{
    class Work
    {
        public static void Convert()
        {
            int TotalCount = 0;

            DataTable dt = new DataTable();
            List<WorkObj> WorkList = new List<WorkObj>();
            Dictionary<string, object> QueryParam = new Dictionary<string, object>();
            dt = DB.DBQuery("Select * From SapWorks  ", QueryParam);

            foreach(DataRow dr in dt.Rows)
            {
                string SqlCommand = "";
                string ExcuteResult = "";
                Dictionary<string, object> param = new Dictionary<string, object>();

                WorkObj _WorkObj = new WorkObj();
                _WorkObj.WorksNo = dr["WorksNo"].ToString();
                _WorkObj.SapNo= dr["MaterialsID"].ToString();
                _WorkObj.WorksName= dr["WorksName"].ToString();
                WorkList.Add(_WorkObj);

                SqlCommand = string.Format("Update Works set MaterialsID=@SapNo  where WorksNo=@WorksNo ");
                ExcuteResult=string.Format("更新作品名稱{0}的SAP料號",_WorkObj.WorksName);
                param.Add("SapNo",_WorkObj.SapNo);
                param.Add("WorksNo", _WorkObj.WorksNo);

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

    }

    class WorkObj
    {
        public string WorksNo = "";
        public string SapNo = "";
        public string WorksName = "";
        public string StartYear = "";
        public string EndYear = "";
        public string Price = "";
    }
}
