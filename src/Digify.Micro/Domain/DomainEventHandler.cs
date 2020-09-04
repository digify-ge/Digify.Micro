using System.Threading.Tasks;

namespace Digify.Micro.Domain
{
    public interface IDomainEventHandlerAsync<in TDomainEvent> where TDomainEvent : IDomainEvent
    {
        Task HandleAsync(TDomainEvent domainEvent);
    }
}
