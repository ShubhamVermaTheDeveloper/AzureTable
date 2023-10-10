using Azure;
using Azure.Data.Tables;
using StorageTableCrud.Options;
using Azure.Data.Tables.Models;
using Microsoft.Extensions.Options;
using StorageTableCrud.Repository;
using System.Collections.Concurrent;
using StorageTableCrud.Models;
using System.Xml;

namespace StorageTableCrud.Repository
{
    public class Person : IPerson
    {
        private readonly AzureOptions _azureOptions;
        static string partitionKey = "India";
        static string rowKey = "P4";
        



        public Person(IOptions<AzureOptions> azureOptions)
        {
            _azureOptions = azureOptions.Value;
        }

        public TableItem CreateTable()
        {
            TableServiceClient client = new TableServiceClient(_azureOptions.ConnectionString);
            TableItem table = client.CreateTableIfNotExists(_azureOptions.TableName);
            return table;
        }


        public void InsertRecord()
        {
            TableServiceClient client = new TableServiceClient(_azureOptions.ConnectionString);
            TableClient table = client.GetTableClient(_azureOptions.TableName);
            var entity = new TableEntity(partitionKey, rowKey)
            {
                { "FirstName", "Piyush" },
                { "City", "Mumbai" }
            };
            table.AddEntity(entity);
        }


        public List<PersonModel> ReadRecord()
        {
            TableClient _tableClient = new TableClient(_azureOptions.ConnectionString, _azureOptions.TableName);
            List<PersonModel> entities = _tableClient.Query<PersonModel>().ToList();
            return entities;
        }
    }
}



