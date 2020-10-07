using System.Threading;
using System.Threading.Tasks;

namespace Digify.Micro
{
    public interface IRequestPreProcessor<in TRequest> where TRequest : notnull
    {
        int Order { get; }

        Task Process(TRequest request, CancellationToken cancellationToken);
    }
}
