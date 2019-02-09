using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Discord.WordGraffiti.Models.Repositories
{
    public interface IRepository<T>
    {
        Task<T> GetOneByFilter(Expression<Func<T, object>> filter, object value);
        Task<IEnumerable<T>> GetManyByFilter<t>(Expression<Func<T, t>> filter, IEnumerable<t> value);
        Task<IEnumerable<T>> GetAll();
        Task Upsert(T item);
        Task Delete(T item);
    }
}
