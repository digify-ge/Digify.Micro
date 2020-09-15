using Digify.Micro.Commands;
using Digify.Micro.Exceptions;
using Digify.Micro.Extensions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digify.Micro.Behaviors
{
    public class MicroHandlerValidator<TRequest>
    {
        private IEnumerable<IValidator<TRequest>> _validators;
        private MicroSettings _microSettings;

        public MicroHandlerValidator(IEnumerable<IValidator<TRequest>> validators, MicroSettings microSettings)
        {
            _validators = validators;
            _microSettings = microSettings;
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
                    throw new MicroValidationException(failures, _microSettings.ValidationErrorMessage);
                }
            }

        }
    }
}
