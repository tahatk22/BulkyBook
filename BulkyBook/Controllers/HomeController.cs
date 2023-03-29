using BulkyBook.DataAccess.Repositary.IRepositary;
using BulkyBook.Models;
using BulkyBook.Models.VM;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyBook.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductRepositary product;
        private readonly IShoppingCartRepositary shoppingCart;

        public HomeController(ILogger<HomeController> logger , IProductRepositary product , IShoppingCartRepositary shoppingCart)
        {
            _logger = logger;
            this.product = product;
            this.shoppingCart = shoppingCart;
        }

        public IActionResult Index()
        {
            var Product = product.GetMany(Props: "Category,CoverType");
            return View(Product);
        }
        public IActionResult Details(int ProductId)
        {
            ShoppingCart cart = new ShoppingCart()
            {
                Count = 1,
                ProductId = ProductId,
                Product = product.GetOne(x => x.Id == ProductId, Props: "Category,CoverType")
            };
            return View(cart);
        }
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart cart)
        {
            var ClaimsIdentitY = (ClaimsIdentity)User.Identity;
            var claim = ClaimsIdentitY.FindFirst(ClaimTypes.NameIdentifier);
            cart.ApplicationUserId = claim.Value;
            var cartFromDb = shoppingCart.GetOne(x => x.ApplicationUserId == claim.Value && x.ProductId == cart.ProductId);
            if (cartFromDb == null)
            {
                shoppingCart.Add(cart);
                var count = shoppingCart.GetMany(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count + 1;
                HttpContext.Session.SetInt32(SD.SessionCart, count);
            }
            else 
            {
                cartFromDb.Count = cart.Count;
            }
            shoppingCart.Save();
            return RedirectToAction("Index");
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}