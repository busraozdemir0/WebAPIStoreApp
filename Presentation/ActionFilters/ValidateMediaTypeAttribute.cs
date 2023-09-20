using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;

namespace Presentation.ActionFilters
{
    public class ValidateMediaTypeAttribute: ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // accept ifadesi var mı yok mu kontrolü
            var acceptHeaderPresent = context.HttpContext
                .Request
                .Headers
                .ContainsKey("Accept");

            if(!acceptHeaderPresent)
            {
                context.Result = new BadRequestObjectResult($"Accept header is missing!");
                return;
            }

            // accept bizim desteklediğimiz formatta mı diye kontrol ettik
            var mediaType = context.HttpContext
                .Request
                .Headers["Accept"]
                .FirstOrDefault();

            if(!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue? outMediaType))
            {
                context.Result = new BadRequestObjectResult($"Media type not present. " + $"Please add Accept header with required media type.");
                return;
            }

            // accept bizim desteklediğimiz formattaysa HttpContext'e ekledik.
            context.HttpContext.Items.Add("AcceptHeaderMediaType", outMediaType);
        }
    }
}
