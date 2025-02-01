using CareerCompass.Application.Users;
using ErrorOr;
using FluentValidation;
using MediatR;

namespace CareerCompass.Application.Common;

public class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IErrorOr
{
    private AbstractValidator<TRequest>? _validator;

    public ValidationPipelineBehavior()
    {
        _validator = null;
    }

    public ValidationPipelineBehavior(AbstractValidator<TRequest>? validator)
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