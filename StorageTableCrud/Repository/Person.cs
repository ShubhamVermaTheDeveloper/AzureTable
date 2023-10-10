using Azure;
using System;
using Azure.Data.Tables;
using StorageTableCrud.Options;
using Azure.Data.Tables.Models;
using Microsoft.Extensions.Options;
using StorageTableCrud.Repository;
using System.Collections.Concurrent;
using StorageTableCrud.Models;
using System.Xml;
using System.Security.Cryptography;
using System.Text;
using MessagePack.Internal;
using Microsoft.IdentityModel;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace StorageTableCrud.Repository
{
    public class Person : IPerson
    {
        private readonly AzureOptions _azureOptions;
        static string partitionKey = "India";
        private readonly static string key = "shubhamvermadotnetdeveloper2023k";
       




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
            var entity = new TableEntity(partitionKey, GenerateUniqueKey())
            {
                { "Name", Encrypt(person.Name) },
                { "FatherName", Encrypt(person.FatherName) },
                { "Age", Encrypt(person.Age) },
                { "City", Encrypt(person.City) }
            };
            table.AddEntity(entity);
        }


        public List<PersonModel> ReadRecord()
        {
            TableClient _tableClient = new TableClient(_azureOptions.ConnectionString, _azureOptions.TableName);
            List<PersonModel> entities = _tableClient.Query<PersonModel>().ToList();
            foreach(var entity in entities)
            {
                entity.Name = Decrypt(entity.Name).ToString();
                entity.FatherName = Decrypt(entity.FatherName);
                entity.Age = Decrypt(entity.Age);
                entity.City = Decrypt(entity.City);
            }
            
            return entities;
        }

        public void DeleteRecord(string rowKey)
        {
            TableClient _tableClient = new TableClient(_azureOptions.ConnectionString, _azureOptions.TableName);
            PersonModel entityToDelete = _tableClient.GetEntity<PersonModel>(partitionKey, rowKey);
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
                aes.Key = Encoding.UTF8.GetBytes(key);
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


        public string Decrypt(string text)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(text);
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (MemoryStream ms = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(ms))
                        {
                            return sr.ReadToEnd();
                        }
                    }

                }
            }
        }





    }
}



