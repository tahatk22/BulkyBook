using BulkyBook.DataAccess.Repositary.IRepositary;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBook.Controllers
{

    [Authorize(Roles = SD.Role_Admin)]
    public class CoverController : Controller
    {
        private readonly ICoverRepositary cover;

        public CoverController(ICoverRepositary cover)
        {
            this.cover = cover;
        }
        public IActionResult Index()
        {
            var Cover = cover.GetMany();
            return View(Cover);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(CoverType obj)
        {
            if (ModelState.IsValid)
            {
                cover.Add(obj);
                cover.Save();
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
            var Cat = cover.GetOne(x => x.Id == id);
            return View(Cat);
        }
        [HttpPost]
        public IActionResult Edit(CoverType obj)
        {
            if (ModelState.IsValid)
            {
                cover.Update(obj);
                cover.Save();
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
            var Cat = cover.GetOne(x => x.Id == id);
            return View(Cat);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int id)
        {
            var Data = cover.GetOne(x => x.Id == id);
            if (Data == null)
            {
                return NotFound();
            }
            cover.Remove(Data);
            cover.Save();
            TempData["Success"] = "Deleted Successfully";
            return RedirectToAction("Index");
        }
    }
}
