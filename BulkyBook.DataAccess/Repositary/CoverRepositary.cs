using BulkyBook.DataAccess.Repositary.IRepositary;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repositary
{
    public class CoverRepositary : Repositary<CoverType>, ICoverRepositary
    {
        private readonly ApplicationDbContext dbContext;

        public CoverRepositary(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
        public void Save()
        {
            dbContext.SaveChanges();
        }

        public void Update(CoverType obj)
        {
            dbContext.Covers.Update(obj);
        }
    }
}
