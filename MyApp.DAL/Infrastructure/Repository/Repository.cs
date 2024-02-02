
using Microsoft.EntityFrameworkCore;
using MyApp.DAL.Infrastructure.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.DAL.Infrastructure.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private DbSet<T> _dbSet;
        public Repository(ApplicationDbContext context)
        {
            _context = context;
           //context.Products.Include(x => x.Category);
            _dbSet =_context.Set<T>();//Set<T> properties ko show karha ha jo DbSet ma property ha
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? predicate=null, string? includeProperties = null)
        {
            IQueryable<T> query = _dbSet;//many to many relationship
            if(predicate != null)
            {
                query = query.Where(predicate);
            }
            if (includeProperties != null)
            {
                foreach(var item in includeProperties.Split(new char[] {','},StringSplitOptions.RemoveEmptyEntries))
                {
                  query = query.Include(item);
                }
            }
           return query.ToList();
        }

        public T GetT(Expression<Func<T, bool>> predicate, string? includeProperties = null)
        {
            IQueryable<T> query=_dbSet;
            query = query.Where(predicate);
            if (includeProperties != null)
            {
                foreach (var item in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(item);
                }
            }
            return query.FirstOrDefault();// where show lambda expression jo boolen value return karta ha
            
        }
        
    }
}
