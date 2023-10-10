using Azure.Data.Tables;
using StorageTableCrud.Options;
using Azure.Data.Tables.Models;
using Microsoft.Extensions.Options;
using StorageTableCrud.Models;
using System.Security.Cryptography;
using System.Text;


namespace StorageTableCrud.Repository
{
    public class Person : IPerson
    {
        private readonly AzureOptions _azureOptions;
       
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


        public void InsertRecord(PersonModel person)
        {
            TableServiceClient client = new TableServiceClient(_azureOptions.ConnectionString);
            TableClient table = client.GetTableClient(_azureOptions.TableName);
            var entity = new TableEntity(Encrypt(_azureOptions.PartitionKey), Encrypt(GenerateUniqueKey()))
            {
                { "Name", Encrypt(person.Name) },
                { "FatherName", Encrypt(person.FatherName) },
                { "Age", Encrypt(person.Age) },
                { "City", Encrypt(person.City) },
                { "Image",  EncryptImg(person.Image)}     
        };
            table.AddEntity(entity);
        }


        public TableEntity UpdateRecord(PersonModel person, string RowKey)
        {
            TableServiceClient client = new TableServiceClient(_azureOptions.ConnectionString);
            TableClient table = client.GetTableClient(_azureOptions.TableName);
            PersonModel entity = table.GetEntity<PersonModel>(Encrypt(_azureOptions.PartitionKey), Encrypt(RowKey));

            var updatedEntity = new TableEntity(entity.PartitionKey, entity.RowKey)
            {
                { "Name", Encrypt(person.Name) },
                { "FatherName", Encrypt(person.FatherName) },
                { "Age", Encrypt(person.Age) },
                { "City", Encrypt(person.City) },
                { "Image", EncryptImg(person.Image) }
            };
            updatedEntity.ETag = person.ETag;

            table.UpdateEntity(updatedEntity, ifMatch: updatedEntity.ETag);

            return updatedEntity;
        }




        public List<PersonModel> ReadRecord()
        {
            TableClient _tableClient = new TableClient(_azureOptions.ConnectionString, _azureOptions.TableName);
            List<PersonModel> entities = _tableClient.Query<PersonModel>().ToList();
            foreach(var entity in entities)
            {
                entity.RowKey = Decrypt(entity.RowKey);
                entity.PartitionKey = Decrypt(entity.PartitionKey);
                entity.Name = Decrypt(entity.Name).ToString();
                entity.FatherName = Decrypt(entity.FatherName);
                entity.Age = Decrypt(entity.Age);
                entity.City = Decrypt(entity.City);
                entity.Image = DecryptImg(entity.Image);
            }
            
            return entities;
        }


        public PersonModel GetRecordByRowKey(string rowKey)
        {
            TableClient _tableClient = new TableClient(_azureOptions.ConnectionString, _azureOptions.TableName);
            PersonModel entity = _tableClient.GetEntity<PersonModel>(Encrypt(_azureOptions.PartitionKey), Encrypt(rowKey));

            entity.RowKey = Decrypt(entity.RowKey);
            entity.PartitionKey = Decrypt(entity.PartitionKey);
            entity.Name = Decrypt(entity.Name);
            entity.FatherName = Decrypt(entity.FatherName);
            entity.Age = Decrypt(entity.Age);
            entity.City = Decrypt(entity.City);
            entity.Image = DecryptImg(entity.Image);

            return entity;
        }


        public void DeleteRecord(string rowKey)
        {
            TableClient _tableClient = new TableClient(_azureOptions.ConnectionString, _azureOptions.TableName);
            PersonModel entityToDelete = _tableClient.GetEntity<PersonModel>(Encrypt(_azureOptions.PartitionKey), Encrypt(rowKey));
            _tableClient.DeleteEntity(entityToDelete.PartitionKey, entityToDelete.RowKey);
        }


        public string GenerateUniqueKey()
        {
            Guid guid = Guid.NewGuid();
            byte[] bytes = guid.ToByteArray();
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(bytes);
                string uniqueKey = BitConverter.ToString(hashBytes, 0, 5).Replace("-", "").Substring(0, 5);
                return uniqueKey;
            }
        }


        public string Encrypt(string text)
        {
            byte[] iv = new byte[16];
            byte[] array;
            using(Aes aes = Aes.Create()) {
                aes.Key = Encoding.UTF8.GetBytes(_azureOptions.EncryptionKey);
                aes.IV = iv;
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream ms = new MemoryStream()) 
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {   
                            streamWriter.Write(text);
                        }
                        array = ms.ToArray();
                    }
                }
            }
            return Convert.ToBase64String(array);
        }

        public byte[] EncryptImg(byte[] data)
        {
            byte[] iv = new byte[16];
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(_azureOptions.EncryptionKey);
                aes.IV = iv;
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(data, 0, data.Length);
                        cryptoStream.FlushFinalBlock();
                    }
                    return ms.ToArray();
                }
            }
        }

        public string Decrypt(string text)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(text);
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(_azureOptions.EncryptionKey);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (MemoryStream ms = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cryptoStream))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }

        public byte[] DecryptImg(byte[] data)
        {
            byte[] iv = new byte[16];
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(_azureOptions.EncryptionKey);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (MemoryStream ms = new MemoryStream(data))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (MemoryStream decryptedStream = new MemoryStream())
                        {
                            cryptoStream.CopyTo(decryptedStream);
                            return decryptedStream.ToArray();
                        }
                    }
                }
            }
        }







    }
}



