using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Discord.WordGraffiti.DAL.Repositories
{
    public interface IRepository<T>
    {
        Task<T> GetById(int ID);
        Task<IEnumerable<T>> GetAll();
        Task<T> Insert(T entity);
        Task<T> Update(T entity);
        Task<T> Upsert(T entity);
        Task Delete(T entity);
    }


}
