namespace StorageTableCrud.Options
{
    public class AzureOptions
    {  
        public string ResourceGroup { get; set; }
        public string Account { get; set; }
        public string TableName { get; set; }
        public string ConnectionString { get; set; }
        public string EncryptionKey { get; set; }
        public string PartitionKey { get; set; }
    }
}
