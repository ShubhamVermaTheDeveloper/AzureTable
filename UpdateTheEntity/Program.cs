using Microsoft.Azure.Cosmos.Table;
using UpdateTheEntity;
using System;

namespace UpdateTheEntity
{
    class Program
    {
        private static string connection_stri = "DefaultEndpointsProtocol=https;AccountName=shubham12;AccountKey=AX7b/yGFhfb4f8/eMqX724KfvPlf9ekENK4cBe9wPA5+7FAKc+EzPvubBi42KuWMWGoRZ16GT17z+AStzzbKgA==;EndpointSuffix=core.windows.net";
        private static string table_name = "Customer";
        private static string partition_key = "Chicago";
        private static string row_key = "C2";

        static void Main(string[] args)
        {
            CloudStorageAccount _accout = CloudStorageAccount.Parse(connection_stri);

            CloudTableClient _table_client = _accout.CreateCloudTableClient();

            CloudTable _table = _table_client.GetTableReference(table_name);

            Customer _customer = new Customer("UserE", partition_key, row_key);

            TableOperation _operation = TableOperation.InsertOrMerge(_customer);

            TableResult _result = _table.Execute(_operation);

            Console.WriteLine("Cutomer Information is updated");
           
            Console.ReadKey();
        }
    }
}