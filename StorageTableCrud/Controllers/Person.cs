using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using StorageTableCrud.Models;
using StorageTableCrud.Options;
using StorageTableCrud.Repository;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace StorageTableCrud.Controllers
{
    public class Person : Controller
    {
        private readonly IPerson _person;
        private readonly AzureOptions _azureOptions;

       

        public Person(IPerson person, IOptions<AzureOptions> azureOptions)
        {
            _person = person;
            _azureOptions = azureOptions.Value;
        }


        public IActionResult Index()
        {
            //_person.CreateTable();
            //_person.InsertRecord();
           
            List<PersonModel> list = _person.ReadRecord();
            return View(list);
        }



        
    }
}
