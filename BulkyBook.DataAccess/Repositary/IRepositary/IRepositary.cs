using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repositary.IRepositary
{
    public interface IRepositary<T> where T : class
    {
        T GetOne(Expression<Func<T, bool>> Filter, string? Props = null);
        IEnumerable<T> GetMany(Expression<Func<T, bool>>? Filter = null, string? Props = null);
        void Add(T obj);
        void Remove(T obj);
        void RemoveRange(IEnumerable<T> obj);
    }
}
