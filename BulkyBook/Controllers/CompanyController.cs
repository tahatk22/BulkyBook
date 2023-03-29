using BulkyBook.DataAccess.Helper;
using BulkyBook.DataAccess.Repositary.IRepositary;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBook.Controllers
{

    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly ICompanyRepositary company;

        public CompanyController(ICompanyRepositary company)
        {
            this.company = company;
        }
        public IActionResult Index()
        {
            var Product = company.GetMany();
            return View(Product);
        }
        public IActionResult UpSert(int? id)
        {
            Company Com = new Company();
            if (id == null || id == 0)
            {
                return View(Com);
            }
            else
            {
                Com = company.GetOne(x => x.Id == id);
                return View(Com);
            }
        }
        [HttpPost]
        public IActionResult UpSert(Company obj)
        {
            if (ModelState.IsValid)
            {
                if (obj.Id != 0)
                {
                    company.Update(obj);
                    TempData["Success"] = "Edited Successfully";
                }
                else
                {
                    company.Add(obj);
                    TempData["Success"] = "Added Successfully";
                }
                company.Save();
                return RedirectToAction("Index");
            }
            return View(obj);
        }
        public IActionResult Delete(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }
            var Product = company.GetOne(x => x.Id == id);
            return View(Product);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int id)
        {
            var Data = company.GetOne(x => x.Id == id);
            if (Data == null)
            {
                return NotFound();
            }
            company.Remove(Data);
            company.Save();
            TempData["Success"] = "Deleted Successfully";
            return RedirectToAction("Index");
        }
    }
}
