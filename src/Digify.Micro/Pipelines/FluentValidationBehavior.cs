using Digify.Micro.Exceptions;
using Digify.Micro.Extensions;
using FluentValidation;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Digify.Micro
{
    public class FluentValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private IEnumerable<IValidator<TRequest>> _validators;
        private readonly MicroSettings _microSettings;

        public FluentValidationBehavior(IEnumerable<IValidator<TRequest>> validators, MicroSettings microSettings)
        {
            _validators = validators;
            _microSettings = microSettings;
        }
        public int Order => 0;

        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);
                var failures = _validators
                                        .Select(v => v.Validate(context))
                                        .SelectMany(result => result.Errors)
                                        .Where(f => f != null)
                                        .ToList();

                if (failures.Any())
                {
                    throw new MicroValidationException(failures, _microSettings.ValidationErrorMessage);
                }
            }

            return next();

        }
    }
}
