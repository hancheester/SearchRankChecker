using SearchRankChecker.Api.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SearchRankChecker.Api.Application.Common;

public interface IRepository<T> where T : BaseEntity
{
    IQueryable<T> Entities { get; }
    Task<T> Add(T entity, CancellationToken cancellationToken = default);
    Task AddRange(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    Task<T> GetById(int id, CancellationToken cancellationToken = default);
    Task Delete(T entity);
}
