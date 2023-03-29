using BulkyBook.DataAccess.Repositary.IRepositary;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repositary
{
    public class OrderDetailRepositary : Repositary<OrderDetail>, IOrderDetailRepositary
    {
        private readonly ApplicationDbContext dbContext;

        public OrderDetailRepositary(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
        public void Save()
        {
            dbContext.SaveChanges();
        }

        public void Update(OrderDetail obj)
        {
            dbContext.OrderDetails.Update(obj);
        }
    }
}
