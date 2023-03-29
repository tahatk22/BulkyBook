using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BulkyBook.DataAccess.Repositary.IRepositary;
using Microsoft.EntityFrameworkCore;

namespace BulkyBook.DataAccess.Repositary
{
    public class Repositary<T> : IRepositary<T> where T : class
    {
        private readonly ApplicationDbContext dbContext;
        internal DbSet<T> Set;  

        public Repositary(ApplicationDbContext dbContext) 
        {
            this.dbContext = dbContext;
            this.Set = dbContext.Set<T>();
        }
        public void Add(T obj)
        {
            Set.Add(obj);
        }

        public IEnumerable<T> GetMany(Expression<Func<T, bool>>? Filter = null, string? Props = null)
        {
            IQueryable<T> Data = Set;
            if (Filter is not null)
            {
                Data = Data.Where(Filter);
            }
            if (Props != null)
            {
                foreach (var item in Props.Split(','))
                {
                    Data = Data.Include(item);
                }
            }
            return Data.ToList();
        }

        public T GetOne(Expression<Func<T, bool>> Filter , string? Props = null)
        {
            IQueryable<T> Data = Set;
            Data = Data.Where(Filter);
            if (Props != null)
            {
                foreach (var item in Props.Split(','))
                {
                    Data = Data.Include(item);
                }
            }
            return Data.FirstOrDefault();
        }

        public void Remove(T obj)
        {
            Set.Remove(obj);
        }

        public void RemoveRange(IEnumerable<T> obj)
        {
            Set.RemoveRange(obj);
        }
    }
}
