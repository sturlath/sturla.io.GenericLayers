using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace sturla.io.GenericLayers
{
	public abstract class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
	{
		private readonly DbContext dbContext;

		protected GenericRepository(DbContext dbContext)
		{
			this.dbContext = dbContext;
		}

		public virtual async Task<TEntity> GetByIdAsync(int id)
		{
			return await dbContext.Set<TEntity>().FindAsync(id).ConfigureAwait(false);
		}

		public virtual async Task<TEntity> GetByIdAsync(int id, params Expression<Func<TEntity, object>>[] includes)
		{
			if (includes.Length > 0)
			{
				IQueryable<TEntity> set = includes
				  .Aggregate<Expression<Func<TEntity, object>>, IQueryable<TEntity>>
					(dbContext.Set<TEntity>(), (current, expression) => current.Include(expression));

				return await set.SingleOrDefaultAsync(s => s.Id == id).ConfigureAwait(false);
			}

			return dbContext.Set<TEntity>().Find(id);
		}

		public virtual async Task<TEntity> GetOneByMatchAsync(Expression<Func<TEntity, bool>> match)
		{
			return await dbContext.Set<TEntity>().FirstOrDefaultAsync(match).ConfigureAwait(false);
		}

		/// <summary>
		/// Use sparingly! You are getting EVERY item!
		/// </summary>
		/// <returns>Every set of T</returns>
		public async Task<IEnumerable<TEntity>> GetAllAsync()
		{
			return await dbContext.Set<TEntity>().ToListAsync().ConfigureAwait(false);
		}

		public virtual async Task<IEnumerable<TEntity>> GetByMatchAsync(Expression<Func<TEntity, bool>> match)
		{
			return await dbContext.Set<TEntity>().Where(match).ToListAsync().ConfigureAwait(false);
		}

		public virtual async Task<IEnumerable<TEntity>> GetByMatchAsync(Expression<Func<TEntity, bool>> match, params Expression<Func<TEntity, object>>[] includes)
		{
			if (includes.Length > 0)
			{
				IQueryable<TEntity> set = includes
				  .Aggregate<Expression<Func<TEntity, object>>, IQueryable<TEntity>>
					(dbContext.Set<TEntity>(), (current, expression) => current.Include(expression));

				return await set.Where(match).ToListAsync().ConfigureAwait(false);
			}

			return await dbContext.Set<TEntity>().Where(match).ToListAsync().ConfigureAwait(false);
		}

		public virtual async Task<TEntity> AddAsync(TEntity entity)
		{
			var savedEntity = await dbContext.Set<TEntity>().AddAsync(entity).ConfigureAwait(false);
			var insertId = await SaveAsync().ConfigureAwait(false);
			return savedEntity.Entity;
		}

		public virtual async Task<TEntity> UpdateAsync(TEntity entity)
		{
			if (entity == null)
				return null;

			TEntity exist = dbContext.Set<TEntity>().FirstOrDefault(x => x.Id == entity.Id);

			if (exist == null)
			{
				throw new Exception("Unable to find entity");
			}

			//TODO: Implement in AddAsync
			foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(entity))
			{
				if (descriptor.Name == "CreatedDate" || descriptor.Name == "Timestamp") continue;
				if (descriptor.Name == "ModifiedDate")
				{
					descriptor.SetValue(entity, DateTime.Now);
				}

				TypeDescriptor.GetProperties(exist)[descriptor.Name].SetValue(exist, descriptor.GetValue(entity));
			}

			var entityEntry = dbContext.Set<TEntity>().Update(exist);
			await dbContext.SaveChangesAsync().ConfigureAwait(false);
			return entityEntry.Entity;
		}

		public virtual async Task<int> DeleteAsync(int id)
		{
			TEntity exist = await dbContext.Set<TEntity>().FindAsync(id).ConfigureAwait(false);
			if (exist != null)
			{
				dbContext.Set<TEntity>().Remove(exist);
				return await SaveAsync().ConfigureAwait(false);
			}

			//TODO: Find a better example of the delete method to use!
			return 0;
		}

		public async Task<int> CountAsync()
		{
			return await dbContext.Set<TEntity>().CountAsync().ConfigureAwait(false);
		}

		public virtual async Task<int> SaveAsync()
		{
			return await dbContext.SaveChangesAsync().ConfigureAwait(false);
		}

		// Q:	Why do u need to dispose your repository?
		// A:	Services.AddTransient<IBlogRepository, BlogRepository>(); 
		//		It is a transient.Lifetime services are created each time they are requested.
		//		This lifetime works best for lightweight, stateless services.
		//
		//		Usually if you are working with a micro services with messaging HTTP calls to database may take longer time,
		//		and Entity framework is a Singleton that means it creates a single instance throughout the application 
		//		and it is not thread-safe.It creates the instance for the first time and reuses the same object in the all calls.
		//		
		//		In summary, plain English, for async/await, We need to dispose dataContext for every use and recreate when we need again.
		//		You can’t make two DataContext save operation async the same time because, DataContext is not thread-safe
		//
		private bool disposed = false;

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					dbContext.Dispose();
				}
				disposed = true;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
