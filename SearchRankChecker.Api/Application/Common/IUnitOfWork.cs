using SearchRankChecker.Api.Entities;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace SearchRankChecker.Api.Application.Common;

public interface IUnitOfWork : IDisposable
{
    IRepository<T> Repository<T>() where T : BaseEntity;

    Task<int> Commit(CancellationToken cancellationToken = default);

    Task Rollback();
}