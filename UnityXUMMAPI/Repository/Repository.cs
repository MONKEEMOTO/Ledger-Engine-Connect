using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UnityXUMMAPI.Entities;

namespace UnityXUMMAPI.Repository
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly  ApplicationDbContext _appContext;
        private DbSet<T> entities;
        string errorMessage = string.Empty;
        public Repository(ApplicationDbContext appContext)
        {
            _appContext = appContext;
            entities = _appContext.Set<T>();
        }

        public async Task Create(T entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");

            await entities.AddAsync(entity);
            await _appContext.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            if (id == null) throw new ArgumentNullException("entity");


            var entity = entities.SingleOrDefault(s => s.Id == id);
            entities.Remove(entity);
     
            await _appContext.SaveChangesAsync();
        }

        public IQueryable<T> GetAll()
        {
            return _appContext.Set<T>().AsNoTracking();
        }

        //public async Task<T> GetById(string id)
        //{
        //    return await _appContext.Set<T>()
        //        .AsNoTracking()
        //        .FirstOrDefaultAsync(e => e.Id == id);
        //}

        public async Task Update(Guid id, T entity)
        {
            entities.Update(entity);
            await _appContext.SaveChangesAsync();
        }
    }
}
