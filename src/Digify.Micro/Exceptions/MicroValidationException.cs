using FluentValidation.Results;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Digify.Micro.Exceptions
{
    public class MicroValidationException : Exception
    {
        public readonly IEnumerable<Field> ValidationFailures = new List<Field>();

        public readonly HttpStatusCode HttpStatusCode = HttpStatusCode.BadRequest;

        public MicroValidationException(List<ValidationFailure> failures, string validationErrorMessage) : base(validationErrorMessage)
        {
            this.ValidationFailures = failures.Select(e => new Field()
            {
                FieldName = e.PropertyName,
                ErrorCode = e.ErrorCode,
                ErrorMessage = e.ErrorMessage,
            });
        }

        public class Field
        {
            public string FieldName { get; set; }
            public string ErrorMessage { get; set; }
            public string ErrorCode { get; set; }
        }
    }
}
