using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace api.core.Utils
{
    public class ApiControllerBase : Controller
    {
        public string UserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}
