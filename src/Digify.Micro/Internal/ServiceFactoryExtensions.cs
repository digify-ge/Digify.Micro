using System;
using System.Collections.Generic;

namespace Digify.Micro.Internal
{
    public delegate object ServiceScope(Type serviceType);

    public static class ServiceFactoryExtensions
    {
        public static T GetInstance<T>(this ServiceScope factory)
            => (T)factory(typeof(T));

        public static IEnumerable<T> GetInstances<T>(this ServiceScope factory)
            => (IEnumerable<T>)factory(typeof(IEnumerable<T>));

        public static THandler GetHandler<THandler>(this ServiceScope factory)
        {
            THandler handler;

            try
            {
                handler = factory.GetInstance<THandler>();
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(typeof(THandler).ToString(), e);
            }

            if (handler == null)
            {
                throw new InvalidOperationException(typeof(THandler).ToString());
            }

            return handler;
        }
    }
}
