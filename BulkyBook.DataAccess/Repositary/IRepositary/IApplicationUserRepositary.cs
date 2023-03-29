using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repositary.IRepositary
{
    public interface IApplicationUserRepositary : IRepositary<ApplicationUser>
    {
        void Save();
    }
}
