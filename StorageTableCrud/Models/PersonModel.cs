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

        public string Name { get; set; }
        public string FatherName { get; set; }
        public string Age { get; set; }
        public string City { get; set; }
        public byte[] Image { get; set; }
    }
}
