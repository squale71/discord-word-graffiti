using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Discord.WordGraffiti.DAL.Repositories
{
    public interface IRepository<T>
    {
        Task<T> GetByID(int ID);
        Task<IEnumerable<T>> GetAll();
        Task<T> Upsert(T entity);
        Task Delete(T entity);
    }
}
