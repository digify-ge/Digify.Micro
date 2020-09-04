using Autofac;
using System;
using System.Threading.Tasks;

namespace Digify.Micro.Queries
{
    public interface IQueryBusAsync
    {
        /// <summary>
        /// Find handler for specific query
        /// </summary>
        /// <typeparam name="TQuery">Query parameter type</typeparam>
        /// <typeparam name="TResult">Query result type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Returns result of query</returns>
        /// <exception cref="ArgumentNullException">Thrown when query argument is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when handler is not found.</exception>
        Task<TResult> ExecuteAsync<TQuery, TResult>(TQuery query) where TQuery : IQuery;
    }

    public class QueryBusAsync : IQueryBusAsync
    {
        private readonly ILifetimeScope context;

        public QueryBusAsync(ILifetimeScope context) => this.context = context ?? throw new ArgumentNullException(nameof(context));

     
        public Task<TResult> ExecuteAsync<TQuery, TResult>(TQuery query) where TQuery : IQuery
        {
            if (query == null)
                throw new ArgumentNullException($"Query shouldn't be null");

            using (var scope = context.BeginLifetimeScope())
            {
                var handler = scope.Resolve<IQueryHandlerAsync<TQuery, TResult>>()
                    ?? throw new InvalidOperationException($"Handler not found for specified query");

                return handler.HandleAsync(query);
            }
        }
    }
}
