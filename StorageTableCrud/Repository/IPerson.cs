using Azure.Data.Tables.Models;
using Azure.Data.Tables;
using StorageTableCrud.Models;

namespace StorageTableCrud.Repository
{
    public interface IPerson
    {
        TableItem CreateTable();
        public void InsertRecord();
        public List<PersonModel> ReadRecord();
    }
}
