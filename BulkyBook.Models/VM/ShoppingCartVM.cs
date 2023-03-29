using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models.VM
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShoppingCart> ShoppingList { get; set; }
        public OrderHeader OrderHeader { get; set; }
    }
}
