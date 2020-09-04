using System.Threading.Tasks;

namespace Digify.Micro.Queries
{
    public interface IQueryHandlerAsync<TQuery, TResult> where TQuery : IQuery
    {
        Task<TResult> HandleAsync(TQuery query);
    }
}

