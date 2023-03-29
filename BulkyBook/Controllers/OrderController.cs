using BulkyBook.DataAccess.Repositary.IRepositary;
using BulkyBook.Models;
using BulkyBook.Models.VM;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace BulkyBook.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderHeaderRepositary orderHeader;
        private readonly IOrderDetailRepositary orderDetail;

        public OrderController(IOrderHeaderRepositary orderHeader , IOrderDetailRepositary orderDetail)
        {
            this.orderHeader = orderHeader;
            this.orderDetail = orderDetail;
        }
        public IActionResult Index(string status)
        {
            IEnumerable<OrderHeader> OH;
            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                OH =  orderHeader.GetMany(Props: "ApplicationUser");
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                OH =  orderHeader.GetMany( x => x.ApplicationUserId == claim.Value ,Props: "ApplicationUser");
            }
            switch (status)
            {
                case "pending":
                    OH = OH.Where(u => u.PaymentStatus == SD.PaymentStatusDelayedPayment);
                    break;
                case "inprocess":
                    OH = OH.Where(u => u.OrderStatus == SD.StatusInProcess);
                    break;
                case "completed":
                    OH = OH.Where(u => u.OrderStatus == SD.StatusShipped);
                    break;
                case "approved":
                    OH = OH.Where(u => u.OrderStatus == SD.StatusApproved);
                    break;
                default:
                    break;
            }
            return View(OH);
        }
        public IActionResult Details(int id)
        {
            OrderVM orderVM = new OrderVM()
            {
                OrderHeader = orderHeader.GetOne(u => u.Id == id, Props: "ApplicationUser"),
                OrderDetail = orderDetail.GetMany(u => u.OrderHeaderId == id, Props: "Product"),
            };
            return View(orderVM);
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult UpdateOrderDetail(OrderVM orderVM)
        {
            var orderHEaderFromDb = orderHeader.GetOne(u => u.Id == orderVM.OrderHeader.Id);
            try
            {
                orderHEaderFromDb.Name = orderVM.OrderHeader.Name;
                orderHEaderFromDb.PhoneNumber = orderVM.OrderHeader.PhoneNumber;
                orderHEaderFromDb.StreetAddress = orderVM.OrderHeader.StreetAddress;
                orderHEaderFromDb.City = orderVM.OrderHeader.City;
                orderHEaderFromDb.State = orderVM.OrderHeader.State;
                orderHEaderFromDb.PostalCode = orderVM.OrderHeader.PostalCode;
                if (orderVM.OrderHeader.Carrier != null)
                {
                    orderHEaderFromDb.Carrier = orderVM.OrderHeader.Carrier;
                }
                if (orderVM.OrderHeader.TrackingNumber != null)
                {
                    orderHEaderFromDb.TrackingNumber = orderVM.OrderHeader.TrackingNumber;
                }
                orderHeader.Update(orderHEaderFromDb);
                orderHeader.Save();
                TempData["Success"] = "Order Details Updated Successfully.";
                return RedirectToAction("Details", "Order" , new { id = orderHEaderFromDb.Id});
            }
            catch 
            {
                return RedirectToAction("Details", "Order", new { id = orderHEaderFromDb.Id });
            }
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult StartProcessing(OrderVM orderVM)
        {
            try
            {
                orderHeader.UpdateStatus(orderVM.OrderHeader.Id, SD.StatusInProcess);
                orderHeader.Save();
                TempData["Success"] = "Order Status Updated Successfully.";
                return RedirectToAction("Details", "Order", new { id = orderVM.OrderHeader.Id });
            }
            catch
            {

                return RedirectToAction("Details", "Order", new { id = orderVM.OrderHeader.Id });
            }
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        [ValidateAntiForgeryToken]
        public IActionResult ShipOrder(OrderVM orderVM)
        {
            try
            {
                var OrderHeader = orderHeader.GetOne(u => u.Id == orderVM.OrderHeader.Id);
                OrderHeader.TrackingNumber = orderVM.OrderHeader.TrackingNumber;
                OrderHeader.Carrier = orderVM.OrderHeader.Carrier;
                OrderHeader.OrderStatus = SD.StatusShipped;
                OrderHeader.ShippingDate = DateTime.Now;
                if (OrderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
                {
                    OrderHeader.PaymentDueDate = DateTime.Now.AddDays(30);
                }
                orderHeader.Update(OrderHeader);
                orderHeader.Save();
                TempData["Success"] = "Order Shipped Successfully.";
                return RedirectToAction("Details", "Order", new { id = orderVM.OrderHeader.Id });
            }
            catch
            {

                return RedirectToAction("Details", "Order", new { id = orderVM.OrderHeader.Id });
            }
            
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        [ValidateAntiForgeryToken]
        public IActionResult CancelOrder(OrderVM orderVM)
        {
            try
            {
                var OrderHeader = orderHeader.GetOne(u => u.Id == orderVM.OrderHeader.Id);
                if (OrderHeader.PaymentStatus == SD.PaymentStatusApproved)
                {
                    var options = new RefundCreateOptions
                    {
                        Reason = RefundReasons.RequestedByCustomer,
                        PaymentIntent = OrderHeader.PaymentIntentId
                    };

                    var service = new RefundService();
                    Refund refund = service.Create(options);

                    orderHeader.UpdateStatus(OrderHeader.Id, SD.StatusCancelled, SD.StatusRefunded);
                }
                else
                {
                    orderHeader.UpdateStatus(OrderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);
                }
                orderHeader.Save();
                TempData["Success"] = "Order Cancelled Successfully.";
                return RedirectToAction("Index", "Order");
            }
            catch
            {
                return RedirectToAction("Index", "Order");
            }
            
        }
        [ActionName("Details")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Details_PAY_NOW(OrderVM orderVM)
        {
            orderVM.OrderHeader = orderHeader.GetOne(u => u.Id == orderVM.OrderHeader.Id, Props: "ApplicationUser");
            orderVM.OrderDetail = orderDetail.GetMany(u => u.OrderHeaderId == orderVM.OrderHeader.Id, Props: "Product");

            //stripeSettings
            var domain = "http://localhost:62910/";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + $"order/PaymentConfirmation?id={orderVM.OrderHeader.Id}",
                CancelUrl = domain + $"order/details?id={orderVM.OrderHeader.Id}",
            };
            foreach (var item in orderVM.OrderDetail)
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
            orderHeader.UpdateStripePaymentID(orderVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
            orderHeader.Save();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }
        public IActionResult PaymentConfirmation(int id)
        {
            try
            {
                OrderHeader OrderHeader = orderHeader.GetOne(u => u.Id == id);
                if (OrderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
                {
                    var service = new SessionService();
                    Session session = service.Get(OrderHeader.SessionId);
                    //check the stripe status
                    if (session.PaymentStatus.ToLower() == "paid")
                    {
                        orderHeader.UpdateStatus(id, OrderHeader.OrderStatus, SD.PaymentStatusApproved);
                        orderHeader.Save();
                    }
                }
                return View(id);
            }
            catch 
            {
                return View(id);
            }
        }
    }
}
