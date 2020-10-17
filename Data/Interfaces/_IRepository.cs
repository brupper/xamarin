using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Brupper.Data
{
    /// <summary> Based on article: https://docs.microsoft.com/en-us/aspnet/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application#implement-a-generic-repository-and-a-unit-of-work-class </summary>
    public interface IRepository<TEntity> : IDisposable
        where TEntity : Entities.IBaseEntity
    {
        /// <summary>
        /// The method uses lambda expressions to allow the calling code to specify a filter 
        /// condition and a column to order the results by, and a string parameter lets the 
        /// caller provide a comma-delimited list of navigation properties for eager loading
        /// </summary>
        /// <param name="filter"> The code Expression<Func<TEntity, bool>> filter means the caller will provide a lambda expression based on the TEntity type, and this expression will return a Boolean value. For example, if the repository is instantiated for the Student entity type, the code in the calling method might specify student => student.LastName == "Smith" for the filter parameter. </param>
        /// <param name="orderBy"> The code Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy also means the caller will provide a lambda expression. But in this case, the input to the expression is an IQueryable object for the TEntity type. The expression will return an ordered version of that IQueryable object. For example, if the repository is instantiated for the Student entity type, the code in the calling method might specify q => q.OrderBy(s => s.LastName) for the orderBy parameter. </param>
        /// <param name="includeProperties"> Lets the caller provide a comma-delimited list of navigation properties for eager loading. </param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "", CancellationToken cancellationToken = default);

        Task<TEntity> GetByIdAsync(object id);

        Task InsertAsync(TEntity entity);

        Task DeleteAsync(object id);

        Task DeleteAsync(TEntity entityToDelete);

        Task UpdateAsync(TEntity entityToUpdate);

        Task SaveAsync();
    }
}
