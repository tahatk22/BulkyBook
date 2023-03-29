using BulkyBook.DataAccess.Repositary.IRepositary;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repositary
{
    public class ProductRepositary : Repositary<Product>, IProductRepositary
    {
        private readonly ApplicationDbContext dbContext;

        public ProductRepositary(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
        public void Save()
        {
            dbContext.SaveChanges();
        }

        public void Update(Product obj)
        {
            dbContext.Products.Update(obj);
        }
    }
}
#region Update
//var objFromDb = dbContext.Products.FirstOrDefault(x => x.Id == obj.Id);
//if (objFromDb != null)
//{
//    objFromDb.Title = obj.Title;
//    objFromDb.ISBN = obj.ISBN;
//    objFromDb.Price = obj.Price;
//    objFromDb.Price50 = obj.Price50;
//    objFromDb.ListPrice = obj.ListPrice;
//    objFromDb.Price100 = obj.Price100;
//    objFromDb.Description = obj.Description;
//    objFromDb.CategoryId = obj.CategoryId;
//    objFromDb.Author = obj.Author;
//    objFromDb.CoverTypeId = obj.CoverTypeId;
//    if (obj.ImageUrl != null)
//    {
//        objFromDb.ImageUrl = obj.ImageUrl;
//    }
//}
#endregion
