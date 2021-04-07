using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PaymentGateway.Application.Contracts.Exceptions;
using System;
using System.Net;
using System.Threading.Tasks;

namespace PaymentGateway.Api.Middleware
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await ConvertException(context, ex);
            }
        }

        private Task ConvertException(HttpContext context, Exception exception)
        {
            HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError;

            context.Response.ContentType = "application/json";

            var result = string.Empty;

            switch (exception)
            {
                case ValidationException validationException:
                    httpStatusCode = HttpStatusCode.BadRequest;
                    result = JsonConvert.SerializeObject(validationException.ValdationErrors);
                    break;
                case BadRequestException badRequestException:
                    httpStatusCode = HttpStatusCode.BadRequest;
                    result = badRequestException.Message;
                    break;
                case NotFoundException notFoundException:
                    httpStatusCode = HttpStatusCode.NotFound;
                    break;
                case UnAuthorizedException unAuthorizedException:
                    httpStatusCode = HttpStatusCode.Unauthorized;
                    break;
                case ForbiddenException forbiddenException:
                    httpStatusCode = HttpStatusCode.Forbidden;
                    break;
                case Exception ex:
                    httpStatusCode = HttpStatusCode.InternalServerError;
                    //log exception details for troubleshooting.
                    _logger.LogError(@$"An exception of type {exception.GetType()} occured.
                            Message:{ex.Message}
                            StackTrace:{ex.StackTrace}");
                    result = "An error occured within the server.";
                    break;
            }

            context.Response.StatusCode = (int)httpStatusCode;
            context.Response.ContentType = "application/json";

            if (result == string.Empty)
            {
                result = JsonConvert.SerializeObject(new { error = exception.Message });
            }

            _logger.LogError($"An exception of type {exception.GetType()} occured. Result to the client:{result}");

            return context.Response.WriteAsync(result as string ?? JsonConvert.SerializeObject(result, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            }));
        }
    }
}
