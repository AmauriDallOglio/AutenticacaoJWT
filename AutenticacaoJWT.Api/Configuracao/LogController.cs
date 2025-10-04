using Microsoft.AspNetCore.Mvc.Filters;

namespace AutenticacaoJWT.Api.Configuracao
{
    public class LogController : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            HelperConsoleColor.Sucesso($"LogController 1 - Antes de ser executada.");
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            HelperConsoleColor.Sucesso($"LogController 2 - Depois de ser executada.");
        }
    }
}
