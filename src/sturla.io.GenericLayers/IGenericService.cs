using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace sturla.io.GenericLayers
{
	public interface IGenericService<T>
	{
		Task<GenericResult<T>> GetByIdAsync(int id);
		Task<GenericResult<T>> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes);

		Task<GenericResult<IEnumerable<T>>> GetByMatchAsync(Expression<Func<T, bool>> match);
		Task<GenericResult<IEnumerable<T>>> GetByMatchAsync(Expression<Func<T, bool>> match, params Expression<Func<T, object>>[] includes);

		Task<GenericResult<T>> GetOneByMatchAsync(Expression<Func<T, bool>> match);
		Task<GenericResult<IEnumerable<T>>> GetAllAsync();

		Task<GenericResult<T>> AddAsync(T dto);
		Task<GenericResult<T>> UpdateAsync(T dto);
		Task<GenericResult<int>> DeleteAsync(int id);
		Task<GenericResult<int>> CountAsync();
		Task<GenericResult<int>> SaveAsync();

		void Dispose();
	}
}