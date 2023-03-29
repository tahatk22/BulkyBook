using BulkyBook.DataAccess.Repositary.IRepositary;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repositary
{
    public class CategoryRepositary : Repositary<Category>, ICategoryRepositary
    {
        private readonly ApplicationDbContext dbContext;

        public CategoryRepositary( ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
        public void Save()
        {
            dbContext.SaveChanges();
        }

        public void Update(Category obj)
        {
            dbContext.Categories.Update(obj);
        }
    }
}
