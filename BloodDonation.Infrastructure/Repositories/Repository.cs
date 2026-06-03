using BloodDonation.Application.Interfaces.Repositories;
using BloodDonation.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodDonation.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _context;

        public Repository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(T entity)
        {
           await _context.Set<T>().AddAsync(entity);
        }

        public void DeleteAsync(T entity)
        {
             _context.Set<T>().Remove(entity);
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
          return  await _context.Set<T>().FindAsync(id);
        }

        public void UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity); 
        }
    }
}
