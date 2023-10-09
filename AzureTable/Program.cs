using Microsoft.Azure.Cosmos.Table;
using System;

namespace AddEntityToTheTable
{
    class Program
    {
        private static string connection_stri = "DefaultEndpointsProtocol=https;AccountName=shubham12;AccountKey=AX7b/yGFhfb4f8/eMqX724KfvPlf9ekENK4cBe9wPA5+7FAKc+EzPvubBi42KuWMWGoRZ16GT17z+AStzzbKgA==;EndpointSuffix=core.windows.net";
        private static string table_name = "Customer";
        static void Main(string[] args)
        {
            CloudStorageAccount _accout = CloudStorageAccount.Parse(connection_stri);
           
            CloudTableClient _table_client = _accout.CreateCloudTableClient();

            CloudTable _table = _table_client.GetTableReference(table_name);

            _table.CreateIfNotExists();

            List<Customer> _customers = new List<Customer>()
            {
                new Customer("UserB", "Chicago", "C2"),
                new Customer("UserC", "Chicago", "C3"),
                new Customer("UserD", "Chicago", "C4"),
            };

            TableBatchOperation _operation = new TableBatchOperation();
            foreach(Customer _customer in _customers)
            {
                _operation.Insert(_customer);
            }

            TableBatchResult _result = _table.ExecuteBatch(_operation);

            Console.WriteLine("Entity is added");

            Console.ReadKey();
        }
    }
}