using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repositary.IRepositary
{
    public interface ICoverRepositary : IRepositary<CoverType>
    {
        void Save();
        void Update(CoverType obj);
    }
}
