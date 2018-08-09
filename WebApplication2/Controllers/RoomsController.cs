using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebApplication2.Models;
using WebApplication2.Services;

namespace WebApplication2.Controllers
{
    [Route("/[controller]")]
    public class RoomsController : Controller
    {
        private readonly IRoomService _roomService;
        private readonly IOpeningService _openingService;
        private readonly PagingOptions _defaultPagingOptions;
        public RoomsController(IRoomService roomService,IOpeningService openingService,IOptions<PagingOptions> defaultPagingOptions)
        {
            _roomService = roomService;
            _openingService = openingService;
            _defaultPagingOptions = defaultPagingOptions.Value;
        }

        [HttpGet(Name = nameof(GetRoomsAsync))]
        public async Task<IActionResult> GetRoomsAsync(CancellationToken ct)
        {
            //return Ok(_roomService.Rooms);
            var rooms = await _roomService.GetRoomsAsync(ct);
            var collectionLink = Link.To(nameof(GetRoomsAsync));
            var collection = new Collection<Room>
            {
                Self = collectionLink,
                Value = rooms.ToArray()
            };
            return Ok(collection);
        }

        // /rooms/{roomID} [HttpGet("{roomId}", Name = nameof(GetRoomByIdAsync))]
        [HttpGet("{roomId}", Name = nameof(GetRoomByIdAsync))]
        public async Task<IActionResult> GetRoomByIdAsync(Guid roomId, CancellationToken ct)
        {
            var room = await _roomService.GetRoomAsync(roomId,ct);
            if(room == null) return NotFound();
            return Ok(room);
        }

        // GET /rooms/openings
        [HttpGet("openings", Name = nameof(GetAllRoomOpeningsAsync))]
        public async Task<IActionResult> GetAllRoomOpeningsAsync([FromQuery] PagingOptions pagingOptions,CancellationToken ct)
        {
            if (!ModelState.IsValid) return BadRequest(new ApiError(ModelState));

            pagingOptions.Offset = pagingOptions.Offset ?? _defaultPagingOptions.Offset;
            pagingOptions.Limit = pagingOptions.Limit ?? _defaultPagingOptions.Limit;

            var openings = await _openingService.GetOpeningsAsync(pagingOptions, ct);

            var collection = PagedCollection<Opening>.Create(
                Link.ToCollection(nameof(GetAllRoomOpeningsAsync)),
                openings.Items.ToArray(),
                openings.TotalSize,
                pagingOptions);

            return Ok(collection);
        }
    }
}