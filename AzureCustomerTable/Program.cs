using Azure;
using Azure.Data.Tables;
using Azure.Data.Tables.Models;

namespace AzureCustomerTable
{
    class Program
    {
        static string ConnectionString = "DefaultEndpointsProtocol=https;AccountName=shubham12;AccountKey=AX7b/yGFhfb4f8/eMqX724KfvPlf9ekENK4cBe9wPA5+7FAKc+EzPvubBi42KuWMWGoRZ16GT17z+AStzzbKgA==;EndpointSuffix=core.windows.net";
        static string partitionKey = "India";
        static string rowKey = "Customer";
        public static async Task Main(string[] args)
        {
            await ProcessAsync();
            Console.WriteLine("Program Finished....");
            Console.ReadLine();
        }

        private static async Task ProcessAsync()
        {
            TableServiceClient client = new TableServiceClient(ConnectionString);
            var table = await CreateTable(client);
            InsertRecord(client, table);
            ReadRecord(client, table);
            DeleteTable(client, table);
        }

        private static async Task<TableItem> CreateTable(TableServiceClient client)
        {
            string tableName = "CustomerTable";
            TableItem table = client.CreateTable(tableName);
            return table;
        }

        private static void ReadRecord(TableServiceClient client, TableItem tableItem)
        {
            TableClient table = client.GetTableClient(tableItem.Name);
            Pageable<TableEntity> queryResultFilter = table.Query<TableEntity>(filter: $"PartiionKey eq '{partitionKey}'");
            foreach(TableEntity qEntity in queryResultFilter)
            {
                Console.WriteLine($"{qEntity.GetString("firstName")}: {qEntity.GetString("City")}");
                
            }
        }


        private static void InsertRecord(TableServiceClient client, TableItem tableItem)
        {
            TableClient table = client.GetTableClient(tableItem.Name);
            var entity = new TableEntity(partitionKey, rowKey)
            {
                { "FirstName", "Piyush" },
                { "City", "Mumbai" }
            };
            table.AddEntity(entity);
        }

        private static void DeleteTable(TableServiceClient client, TableItem table)
        {
            client.DeleteTable(table.Name);            
        }


    }
}