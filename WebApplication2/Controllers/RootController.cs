using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    [Route("/")]
    [ApiVersion("1.0")]
    public class RootController : Controller
    {
        [HttpGet(Name = nameof(GetRoot))]
        public IActionResult GetRoot()
        {
            /*var response = new
            {
                href = Url.Link(nameof(GetRoot),null),
                rooms = new { href = Url.Link(nameof(RoomsController.GetRooms), null)},
                info = new { href = Url.Link(nameof(InfoController.GetInfo), null)}
            };*/

            var response = new RootResponse
            {
                Self = Link.To(nameof(GetRoot)),
                Rooms = Link.To(nameof(RoomsController.GetRoomsAsync)),
                Info = Link.To(nameof(InfoController.GetInfo)),
            };


            return Ok(response);
        }
    }
}