using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApplication2.Models;

namespace WebApplication2.Services
{
    public class DefaultRoomService : IRoomService
    {
        private readonly HotelApiContext _context;

        public DefaultRoomService(HotelApiContext context)
        {
            _context = context;
        }

        public async Task<Room> GetRoomAsync(Guid id, CancellationToken ct)
        {
            var entity = await _context.Rooms.SingleOrDefaultAsync(r => r.Id == id, ct);
            if (entity == null) return null;
            return Mapper.Map<Room>(entity);
            /*var resource = new Room
            {
                Href = null, // Url.Link(nameof(GetRoomByIdAsync), new { roomId = entity.Id }),
                Name = entity.Name,
                Rate = entity.Rate / 100.0m
            };

            return resource;*/
        }

        public async Task<IEnumerable<Room>> GetRoomsAsync(CancellationToken ct)
        {
            var query = _context.Rooms.ProjectTo<Room>();
            return await query.ToArrayAsync();
        }
    }
}
