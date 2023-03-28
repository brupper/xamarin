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
        // /// <summary> </summary>
        // IRepository<TEntity> WithTracking();
        // 
        // /// <summary> </summary>
        // IRepository<TEntity> WithoutTracking();

        /// <summary>
        /// The method uses lambda expressions to allow the calling code to specify a filter 
        /// condition and a column to order the results by, and a string parameter lets the 
        /// caller provide a comma-delimited list of navigation properties for eager loading
        /// </summary>
        /// <param name="filter"> The code Expression{Func{TEntity, bool}} filter means the caller will provide a lambda expression based on the TEntity type, and this expression will return a Boolean value. For example, if the repository is instantiated for the Student entity type, the code in the calling method might specify student => student.LastName == "Smith" for the filter parameter. </param>
        /// <param name="orderBy"> The code Func{IQueryable{TEntity}, IOrderedQueryable{TEntity}} orderBy also means the caller will provide a lambda expression. But in this case, the input to the expression is an IQueryable object for the TEntity type. The expression will return an ordered version of that IQueryable object. For example, if the repository is instantiated for the Student entity type, the code in the calling method might specify q => q.OrderBy(s => s.LastName) for the orderBy parameter. </param>
        /// <param name="includeProperties"> Lets the caller provide a comma-delimited list of navigation properties for eager loading. </param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? pageSize = null,
            int pageNumber = 0,
            string includeProperties = "",
            CancellationToken cancellationToken = default);

        /// <summary> </summary>
        Task<TEntity> GetByIdAsync(object id);

        /// <summary> </summary>
        Task InsertAsync(TEntity entity);

        /// <summary> </summary>
        Task InsertRangeAsync(IEnumerable<TEntity> entities);

        /// <summary> </summary>
        Task InsertOrUpdateAsync(TEntity entity);

        // note that, for the ADD and Remove Functions, we just do the operation on the dbContext object.
        // But we are not yet commiting/updating/saving the changes to the database whatsover. This is not
        // something to be done in a Repository Class. We would need Unit of Work Pattern for these cases
        // where you commit data to the database.

        /// <summary> </summary>
        Task UpdateAsync(TEntity entityToUpdate);

        /// <summary> </summary>
        Task DeleteAsync(object id);

        /// <summary> </summary>
        Task DeleteAsync(TEntity entityToDelete);

        /// <summary> </summary>
        Task DeleteRangeAsync(IEnumerable<TEntity> entities);

        /// <summary> </summary>
        Task Revert(TEntity entity);

        /// <summary> </summary>
        Task SaveAsync();
    }
}
