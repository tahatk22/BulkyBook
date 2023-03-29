using BulkyBook.DataAccess.Repositary.IRepositary;
using BulkyBook.Models.VM;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyBook.ViewComponents
{
   public class ShoppingCartViewComponent : ViewComponent
    {
        private readonly IShoppingCartRepositary shoppingCart;

        public ShoppingCartViewComponent(IShoppingCartRepositary shoppingCart)
        {
            this.shoppingCart = shoppingCart;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                if (HttpContext.Session.GetInt32(SD.SessionCart) != null)
                {
                    return View(HttpContext.Session.GetInt32(SD.SessionCart));
                }
                else
                {
                    HttpContext.Session.SetInt32(SD.SessionCart,
                        shoppingCart.GetMany(u => u.ApplicationUserId == claim.Value).ToList().Count);
                    return View(HttpContext.Session.GetInt32(SD.SessionCart));
                }
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }
        }
    }
}
