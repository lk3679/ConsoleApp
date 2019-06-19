using ConsoleApp1.App_Code;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {

            //更新庫別
            //Warehouses.Update();

            //更新供應商
            //Vendors.Update();

            //更新物料主檔
            //Products.Update();

            //更新庫存數量
            Stocks.Update();

            //更新販售價格
            //Prices.Update();

            Console.ReadLine();

        }
    }
}
