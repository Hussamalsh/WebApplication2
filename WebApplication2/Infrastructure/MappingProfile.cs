using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Models;

namespace WebApplication2.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RoomEntity, Room>()
                .ForMember(dest => dest.Rate, opt => opt.MapFrom(src => src.Rate / 100.0m))
                .ForMember(dest => dest.Self, opt => opt.MapFrom(src =>
                    Link.To(nameof(Controllers.RoomsController.GetRoomByIdAsync), new { roomId = src.Id })))
                .ForMember(dest => dest.Book, opt => opt.MapFrom(src =>
                    FormMetadata.FromModel(
                        new BookingForm(),
                        Link.ToForm(
                            nameof(Controllers.RoomsController.CreateBookingForRoomAsync),
                            new { roomId = src.Id },
                            Link.PostMethod,
                            Form.CreateRelation))));

            CreateMap<OpeningEntity, Opening>()
                .ForMember(dest => dest.Rate, opt => opt.MapFrom(src => src.Rate / 100m))
                .ForMember(dest => dest.StartAt, opt => opt.MapFrom(src => src.StartAt.ToUniversalTime()))
                .ForMember(dest => dest.EndAt, opt => opt.MapFrom(src => src.EndAt.ToUniversalTime()))
                .ForMember(dest => dest.Room, opt => opt.MapFrom(src =>
                    Link.To(nameof(Controllers.RoomsController.GetRoomByIdAsync), new { roomId = src.RoomId })));

            CreateMap<BookingEntity, Booking>()
                .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total / 100m))
                .ForMember(dest => dest.Self, opt => opt.MapFrom(src =>
                    Link.To(
                        nameof(Controllers.BookingsController.GetBookingByIdAsync),
                        new { bookingId = src.Id })))
                .ForMember(dest => dest.Room, opt => opt.MapFrom(src =>
                    Link.To(
                        nameof(Controllers.RoomsController.GetRoomByIdAsync),
                        new { roomId = src.Id }))).ForMember(dest => dest.Cancel, opt => opt.MapFrom(src =>
                    new Link
                    {
                        RouteName = nameof(Controllers.BookingsController.DeleteBookingByIdAsync),
                        RouteValues = new { bookingId = src.Id },
                        Method = Link.DeleteMethod
                    }));

            CreateMap<UserEntity, User>()
                .ForMember(dest => dest.Self, opt => opt.MapFrom(src =>
                    Link.To(nameof(Controllers.UsersController.GetUserByIdAsync),
                            new { userId = src.Id })));
        }
    }
}
