using Digify.Micro.Commands;
using Digify.Micro.Domain;
using Digify.Micro.Queries;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Digify.Micro
{
    public interface ICommandValidationBehaviour<TCommand> where TCommand : ICommand
    {
        Task Handle(TCommand command);
    }
}
