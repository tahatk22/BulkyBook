using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repositary.IRepositary
{
    public interface IOrderHeaderRepositary : IRepositary<OrderHeader>
    {
        void Save();
        void Update(OrderHeader obj);
        void UpdateStatus(int id, string OrderStatus, string? PaymentStatus = null);
        void UpdateStripePaymentID(int id, string sessionId, string paymentItentId);
    }
}
