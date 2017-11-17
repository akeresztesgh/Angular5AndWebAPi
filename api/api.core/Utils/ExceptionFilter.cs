using api.core.Models;
using log4net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace api.core.Utils
{
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        private readonly ILog _logger = Logging.GetLogger("ExceptionFilter");
        private ExceptionContext _context;       

        public override void OnException(ExceptionContext context)
        {
            _context = context;
            string error = string.Empty;

            LogError();
            if (context.Exception is NotImplementedException)
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotImplemented;
            }
            else if (context.Exception is UnauthorizedAccessException)
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
            else if (context.Exception is InvalidOperationException)
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;
            }
            else
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            context.Result = new JsonResult(context.Exception.ToString());
            base.OnException(context);
        }

        private void LogError()
        {
            LogicalThreadContext.Properties["controller"] = _context.RouteData.Values["controller"];
            LogicalThreadContext.Properties["action"] = _context.RouteData.Values["action"];
            LogicalThreadContext.Properties["userid"] = GetUserId();
            LogicalThreadContext.Properties["parameters"] = GetParameters();

            _logger.Error("Unhandled Exception", _context.Exception);
        }

        private string GetParameters()
        {
            try
            {
                if (_context.HttpContext.Request.QueryString.HasValue)
                    return _context.HttpContext.Request.QueryString.ToString();
                if (_context.HttpContext.Request.Form.Count > 0)
                    return String.Join(string.Empty, _context.HttpContext.Request.Form.Select(x => $"{x.Key} = {x.Value},").ToList());
                return string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private string GetUserId()
        {
            try
            {
                var mgr = _context.HttpContext.RequestServices.GetService<UserManager<ApplicationUser>>();
                return _context.HttpContext.User == null ? string.Empty : mgr.GetUserId(_context.HttpContext.User);
            }
            catch (Exception)
            {
                return "N/A";
            }
        }

    }

}
