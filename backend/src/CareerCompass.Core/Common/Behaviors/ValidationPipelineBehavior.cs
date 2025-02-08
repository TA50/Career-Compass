using ErrorOr;
using FluentValidation;
using MediatR;

namespace CareerCompass.Core.Common.Behaviors;

public class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IErrorOr
{
    private IValidator<TRequest>? _validator;

    // public ValidationPipelineBehavior()
    // {
    //     _validator = null;
    // }

    public ValidationPipelineBehavior(IValidator<TRequest>? validator)
    {
        _validator = validator;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (_validator is null)
        {
            var result = await next();
            return result;
        }

        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (validationResult.IsValid)
        {
            return await next();
        }


        var errors = validationResult
            .Errors.ConvertAll(e => Error.Validation(e.PropertyName, e.ErrorMessage));

        return (dynamic)errors;
    }
}