using BulkyBook.DataAccess.Repositary.IRepositary;
using BulkyBook.Models.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repositary
{
    public class ShoppingCartRepositary : Repositary<ShoppingCart>, IShoppingCartRepositary
    {
        private readonly ApplicationDbContext dbContext;

        public ShoppingCartRepositary(ApplicationDbContext dbContext) : base(dbContext) 
        {
            this.dbContext = dbContext;
        }

        public int DecrementCount(ShoppingCart cart)
        {
            cart.Count -= 1;
            return cart.Count;
        }

        public int InCrementCount(ShoppingCart cart)
        {
            cart.Count += 1;
            return cart.Count;
        }

        public void Save()
        {
            dbContext.SaveChanges();
        }
    }
}
