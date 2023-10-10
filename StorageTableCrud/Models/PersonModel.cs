using Azure;
using Azure.Data.Tables;
using System.ComponentModel;

namespace StorageTableCrud.Models
{
    public class PersonModel :  ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public string FirstName { get; set; }
        public string City { get; set; }
    }
}
