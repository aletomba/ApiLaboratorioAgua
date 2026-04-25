using Dominio.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ApiLaboratorioAgua.Filters;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class ValidatePaginationAttribute : ActionFilterAttribute
{
    public int MaxPageSize { get; set; } = PaginationDefaults.MaxPageSize;

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ActionArguments.TryGetValue("page", out var pageObj) ||
            !context.ActionArguments.TryGetValue("pageSize", out var pageSizeObj))
        {
            return;
        }

        if (pageObj is int page && pageSizeObj is int pageSize)
        {
            if (page < 1 || pageSize < 1 || pageSize > MaxPageSize)
            {
                context.Result = new BadRequestObjectResult(
                    $"page debe ser >= 1 y pageSize entre 1 y {MaxPageSize}.");
            }
        }
    }
}