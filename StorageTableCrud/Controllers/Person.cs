using Azure.Data.Tables;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using StorageTableCrud.Models;
using StorageTableCrud.Options;
using StorageTableCrud.Repository;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;
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

        [HttpGet]
        public IActionResult Index()
        {
            List<PersonModel> list = _person.ReadRecord();
            return View(list);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PersonModel person, IFormFile ImageFile)
        {
            if (ImageFile != null && ImageFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    ImageFile.CopyTo(memoryStream);
                    person.Image = memoryStream.ToArray();
                }
            }
            _person.InsertRecord(person);
            return RedirectToAction("Index");
        }

        

        public IActionResult Edit(string rowKey)
        {
            PersonModel person = _person.GetRecordByRowKey(rowKey);
            return View(person);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string rowKey, PersonModel person, IFormFile ImageFile)
        {
            if (ImageFile != null && ImageFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    ImageFile.CopyTo(memoryStream);
                    person.Image = memoryStream.ToArray();
                }
            }
            _person.UpdateRecord(person, rowKey);
            return RedirectToAction("Index");
        }




        public IActionResult Delete(string RowKey)
        {
            _person.DeleteRecord(RowKey);
            return RedirectToAction("Index");
        }


    }
}
