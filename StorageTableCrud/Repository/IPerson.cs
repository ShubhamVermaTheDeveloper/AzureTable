using Azure.Data.Tables.Models;
using Azure.Data.Tables;
using StorageTableCrud.Models;

namespace StorageTableCrud.Repository
{
    public interface IPerson
    {
        TableItem CreateTable();
        public void InsertRecord(PersonModel person);
        public List<PersonModel> ReadRecord();
        public TableEntity UpdateRecord(PersonModel person, string RowKey);
        public string GenerateUniqueKey();
        public void DeleteRecord(string rowKey);
        public string Encrypt(string plainText);
        public string Decrypt(string cipherText);
        public byte[] EncryptImg(byte[] data);
        public byte[] DecryptImg(byte[] data);
        public PersonModel GetRecordByRowKey(string rowKey);



    }
}
