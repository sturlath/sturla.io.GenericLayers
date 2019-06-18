using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace sturla.io.GenericLayers
{
	/// <summary>
	/// Generic service class that contains most of what you would need for a basic crud stuff!
	/// It talks to the 
	/// </summary>
	/// <typeparam name="Dto">Dto's</typeparam>
	/// <typeparam name="Entity">Entity Framework classes</typeparam>
	public abstract class GenericService<Dto, Entity> : IGenericService<Dto> where Dto : BaseDto where Entity : BaseEntity
	{
		public readonly IGenericRepository<Entity> repository;
		private readonly IMapper mapper;

		protected GenericService(IGenericRepository<Entity> repository, IMapper mapper)
		{
			this.repository = repository;
			this.mapper = mapper;
		}

		public virtual async Task<GenericResult<Dto>> GetByIdAsync(int id)
		{
			try
			{
				Entity result = await repository.GetByIdAsync(id).ConfigureAwait(false);

				if (result == null)
					return new GenericResult<Dto>($"No {typeof(Entity).Name} with id '{id}' found.");

				return new GenericResult<Dto>
				{
					Value = mapper.Map<Dto>(result)
				};
			}
			catch (Exception ex)
			{
				return new GenericResult<Dto>(ex);
			}
		}

		public async Task<GenericResult<Dto>> GetByIdAsync(int id, Expression<Func<Dto, object>>[] includes)
		{
			try
			{
				var entityExpression = mapper.Map<Expression<Func<Entity, object>>[]>(includes);

				Entity result = await repository.GetByIdAsync(id, entityExpression).ConfigureAwait(false);

				if (result == null)
				{
					var message = ($"No {typeof(Entity).Name} with id '{id}' and expressions found.");
					return new GenericResult<Dto>(message);
				}

				return new GenericResult<Dto>
				{
					Value = mapper.Map<Dto>(result)
				};
			}
			catch (Exception ex)
			{
				return new GenericResult<Dto>(ex);
			}
		}

		public virtual async Task<GenericResult<Dto>> AddAsync(Dto dto)
		{
			try
			{
				Entity entity = mapper.Map<Entity>(dto);

				Entity result = await repository.AddAsync(entity).ConfigureAwait(false);

				return new GenericResult<Dto>
				{
					//Note that we could do Value = dto;
					Value = mapper.Map<Dto>(result)
				};
			}
			catch (Exception ex)
			{
				return new GenericResult<Dto>(ex);
			}
		}

		public virtual async Task<GenericResult<int>> CountAsync()
		{
			try
			{
				return new GenericResult<int>
				{
					Value = await repository.CountAsync().ConfigureAwait(false)
				};
			}
			catch (Exception ex)
			{
				return new GenericResult<int>(ex);
			}
		}

		public virtual async Task<GenericResult<int>> DeleteAsync(int id)
		{
			try
			{
				return new GenericResult<int>
				{
					Value = await repository.DeleteAsync(id).ConfigureAwait(false)
				};
			}
			catch (Exception ex)
			{
				return new GenericResult<int>(ex);
			}
		}

		public virtual void Dispose()
		{
			repository.Dispose();
		}

		public virtual async Task<GenericResult<IEnumerable<Dto>>> GetAllAsync()
		{
			try
			{
				IEnumerable<Entity> result = await repository.GetAllAsync().ConfigureAwait(false);

				if (result == null)
				{
					var message = ($"No matching {typeof(Entity).Name} was found");
					return new GenericResult<IEnumerable<Dto>>(message);
				}

				return new GenericResult<IEnumerable<Dto>>
				{
					Value = mapper.Map<IEnumerable<Dto>>(result)
				};
			}
			catch (Exception ex)
			{
				return new GenericResult<IEnumerable<Dto>>(ex);
			}
		}

		public virtual async Task<GenericResult<IEnumerable<Dto>>> GetByMatchAsync(Expression<Func<Dto, bool>> match)
		{
			try
			{
				Expression<Func<Entity, bool>> matchMap = mapper.Map<Expression<Func<Entity, bool>>>(match);

				IEnumerable<Entity> result = await repository.GetByMatchAsync(matchMap).ConfigureAwait(false);

				if (result == null)
				{
					var message = ($"No matching {typeof(Entity).Name} was found.");
					return new GenericResult<IEnumerable<Dto>>(message);
				}

				return new GenericResult<IEnumerable<Dto>>
				{
					Value = mapper.Map<IEnumerable<Dto>>(result)
				};
			}
			catch (Exception ex)
			{
				return new GenericResult<IEnumerable<Dto>>(ex);
			}
		}

		public virtual async Task<GenericResult<IEnumerable<Dto>>> GetByMatchAsync(Expression<Func<Dto, bool>> match, Expression<Func<Dto, object>>[] includes)
		{
			try
			{
				Expression<Func<Entity, bool>> matchMap = mapper.Map<Expression<Func<Entity, bool>>>(match);
				var entityExpression = mapper.Map<Expression<Func<Entity, object>>[]>(includes);

				IEnumerable<Entity> result = await repository.GetByMatchAsync(matchMap, entityExpression).ConfigureAwait(false);

				if (result == null)
				{
					var message = ($"No matching {typeof(Entity).Name} was found.");
					return new GenericResult<IEnumerable<Dto>>(message);
				}

				return new GenericResult<IEnumerable<Dto>>
				{
					Value = mapper.Map<IEnumerable<Dto>>(result)
				};
			}
			catch (Exception ex)
			{
				return new GenericResult<IEnumerable<Dto>>(ex);
			}
		}

		public virtual async Task<GenericResult<Dto>> GetOneByMatchAsync(Expression<Func<Dto, bool>> match)
		{
			try
			{
				Expression<Func<Entity, bool>> matchMap = mapper.Map<Expression<Func<Entity, bool>>>(match);

				Entity result = await repository.GetOneByMatchAsync(matchMap).ConfigureAwait(false);

				var response = new GenericResult<Dto>
				{
					Value = mapper.Map<Dto>(result)
				};

				return response;
			}
			catch (Exception ex)
			{
				return new GenericResult<Dto>(ex);
			}
		}

		public virtual async Task<GenericResult<int>> SaveAsync()
		{
			try
			{
				var result = await repository.SaveAsync().ConfigureAwait(false);

				return new GenericResult<int>
				{
					Value = result
				};
			}
			catch (Exception ex)
			{
				return new GenericResult<int>(ex);
			}
		}

		public virtual async Task<GenericResult<Dto>> UpdateAsync(Dto dto)
		{
			try
			{
				Entity entity = mapper.Map<Entity>(dto);

				Entity result = await repository.UpdateAsync(entity).ConfigureAwait(false);

				if (result == null)
				{
					var message =
						$"Could not update entity. No matching {typeof(Entity).Name} with Id: {dto.Id} was found.";
					return new GenericResult<Dto>(message);
				}

				return new GenericResult<Dto>
				{
					Value = mapper.Map<Dto>(result)
				};
			}
			catch (Exception ex)
			{
				return new GenericResult<Dto>(ex);
			}
		}
	}
}
