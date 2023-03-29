using BulkyBook.DataAccess.Repositary.IRepositary;
using BulkyBook.Models;
using BulkyBook.Models.VM;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;
using static System.Net.WebRequestMethods;

namespace BulkyBook.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IShoppingCartRepositary shoppingCart;
        private readonly IApplicationUserRepositary applicationUser;
        private readonly IOrderDetailRepositary orderDetail;
        private readonly IOrderHeaderRepositary orderHeader;

        public CartController(IShoppingCartRepositary shoppingCart , IApplicationUserRepositary applicationUser , IOrderDetailRepositary orderDetail , IOrderHeaderRepositary orderHeader)
        {
            this.shoppingCart = shoppingCart;
            this.applicationUser = applicationUser;
            this.orderDetail = orderDetail;
            this.orderHeader = orderHeader;
        }
        public IActionResult Index()
        {
            var ClaimsIdentitY = (ClaimsIdentity)User.Identity;
            var claim = ClaimsIdentitY.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartVM sh = new ShoppingCartVM()
            {
                ShoppingList = shoppingCart.GetMany(x => x.ApplicationUserId == claim.Value, Props: "Product"),
                OrderHeader = new()
            };
            foreach (var item in sh.ShoppingList)
            {
                item.Price = GetPriceBasedOnQuantity(item.Count, item.Product.Price, item.Product.Price50, item.Product.Price100);
                sh.OrderHeader.OrderTotal += (item.Price * item.Count);
            }
            return View(sh);
        }
        public IActionResult Summary()
        {
            var ClaimsIdentitY = (ClaimsIdentity)User.Identity;
            var claim = ClaimsIdentitY.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartVM sh = new ShoppingCartVM()
            {
                ShoppingList = shoppingCart.GetMany(x => x.ApplicationUserId == claim.Value, Props: "Product"),
                OrderHeader = new()
            };
            sh.OrderHeader.ApplicationUser = applicationUser.GetOne(x => x.Id == claim.Value);

            sh.OrderHeader.Name = sh.OrderHeader.ApplicationUser.Name;
            sh.OrderHeader.PhoneNumber = sh.OrderHeader.ApplicationUser.PhoneNumber;
            sh.OrderHeader.StreetAddress = sh.OrderHeader.ApplicationUser.StreetAddress;
            sh.OrderHeader.City = sh.OrderHeader.ApplicationUser.City;
            sh.OrderHeader.State = sh.OrderHeader.ApplicationUser.State;
            sh.OrderHeader.PostalCode = sh.OrderHeader.ApplicationUser.PostalCode;

            foreach (var item in sh.ShoppingList)
            {
                item.Price = GetPriceBasedOnQuantity(item.Count, item.Product.Price, item.Product.Price50, item.Product.Price100);
                sh.OrderHeader.OrderTotal += (item.Price * item.Count);
            }
            return View(sh);
        }
        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPost(ShoppingCartVM shopping)
        {
            var ClaimsIdentitY = (ClaimsIdentity)User.Identity;
            var claim = ClaimsIdentitY.FindFirst(ClaimTypes.NameIdentifier);

            shopping.ShoppingList = shoppingCart.GetMany(x => x.ApplicationUserId == claim.Value, Props: "Product");

            shopping.OrderHeader.OrderDate = System.DateTime.Now;
            shopping.OrderHeader.ApplicationUserId = claim.Value;

            foreach (var item in shopping.ShoppingList)
            {
                item.Price = GetPriceBasedOnQuantity(item.Count, item.Product.Price, item.Product.Price50, item.Product.Price100);
                shopping.OrderHeader.OrderTotal += (item.Price * item.Count);
            }
            ApplicationUser user = applicationUser.GetOne(x => x.Id == claim.Value);
            if (user.CompanyId.GetValueOrDefault() == 0)
            {
                shopping.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                shopping.OrderHeader.OrderStatus = SD.StatusPending;
            }
            else
            {
                shopping.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
                shopping.OrderHeader.OrderStatus = SD.StatusApproved;
            }
            orderHeader.Add(shopping.OrderHeader);
            orderHeader.Save();
            foreach (var item in shopping.ShoppingList)
            {
                OrderDetail detail = new OrderDetail()
                {
                    ProductId = item.ProductId,
                    OrderHeaderId = shopping.OrderHeader.Id,
                    Price = item.Price,
                    Count = item.Count
                };
                orderDetail.Add(detail);
                orderDetail.Save();
            }

            if (user.CompanyId.GetValueOrDefault() == 0)
            {
                //stripeSettings
                var domain = "http://localhost:62910/";
                var options = new SessionCreateOptions
                {
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                    SuccessUrl = domain + $"cart/OrderConfirmation?id={shopping.OrderHeader.Id}",
                    CancelUrl = domain + $"cart/index",
                };
                foreach (var item in shopping.ShoppingList)
                {
                    var SessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Title,
                            },
                        },
                        Quantity = item.Count,
                    };
                    options.LineItems.Add(SessionLineItem);
                }
                var service = new SessionService();
                Session session = service.Create(options);
                orderHeader.UpdateStripePaymentID(shopping.OrderHeader.Id, session.Id, session.PaymentIntentId);
                orderHeader.Save();
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }
            else
            {
                return RedirectToAction("OrderConfirmation" , "Cart" , new {id = shopping.OrderHeader.Id});
            }
        }
        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader OH = orderHeader.GetOne(x => x.Id == id);
            if (OH.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {
                var services = new SessionService();
                Session session = services.Get(OH.SessionId);
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    orderHeader.UpdateStripePaymentID(id, OH.SessionId, session.PaymentIntentId);
                    orderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                    orderHeader.Save();
                }
            }
            List<ShoppingCart> S = shoppingCart.GetMany( x => x.ApplicationUserId == OH.ApplicationUserId).ToList();
            HttpContext.Session.Clear();
            shoppingCart.RemoveRange(S);
            shoppingCart.Save();
            return View(id);
        }
        public IActionResult plus(int cartId)
        {
            var cart = shoppingCart.GetOne(x => x.Id == cartId);
            shoppingCart.InCrementCount(cart);
            shoppingCart.Save();
            return RedirectToAction("Index");
        }
        public IActionResult minus(int cartId)
        {
            var cart = shoppingCart.GetOne(x => x.Id == cartId);
            if (cart.Count <= 1)
            {
                shoppingCart.Remove(cart);
                shoppingCart.Save();
                var count = shoppingCart.GetMany(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
                HttpContext.Session.SetInt32(SD.SessionCart, count);
            }
            else
            {
                shoppingCart.DecrementCount(cart);
                shoppingCart.Save();
            }
            return RedirectToAction("Index");
        }
        public IActionResult remove(int cartId)
        {
            var cart = shoppingCart.GetOne(x => x.Id == cartId);
            shoppingCart.Remove(cart);
            shoppingCart.Save();
            var count = shoppingCart.GetMany(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
            HttpContext.Session.SetInt32(SD.SessionCart, count);
            return RedirectToAction("Index");
        }

        private double GetPriceBasedOnQuantity(double quantity,double price , double price50 , double price100)
        {
            if (quantity <= 50)
            {
                return price;
            }
            else
            {
                if (quantity <= 100)
                {
                    return price50;
                }
                return price100;
            }
        }
    }
}
