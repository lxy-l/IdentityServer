using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace WebMVC.Controllers
{
    public class AuthroizationController : Controller
    {
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
