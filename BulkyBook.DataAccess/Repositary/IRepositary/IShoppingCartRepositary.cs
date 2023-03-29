using BulkyBook.Models.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repositary.IRepositary
{
    public interface IShoppingCartRepositary : IRepositary<ShoppingCart>
    {
        int InCrementCount(ShoppingCart cart);
        int DecrementCount(ShoppingCart cart);
        void Save();
    }
}
