using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Brupper.Data
{
    public interface IFilterableRepository<TEntity> : IDisposable
        where TEntity : Entities.IBaseEntity
    {
        Task<IEnumerable<TEntity>> FilterAsync(string filterText, string includeProperties = "", CancellationToken cancellationToken = default);
    }
}
