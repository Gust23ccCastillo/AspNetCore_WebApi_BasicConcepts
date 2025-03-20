using FluentValidation;
using MediatR;

namespace BookingApplication.WebApi.FluentValidationBehavior
{
    /*El middleware global manejará las excepciones de validación lanzadas por ValidationBehavior.
    Las reglas de FluentValidation aplicadas a los comandos serán evaluadas antes de procesar el comando en sí.
    Si el comando tiene datos inválidos, se lanzará una excepción con detalles claros en el formato JSON.*/
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);

                var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
                var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

                if (failures.Count != 0)
                {
                    throw new FluentValidation.ValidationException(failures);
                }
            }

            return await next();
        }
    }

}
