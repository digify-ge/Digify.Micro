﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Digify.Micro
{
    public interface IRequest : IRequest<Unit> { }

    public interface IRequest<out TResponse> : IRequestBase { }
    public interface IRequestBase { }
    public interface ICommand<T> : IRequest<T> { }
    public interface ICommand : IRequest { }
    public interface IDomainEvent : IRequest
    {
        Guid AggregateRootId { get; }
        long Version { get; set; }
    }
}
