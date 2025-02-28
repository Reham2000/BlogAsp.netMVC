﻿using Microsoft.EntityFrameworkCore;

namespace MyBlog.infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {

        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task AddAsync(T entity, Action<string> logAction)
        {
            logAction?.Invoke($"Adding {typeof(T).Name} to database!");
            await _dbSet.AddAsync( entity );
            await _context.SaveChangesAsync();  
        }

        public async Task DeleteAsync(int id, Action<string> logAction)
        {
            var entity = await _dbSet.FindAsync(id);
            if(entity is not null)
            {
                logAction?.Invoke($"Deleting {typeof(T).Name} with Id {id}");
                _dbSet.Remove( entity );
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task UpdateAstnc(T entity, Action<string> logAction)
        {
            logAction?.Invoke($"Updating {typeof(T).Name} in database!");
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
