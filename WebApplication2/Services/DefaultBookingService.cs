using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApplication2.Models;

namespace WebApplication2.Services
{
    public class DefaultBookingService : IBookingService
    {
        private readonly HotelApiContext _context;
        private readonly IDateLogicService _dateLogicService;

        public DefaultBookingService(
            HotelApiContext context,
            IDateLogicService dateLogicService)
        {
            _context = context;
            _dateLogicService = dateLogicService;
        }

        public Task<Guid> CreateBookingAsync(
            Guid userId,
            Guid roomId,
            DateTimeOffset startAt,
            DateTimeOffset endAt,
            CancellationToken ct)
        {
            // TODO: Save the new booking to the database
            throw new NotImplementedException();
        }

        public async Task<Booking> GetBookingAsync(
            Guid bookingId,
            CancellationToken ct)
        {
            var entity = await _context.Bookings.SingleOrDefaultAsync(b => b.Id == bookingId, ct);

            if (entity == null) return null;

            return Mapper.Map<Booking>(entity);
        }
    }
}
