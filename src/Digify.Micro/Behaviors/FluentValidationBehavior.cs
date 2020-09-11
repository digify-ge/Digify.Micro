using Digify.Micro.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digify.Micro.Behaviors
{
    public class CommandValidator<TRequest>
    {
        private IEnumerable<IValidator<TRequest>> _validators;
        public CommandValidator(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task Handle(TRequest command)
        {
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(command);
                var validationResult = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context)));
                var failures = validationResult.SelectMany(x => x.Errors).Where(f => f != null).ToList();
                if (failures.Any())
                {
                    throw new ValidationException(failures);
                }
            }

        }
    }
}
