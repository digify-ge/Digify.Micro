using System.Threading;
using System.Threading.Tasks;

namespace Digify.Micro
{
    public interface IRequestPostProcessor<in TRequest, in TResponse> where TRequest : notnull
    {
        int Order { get; }
        Task Process(TRequest request, TResponse response, CancellationToken cancellationToken);
    }
}
