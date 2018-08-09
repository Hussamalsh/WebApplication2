using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication2.Controllers
{
    [Route("/[controller]")]
    public class InfoController : Controller
    {
        
        [HttpGet(Name = nameof(GetInfo))]
        public IActionResult GetInfo()
        {
            return View();
        }
    }
}