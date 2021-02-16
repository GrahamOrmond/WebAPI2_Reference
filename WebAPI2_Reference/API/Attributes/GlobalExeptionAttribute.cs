using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http.Filters;

namespace WebAPI2_Reference.API.Attributes
{
    public class UnhandledExeptionAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            HttpStatusCode status;
            string message, error;
            var exceptionType = context.Exception.GetType();
            if (exceptionType == typeof(UnauthorizedAccessException))
            {
                error = "invalid_grant";
                message = "Access to the Web API is not authorized.";
                status = HttpStatusCode.Unauthorized;
            }
            else
            {
                error = "internal_server_error";
                message = "Oops looks like somethings wrong with the server. Please check input and try again.";
                status = HttpStatusCode.InternalServerError;
            }

            context.Response = context.Request.CreateResponse(status,
            new
            {
                error = error,
                error_description = message,
            },
            new JsonMediaTypeFormatter());
            base.OnException(context);
        }
    }
}
