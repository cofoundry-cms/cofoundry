using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.CQS
{
    /// <summary>
    /// Helper extensions to QueryExecutor to avoid creating tons
    /// of stub query objects.
    /// </summary>
    /// <remarks>
    /// Idea taken from http://tech.trailmax.info/2013/08/constructors-should-be-simple/
    /// </remarks>
    public static class QueryExecutorExtensions
    {
        #region GetAll

        /// <summary>
        /// Executes a query to get all entities of a particular type. Use this
        /// as a short cut instead of creating a dedicated IQuery class.
        /// </summary>
        /// <typeparam name="TEntity">type of entity to return</typeparam>
        /// <param name="executor">IQueryExecutor instance</param>
        /// <returns>Collection of TEntity</returns>
        public static IEnumerable<TEntity> GetAll<TEntity>(this IQueryExecutor executor)
        {
            var query = new GetAllQuery<TEntity>();
            return executor.Execute(query);
        }

        /// <summary>
        /// Executes a query to get all entities of a particular type. Use this
        /// as a short cut instead of creating a dedicated IQuery class.
        /// </summary>
        /// <typeparam name="TEntity">type of entity to return</typeparam>
        /// <param name="executor">IQueryExecutor instance</param>
        /// <param name="ex">
        /// If specified this will execute the query under 
        /// the specified context. Useful for running the query under a higher privalidge account.
        /// </param>
        /// <returns>Collection of TEntity</returns>
        public static IEnumerable<TEntity> GetAll<TEntity>(this IQueryExecutor executor, IExecutionContext ex)
        {
            var query = new GetAllQuery<TEntity>();
            return executor.Execute(query, ex);
        }

        /// <summary>
        /// Executes a query to get all entities of a particular type. Use this
        /// as a short cut instead of creating a dedicated IQuery class.
        /// </summary>
        /// <typeparam name="TEntity">type of entity to return</typeparam>
        /// <param name="executor">IQueryExecutor instance</param>
        /// <returns>Collection of TEntity</returns>
        public static async Task<IEnumerable<TEntity>> GetAllAsync<TEntity>(this IQueryExecutor executor)
        {
            var query = new GetAllQuery<TEntity>();
            return await executor.ExecuteAsync(query);
        }

        /// <summary>
        /// Executes a query to get all entities of a particular type. Use this
        /// as a short cut instead of creating a dedicated IQuery class.
        /// </summary>
        /// <typeparam name="TEntity">type of entity to return</typeparam>
        /// <param name="executor">IQueryExecutor instance</param>
        /// <param name="ex">
        /// If specified this will execute the query under 
        /// the specified context. Useful for running the query under a higher privalidge account.
        /// </param>
        /// <returns>Collection of TEntity</returns>
        public static async Task<IEnumerable<TEntity>> GetAllAsync<TEntity>(this IQueryExecutor executor, IExecutionContext ex)
        {
            var query = new GetAllQuery<TEntity>();
            return await executor.ExecuteAsync(query, ex);
        }

        #endregion

        #region Get

        /// <summary>
        /// Executes a query to get a single entity of the specified type where no query parameters are required. Use this
        /// as a short cut instead of creating a dedicated IQuery class.
        /// </summary>
        /// <typeparam name="TEntity">type of entity to return</typeparam>
        /// <returns>TEntity instance if found; otherwise default(TEntity)</returns>
        public static TEntity Get<TEntity>(this IQueryExecutor executor)
        {
            var query = new GetQuery<TEntity>();

            return executor.Execute(query);
        }

        /// <summary>
        /// Executes a query to get a single entity of the specified type where no query parameters are required. Use this
        /// as a short cut instead of creating a dedicated IQuery class.
        /// </summary>
        /// <typeparam name="TEntity">type of entity to return</typeparam>
        /// <param name="ex">
        /// If specified this will execute the query under 
        /// the specified context. Useful for running the query under a higher privalidge account.
        /// </param>
        /// <returns>TEntity instance if found; otherwise default(TEntity)</returns>
        public static TEntity Get<TEntity>(this IQueryExecutor executor, IExecutionContext ex)
        {
            var query = new GetQuery<TEntity>();

            return executor.Execute(query, ex);
        }

        /// <summary>
        /// Executes a query to get a single entity of the specified type where no query parameters are required. Use this
        /// as a short cut instead of creating a dedicated IQuery class.
        /// </summary>
        /// <typeparam name="TEntity">type of entity to return</typeparam>
        /// <returns>TEntity instance if found; otherwise default(TEntity)</returns>
        public static async Task<TEntity> GetAsync<TEntity>(this IQueryExecutor executor)
        {
            var query = new GetQuery<TEntity>();

            return await executor.ExecuteAsync(query);
        }

        /// <summary>
        /// Executes a query to get a single entity of the specified type where no query parameters are required. Use this
        /// as a short cut instead of creating a dedicated IQuery class.
        /// </summary>
        /// <typeparam name="TEntity">type of entity to return</typeparam>
        /// <param name="ex">
        /// If specified this will execute the query under 
        /// the specified context. Useful for running the query under a higher privalidge account.
        /// </param>
        /// <returns>TEntity instance if found; otherwise default(TEntity)</returns>
        public static async Task<TEntity> GetAsync<TEntity>(this IQueryExecutor executor, IExecutionContext ex)
        {
            var query = new GetQuery<TEntity>();

            return await executor.ExecuteAsync(query, ex);
        }

        #endregion

        #region GetById

        /// <summary>
        /// Executes a query to get a single entity of the specified type using an integer identifier. Use this
        /// as a short cut instead of creating a dedicated IQuery class.
        /// </summary>
        /// <typeparam name="TEntity">type of entity to return</typeparam>
        /// <param name="id">Id of the entity to get. Values less than 1 will always return default(TEntity).</param>
        /// <returns>TEntity instance if found; otherwise default(TEntity)</returns>
        public static TEntity GetById<TEntity>(this IQueryExecutor executor, int id)
        {
            if (id <= 0) return default(TEntity);
            var query = new GetByIdQuery<TEntity>() { Id = id };

            return executor.Execute(query);
        }

        /// <summary>
        /// Executes a query to get a single entity of the specified type using an integer identifier. Use this
        /// as a short cut instead of creating a dedicated IQuery class.
        /// </summary>
        /// <typeparam name="TEntity">type of entity to return</typeparam>
        /// <param name="id">Id of the entity to get. Values less than 1 will always return default(TEntity).</param>
        /// <param name="ex">
        /// If specified this will execute the query under 
        /// the specified context. Useful for running the query under a higher privalidge account.
        /// </param>
        /// <returns>TEntity instance if found; otherwise default(TEntity)</returns>
        public static TEntity GetById<TEntity>(this IQueryExecutor executor, int id, IExecutionContext ex)
        {
            if (id <= 0) return default(TEntity);
            var query = new GetByIdQuery<TEntity>() { Id = id };

            return executor.Execute(query, ex);
        }

        /// <summary>
        /// Executes a query to get a single entity of the specified type using an integer identifier. Use this
        /// as a short cut instead of creating a dedicated IQuery class.
        /// </summary>
        /// <typeparam name="TEntity">type of entity to return</typeparam>
        /// <param name="id">Id of the entity to get. Values less than 1 will always return default(TEntity).</param>
        /// <returns>TEntity instance if found; otherwise default(TEntity)</returns>
        public static async Task<TEntity> GetByIdAsync<TEntity>(this IQueryExecutor executor, int id)
        {
            if (id <= 0) return default(TEntity);
            var query = new GetByIdQuery<TEntity>() { Id = id };

            return await executor.ExecuteAsync(query);
        }

        /// <summary>
        /// Executes a query to get a single entity of the specified type using an integer identifier. Use this
        /// as a short cut instead of creating a dedicated IQuery class.
        /// </summary>
        /// <typeparam name="TEntity">type of entity to return</typeparam>
        /// <param name="id">Id of the entity to get. Values less than 1 will always return default(TEntity).</param>
        /// <param name="ex">
        /// If specified this will execute the query under 
        /// the specified context. Useful for running the query under a higher privalidge account.
        /// </param>
        /// <returns>TEntity instance if found; otherwise default(TEntity)</returns>
        public static async Task<TEntity> GetByIdAsync<TEntity>(this IQueryExecutor executor, int id, IExecutionContext ex)
        {
            if (id <= 0) return default(TEntity);
            var query = new GetByIdQuery<TEntity>() { Id = id };

            return await executor.ExecuteAsync(query, ex);
        }

        /// <summary>
        /// Executes a query to get a single entity of the specified type using a string identifier. Use this
        /// as a short cut instead of creating a dedicated IQuery class.
        /// </summary>
        /// <typeparam name="TEntity">type of entity to return</typeparam>
        /// <param name="id">Id of the entity to get. Null or empty values will always return default(TEntity).</param>
        /// <returns>TEntity instance if found; otherwise default(TEntity)</returns>
        public static TEntity GetById<TEntity>(this IQueryExecutor executor, string id)
        {
            if (string.IsNullOrEmpty(id)) return default(TEntity);
            var query = new GetByStringQuery<TEntity>() { Id = id };
            return executor.Execute(query);
        }

        /// <summary>
        /// Executes a query to get a single entity of the specified type using a string identifier. Use this
        /// as a short cut instead of creating a dedicated IQuery class.
        /// </summary>
        /// <typeparam name="TEntity">type of entity to return</typeparam>
        /// <param name="id">Id of the entity to get. Null or empty values will always return default(TEntity).</param>
        /// <param name="ex">
        /// If specified this will execute the query under 
        /// the specified context. Useful for running the query under a higher privalidge account.
        /// </param>
        /// <returns>TEntity instance if found; otherwise default(TEntity)</returns>
        public static TEntity GetById<TEntity>(this IQueryExecutor executor, string id, IExecutionContext ex)
        {
            if (string.IsNullOrEmpty(id)) return default(TEntity);
            var query = new GetByStringQuery<TEntity>() { Id = id };
            return executor.Execute(query, ex);
        }

        /// <summary>
        /// Executes a query to get a single entity of the specified type using a string identifier. Use this
        /// as a short cut instead of creating a dedicated IQuery class.
        /// </summary>
        /// <typeparam name="TEntity">type of entity to return</typeparam>
        /// <param name="id">Id of the entity to get. Null or empty values will always return default(TEntity).</param>
        /// <returns>TEntity instance if found; otherwise default(TEntity)</returns>
        public static async Task<TEntity> GetByIdAsync<TEntity>(this IQueryExecutor executor, string id)
        {
            if (string.IsNullOrEmpty(id)) return default(TEntity);
            var query = new GetByStringQuery<TEntity>() { Id = id };
            return await executor.ExecuteAsync(query);
        }

        /// <summary>
        /// Executes a query to get a single entity of the specified type using a string identifier. Use this
        /// as a short cut instead of creating a dedicated IQuery class.
        /// </summary>
        /// <typeparam name="TEntity">type of entity to return</typeparam>
        /// <param name="id">Id of the entity to get. Null or empty values will always return default(TEntity).</param>
        /// <param name="ex">
        /// If specified this will execute the query under 
        /// the specified context. Useful for running the query under a higher privalidge account.
        /// </param>
        /// <returns>TEntity instance if found; otherwise default(TEntity)</returns>
        public static async Task<TEntity> GetByIdAsync<TEntity>(this IQueryExecutor executor, string id, IExecutionContext ex)
        {
            if (string.IsNullOrEmpty(id)) return default(TEntity);
            var query = new GetByStringQuery<TEntity>() { Id = id };
            return await executor.ExecuteAsync(query, ex);
        }

        #endregion

        #region GetByIdRange

        /// <summary>
        /// Executes a query to get a set of entities of the specified type using a range of integer identifier. Use this
        /// as a short cut instead of creating a dedicated IQuery class.
        /// </summary>
        /// <typeparam name="TEntity">type of entity to return in the dictionary</typeparam>
        /// <param name="ids">Ids of the entitys to get. Null or values less than 1 will always return an empty dictionary.</param>
        /// <returns>IDictionary containing any records found</returns>
        public static IDictionary<int, TEntity> GetByIdRange<TEntity>(this IQueryExecutor executor, IEnumerable<int> ids)
        {
            if (ids == null || !ids.Any()) return new Dictionary<int, TEntity>();
            var query = new GetByIdRangeQuery<TEntity>(ids);

            return executor.Execute(query);
        }

        /// <summary>
        /// Executes a query to get a set of entities of the specified type using a range of integer identifier. Use this
        /// as a short cut instead of creating a dedicated IQuery class.
        /// </summary>
        /// <typeparam name="TEntity">type of entity to return in the dictionary</typeparam>
        /// <param name="ids">Ids of the entitys to get. Null or values less than 1 will always return an empty dictionary.</param>
        /// <param name="ex">
        /// If specified this will execute the query under 
        /// the specified context. Useful for running the query under a higher privalidge account.
        /// </param>
        /// <returns>IDictionary containing any records found</returns>
        public static IDictionary<int, TEntity> GetByIdRange<TEntity>(this IQueryExecutor executor, IEnumerable<int> ids, IExecutionContext ex)
        {
            if (ids == null || !ids.Any()) return new Dictionary<int, TEntity>();
            var query = new GetByIdRangeQuery<TEntity>(ids);

            return executor.Execute(query, ex);
        }

        /// <summary>
        /// Executes a query to get a set of entities of the specified type using a range of integer identifier. Use this
        /// as a short cut instead of creating a dedicated IQuery class.
        /// </summary>
        /// <typeparam name="TEntity">type of entity to return in the dictionary</typeparam>
        /// <param name="ids">Ids of the entity to get. Null or less than 1 will always return an empty dictionary.</param>
        /// <returns>IDictionary containing any records found</returns>
        public static async Task<IDictionary<int, TEntity>> GetByIdRangeAsync<TEntity>(this IQueryExecutor executor, IEnumerable<int> ids)
        {
            if (ids == null || !ids.Any()) return new Dictionary<int, TEntity>();
            var query = new GetByIdRangeQuery<TEntity>(ids);

            return await executor.ExecuteAsync(query);
        }

        /// <summary>
        /// Executes a query to get a set of entities of the specified type using a range of integer identifier. Use this
        /// as a short cut instead of creating a dedicated IQuery class.
        /// </summary>
        /// <typeparam name="TEntity">type of entity to return in the dictionary</typeparam>
        /// <param name="ids">Ids of the entity to get. Null or less than 1 will always return an empty dictionary.</param>
        /// <param name="ex">
        /// If specified this will execute the query under 
        /// the specified context. Useful for running the query under a higher privalidge account.
        /// </param>
        /// <returns>IDictionary containing any records found</returns>
        public static async Task<IDictionary<int, TEntity>> GetByIdRangeAsync<TEntity>(this IQueryExecutor executor, IEnumerable<int> ids, IExecutionContext ex)
        {
            if (ids == null || !ids.Any()) return new Dictionary<int, TEntity>();
            var query = new GetByIdRangeQuery<TEntity>(ids);

            return await executor.ExecuteAsync(query, ex);
        }

        /// <summary>
        /// Executes a query to get a set of entities of the specified type using a range of string identifier. Use this
        /// as a short cut instead of creating a dedicated IQuery class.
        /// </summary>
        /// <typeparam name="TEntity">type of entity to return in the dictionary</typeparam>
        /// <param name="ids">Ids of the entity to get. Null or empty values will always return an empty dictionary.</param>
        /// <returns>IDictionary containing any records found</returns>
        public static IDictionary<string, TEntity> GetById<TEntity>(this IQueryExecutor executor, IEnumerable<string> ids)
        {
            if (ids == null || !ids.Any()) return new Dictionary<string, TEntity>();
            var query = new GetByStringRangeQuery<TEntity>(ids);
            return executor.Execute(query);
        }

        /// <summary>
        /// Executes a query to get a set of entities of the specified type using a range of string identifier. Use this
        /// as a short cut instead of creating a dedicated IQuery class.
        /// </summary>
        /// <typeparam name="TEntity">type of entity to return in the dictionary</typeparam>
        /// <param name="ids">Ids of the entity to get. Null or empty values will always return an empty dictionary.</param>
        /// <param name="ex">
        /// If specified this will execute the query under 
        /// the specified context. Useful for running the query under a higher privalidge account.
        /// </param>
        /// <returns>IDictionary containing any records found</returns>
        public static IDictionary<string, TEntity> GetById<TEntity>(this IQueryExecutor executor, IEnumerable<string> ids, IExecutionContext ex)
        {
            if (ids == null || !ids.Any()) return new Dictionary<string, TEntity>();
            var query = new GetByStringRangeQuery<TEntity>(ids);
            return executor.Execute(query, ex);
        }

        /// <summary>
        /// Executes a query to get a set of entities of the specified type using a range of string identifier. Use this
        /// as a short cut instead of creating a dedicated IQuery class.
        /// </summary>
        /// <typeparam name="TEntity">type of entity to return in the dictionary</typeparam>
        /// <param name="ids">Ids of the entity to get. Null or empty values will always return an empty dictionary.</param>
        /// <returns>IDictionary containing any records found</returns>
        public static async Task<IDictionary<string, TEntity>> GetByIdAsync<TEntity>(this IQueryExecutor executor, IEnumerable<string> ids)
        {
            if (ids == null || !ids.Any()) return new Dictionary<string, TEntity>();
            var query = new GetByStringRangeQuery<TEntity>(ids);
            return await executor.ExecuteAsync(query);
        }

        /// <summary>
        /// Executes a query to get a set of entities of the specified type using a range of string identifier. Use this
        /// as a short cut instead of creating a dedicated IQuery class.
        /// </summary>
        /// <typeparam name="TEntity">type of entity to return in the dictionary</typeparam>
        /// <param name="ids">Ids of the entity to get. Null or empty values will always return an empty dictionary.</param>
        /// <param name="ex">
        /// If specified this will execute the query under 
        /// the specified context. Useful for running the query under a higher privalidge account.
        /// </param>
        /// <returns>IDictionary containing any records found</returns>
        public static async Task<IDictionary<string, TEntity>> GetByIdAsync<TEntity>(this IQueryExecutor executor, IEnumerable<string> ids, IExecutionContext ex)
        {
            if (ids == null || !ids.Any()) return new Dictionary<string, TEntity>();
            var query = new GetByStringRangeQuery<TEntity>(ids);
            return await executor.ExecuteAsync(query, ex);
        }

        #endregion
    }
}
