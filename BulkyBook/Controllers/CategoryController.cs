using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repositary.IRepositary;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBook.Controllers
{
    [Authorize(Roles =SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepositary category;

        public CategoryController(ICategoryRepositary category ) 
        {
            this.category = category;
        }
        public IActionResult Index()
        {
            var Category = category.GetMany();
            return View(Category);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("DisplayOrder", "The Dispaly Orde Can Not Have The Same Value As Name");
            }
            if (ModelState.IsValid)
            {
                category.Add(obj);
                category.Save();
                TempData["Success"] = "Created Successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }
        public IActionResult Edit(int id) 
        {
            if (id == 0)
            {
                return NotFound();
            }
            var Cat = category.GetOne(x => x.Id == id);
            return View(Cat);
        }
        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("DisplayOrder", "The Dispaly Orde Can Not Have The Same Value As Name");
            }
            if (ModelState.IsValid)
            {
                category.Update(obj);
                category.Save();
                TempData["Success"] = "Edited Successfully";
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
            var Cat = category.GetOne(x => x.Id == id);
            return View(Cat);
        }
        [HttpPost,ActionName("Delete")]
        public IActionResult DeletePost(int id)
        {
            var Data = category.GetOne(x => x.Id == id);
            if (Data == null)
            {
                return NotFound();
            }
            category.Remove(Data);
            category.Save();
            TempData["Success"] = "Deleted Successfully";
            return RedirectToAction("Index");
        }
    }
}
