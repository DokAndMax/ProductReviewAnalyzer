using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ReviewsScraper.API.Filters;

public sealed class ValidationExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext ctx)
    {
        if (ctx.Exception is not ValidationException ex) return;

        ctx.Result = new BadRequestObjectResult(new
        {
            Errors = ex.Errors.Select(e => e.ErrorMessage)
        });
        ctx.ExceptionHandled = true;
    }
}