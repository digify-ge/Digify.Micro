using Digify.Micro.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Digify.Micro
{
    public class BusAsync : IBusAsync
    {
        private readonly ServiceFactory _serviceFactory;

        public BusAsync(ServiceFactory serviceFactory)

        {
            this._serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));
        }
        public Task<TResult> ExecuteAsync<TResult>(IRequest<TResult> request, CancellationToken cancellationToken)
        {
            var requestType = request.GetType();
            var handler = (RequestHandlerWrapper<TResult>)Activator.CreateInstance(typeof(RequestHandlerWrapperImpl<,>).MakeGenericType(requestType, typeof(TResult)));
            return handler.Handle(request, cancellationToken, _serviceFactory);
        }
        public Task Publish<TRequest>(TRequest request, CancellationToken cancellationToken) where TRequest : IRequest
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            var requestType = request.GetType();
            var requestInterfaceType = requestType
                .GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>));
            var isValidRequest = requestInterfaceType != null;

            if (!isValidRequest)
            {
                throw new ArgumentException($"{nameof(request)} does not implement ${nameof(IRequest)}");
            }

            var responseType = requestInterfaceType!.GetGenericArguments()[0];
            var handler = (RequestHandlerBase)Activator.CreateInstance(typeof(RequestHandlerWrapperImpl<,>).MakeGenericType(requestType, responseType));

           
            return handler.Handle(request, cancellationToken, _serviceFactory);
        }
        public Task ExecuteAsync<TRequest>(TRequest request, CancellationToken cancellationToken) where TRequest : IRequest
        {
            var requestType = request.GetType();
            var requestInterfaceType = requestType
                .GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>));
            var isValidRequest = requestInterfaceType != null;

            if (!isValidRequest)
            {
                throw new ArgumentException($"{nameof(request)} does not implement ${nameof(IRequest)}");
            }

            var responseType = requestInterfaceType!.GetGenericArguments()[0];
            var handler = (RequestHandlerBase)Activator.CreateInstance(typeof(RequestHandlerWrapperImpl<,>).MakeGenericType(requestType, responseType));

            return handler.Handle(request, cancellationToken, _serviceFactory);
        }
        private static TypeBuilder GetTypeBuilder(Type type)
        {
            var typeSignature = type.Name;
            var an = new AssemblyName(typeSignature);
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(Guid.NewGuid().ToString()), AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
            TypeBuilder tb = moduleBuilder.DefineType(typeSignature,
                    TypeAttributes.Public |
                    TypeAttributes.Class |
                    TypeAttributes.AutoClass |
                    TypeAttributes.AnsiClass |
                    TypeAttributes.BeforeFieldInit |
                    TypeAttributes.AutoLayout,
                    null);
            return tb;
        }
    }
}
