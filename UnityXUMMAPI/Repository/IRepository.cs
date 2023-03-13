using System.Linq.Expressions;
using UnityXUMMAPI.Entities;

namespace UnityXUMMAPI.Repository
{
    public interface IRepository<T> where T : BaseEntity
    {
        IQueryable<T> GetAll();

        //Task<T> GetById(string id);

        Task Create(T entity);

        Task Update(Guid id, T entity);

        Task Delete(Guid id);
    }
}
