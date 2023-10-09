using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureTable
{
    public class Customer : TableEntity
    {
        public string customername {  get; set; }
        public Customer(string _customername, string _city, string _customerid) 
        {
            PartitionKey = _city;
            RowKey = _customerid;
            customername = _customername;
        }
    }
}
