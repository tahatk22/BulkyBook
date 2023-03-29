using BulkyBook.DataAccess.Repositary.IRepositary;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repositary
{
    public class CompanyRepositary : Repositary<Company>, ICompanyRepositary
    {
        private readonly ApplicationDbContext dbContext;

        public CompanyRepositary(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
        public void Save()
        {
            dbContext.SaveChanges();
        }

        public void Update(Company obj)
        {
            dbContext.Companies.Update(obj);
        }
    }
}
