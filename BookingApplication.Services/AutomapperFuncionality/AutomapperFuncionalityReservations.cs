using AutoMapper;
using BookingApplication.Domain.Models;
using BookingApplication.Services.Querys.ReservationQuery.QueryReservationDtos;


namespace BookingApplication.Services.AutomapperFuncionality
{
    public class AutomapperFuncionalityReservations:Profile
    {
        public AutomapperFuncionalityReservations()
        {
            //NO ESTA EN FUNCIONAMIENTO
            CreateMap<Reservation, ModelDto_Reservation_Information>()
            .ForMember(dest => dest.reservationId, opt => opt.MapFrom(src => src.ReservationId))
            .ForMember(dest => dest.hotel_Information, opt => opt.MapFrom(src => src.HotelReservated ?? new Hotel()))
            .ForMember(dest => dest.room_Information, opt => opt.MapFrom(src => src.RoomReservated ?? new Room()))
            .ForMember(dest => dest.nameClient, opt => opt.MapFrom(src => src.Customer ?? "Cliente Desconocido"))
            .ForMember(dest => dest.daysOfReservation, opt => opt.MapFrom(src =>
                src.ListToDateReservatedInHotel
            ));

            //Mapeo para transformar `HotelReservationDate` a un DTO con la fecha en string
            CreateMap<HotelReservationDate, ModelDto_InfoDaysReservated>()
                  .ForMember(dest =>
                     dest.DayReservate,
                    opt => opt.MapFrom(src => src.ReservationDate.ToString("dd/MM/yyyy")));

            CreateMap<Room, ModelDto_Room_Information>()
                .ForMember(dest => dest.roomNumber,
                           opt => opt.MapFrom(src => src.RoomNumber))
                .ForMember(dest => dest.size,
                           opt => opt.MapFrom(src => src.RoomSize));

            CreateMap<Hotel, ModelDto_Hotel_Information>()
                .ForMember(dest => dest.Name,
                           opt => opt.MapFrom(src => src.HotelName))
                .ForMember(dest => dest.city,
                           opt => opt.MapFrom(src => src.City))
                .ForMember(dest => dest.country,
                           opt => opt.MapFrom(src => src.Country));
        }
    }
}
