using BulkyBook.DataAccess.Repositary.IRepositary;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repositary
{
    public class ApplicationUserRepositary : Repositary<ApplicationUser>, IApplicationUserRepositary
    {
        private readonly ApplicationDbContext dbContext;

        public ApplicationUserRepositary(ApplicationDbContext dbContext) : base(dbContext) 
        {
            this.dbContext = dbContext;
        }
        public void Save()
        {
           dbContext.SaveChanges();
        }
    }
}
