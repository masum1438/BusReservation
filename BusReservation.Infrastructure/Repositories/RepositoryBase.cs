using BusReservation.Application.Interfaces;
using BusReservation.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusReservation.Infrastructure.Repositories
{
    public class RepositoryBase<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _db;
        protected readonly DbSet<T> _set;

        public RepositoryBase(AppDbContext db)
        {
            _db = db;
            _set = db.Set<T>();
        }

        public virtual async Task AddAsync(T entity) => await _set.AddAsync(entity);
        public virtual void Update(T entity) => _set.Update(entity);
        public virtual void Remove(T entity) => _set.Remove(entity);
        public virtual async Task<T?> GetByIdAsync(Guid id) => await _set.FindAsync(id);
        public virtual async Task<List<T>> ListAsync() => await _set.ToListAsync();
        public virtual async Task SaveChangesAsync() => await _db.SaveChangesAsync();
    }
}
