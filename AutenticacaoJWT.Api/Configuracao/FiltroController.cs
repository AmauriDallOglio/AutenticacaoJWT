using Microsoft.AspNetCore.Mvc.Filters;

namespace AutenticacaoJWT.Api.Configuracao
{
    public class FiltroController : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            HelperConsoleColor.Sucesso($"FiltroController 1 - Antes de ser executada.");
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            HelperConsoleColor.Sucesso($"FiltroController 2 - Depois de ser executada.");
        }
    }
}
