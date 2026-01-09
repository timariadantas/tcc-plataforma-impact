using ClientService.Domain.Exceptions;
using ClientService.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;

namespace ClientService.Middleswares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly LoggerService _logger;
        
    

    public ExceptionMiddleware (RequestDelegate next, LoggerService logger)
        {
            _next = next;           //próximo middleware na pipeline do ASP.NET
            _logger = logger;
        }

    public async Task Invoke (HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(ClientNotFoundException ex)
            {
                _logger.Log(ex.Message, Logging.LogLevel.WARNING);
                await WriteError(context, HttpStatusCode.NotFound, ex.Message);
            }
        catch(ClientValidationException ex){
            _logger.Log(ex.Message, Logging.LogLevel.WARNING);
            await WriteError(context, HttpStatusCode.BadRequest,ex.Message);

            }
            catch(DomainException ex)
            {
                _logger.Log(ex.Message, Logging.LogLevel.WARNING);
                await WriteError(context, HttpStatusCode.BadRequest,ex.Message);
            }
            catch(System.Exception ex)
            {
                _logger.Log($"Erro inesperado: {ex.Message}", Logging.LogLevel.ERROR);
                _logger.Log(ex.StackTrace ?? "Sem stacktrace", Logging.LogLevel.ERROR);
                await WriteError(context, HttpStatusCode.InternalServerError,"Erro interno do servidor");
            }
        }

            private static async Task WriteError(HttpContext context , HttpStatusCode status, string message)
        {
            context.Response.ContentType = "application/json";
            
            var payload = new {error = message};
            await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
        }
}}
        
