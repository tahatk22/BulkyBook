using BulkyBook.DataAccess.Helper;
using BulkyBook.DataAccess.Repositary.IRepositary;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;

namespace BulkyBook.Controllers
{

    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IProductRepositary product;
        private readonly ICategoryRepositary category;
        private readonly ICoverRepositary cover;

        public ProductController(IProductRepositary product , ICategoryRepositary category , ICoverRepositary cover)
        {
            this.product = product;
            this.category = category;
            this.cover = cover;
        }
        public IActionResult Index()
        {
            var Product = product.GetMany(Props : "Category");
            return View(Product);
        }
        public IActionResult UpSert(int? id)
        {
            Product prod = new Product();
            if (id == null || id == 0)
            {
                ViewBag.CategoryList  = new SelectList(category.GetMany() , "Id" , "Name");
                ViewData["CoverList"] = new SelectList(cover.GetMany(), "Id", "Name");
                return View(prod);
            }
            else
            {
                ViewBag.CategoryList = new SelectList(category.GetMany(), "Id", "Name");
                ViewData["CoverList"] = new SelectList(cover.GetMany(), "Id", "Name");
                var Data = product.GetOne(x => x.Id == id);
                return View(Data);
            }
        }
        [HttpPost]
        public IActionResult UpSert(Product obj , IFormFile? File)
        {
            if (ModelState.IsValid)
            {
                if (File != null)
                {
                    if (obj.ImageUrl != null)
                    {
                        FileUpload.DeleteFile("/wwwroot/Imgs/Products", obj.ImageUrl);
                    }
                    obj.ImageUrl = FileUpload.Upload("/wwwroot/Imgs/Products", File);
                }
                if (obj.Id != 0)
                {
                    product.Update(obj);
                    TempData["Success"] = "Edited Successfully";
                }
                else
                {
                    product.Add(obj);
                    TempData["Success"] = "Added Successfully";
                }
                product.Save();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryList = new SelectList(category.GetMany(), "Id", "Name" , obj.CategoryId);
            ViewData["CoverList"] = new SelectList(cover.GetMany(), "Id", "Name" , obj.CoverTypeId);
            return View(obj);
        }
        public IActionResult Delete(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }
            ViewBag.CategoryList = new SelectList(category.GetMany(), "Id", "Name");
            ViewData["CoverList"] = new SelectList(cover.GetMany(), "Id", "Name");
            var Product = product.GetOne(x => x.Id == id);
            return View(Product);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int id)
        {
            var Data = product.GetOne(x => x.Id == id);
            if (Data == null)
            {
                return NotFound();
            }
            FileUpload.DeleteFile("/wwwroot/Imgs/Products", Data.ImageUrl);
            product.Remove(Data);
            product.Save();
            TempData["Success"] = "Deleted Successfully";
            return RedirectToAction("Index");
        }
    }
}
