using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApplication2.Models;

namespace WebApplication2.Services
{
    public interface IBookingService
    {
        Task<Booking> GetBookingAsync(Guid bookingId, CancellationToken ct);

        Task<Guid> CreateBookingAsync(
                                        Guid userId,
                                        Guid roomId,
                                        DateTimeOffset startAt,
                                        DateTimeOffset endAt,
                                        CancellationToken ct);
    }
}
