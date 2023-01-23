using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using Microsoft.EntityFrameworkCore;
using SpyStore.DAL.Exceptions;


namespace SpyStore.Service.Filters
{
    public class SpyStoreExceptionFIlter: ExceptionFilterAttribute
    {

        private readonly IWebHostEnvironment _hostingEnvironment;

        public SpyStoreExceptionFIlter(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
/// <inheritdoc/>

        public override void OnException(ExceptionContext context)
        {

            bool isDevelopment = _hostingEnvironment.IsDevelopment();
            var ex = context.Exception;

            string stackTrace = (isDevelopment) ? context.Exception.StackTrace : string.Empty;
            string message = ex.Message;
            string error = string.Empty;
            base.OnException(context);

            IActionResult actionResult;

            switch(ex)
            {
                case SpyStoreInvalidQuantityException iqe:
                    error = "Invalid quantity request.";
                    actionResult = new BadRequestObjectResult(new { Error = error, Message = message, StackTrace = stackTrace});
                    break;
                case DbUpdateConcurrencyException ce:
                    //Returns a 400
                    error = "Concurrency Issue.";
                    actionResult = new BadRequestObjectResult(new
                    { Error = error, Message = message, StackTrace = stackTrace });
                    break;
                case SpyStoreInvalidProductException ipe:
                    //Returns a 400
                    error = "Invalid Product Id.";
                    actionResult = new BadRequestObjectResult(new
                    { Error = error, Message = message, StackTrace = stackTrace });
                    break;
                case SpyStoreInvalidCustomerException ice:
                    //Returns a 400
                    error = "Invalid Customer Id.";
                    actionResult = new BadRequestObjectResult(new
                    { Error = error, Message = message, StackTrace = stackTrace });
                    break;
                default:
                    error = "General Error.";
                    actionResult = new ObjectResult(new
                    { Error = error, Message = message, StackTrace = stackTrace })
                    { StatusCode = 500 };
                    break;

            }
            //context.ExceptionHandled = true;
            context.Result = actionResult;
        }
    }
}
