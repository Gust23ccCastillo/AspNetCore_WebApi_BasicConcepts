using AutoMapper;
using BookingApplication.Domain.Models;
using BookingApplication.Services.Commands.CommandReservations.CommandReservationDto;
using BookingApplication.Services.Commands.CommandRooms.CommandModelDto;
using BookingApplication.Services.Querys.RoomHotelQuery.QueryRoomHotelDto;

namespace BookingApplication.Services.AutomapperFuncionality
{
    public class AutomapperFuncionalityRooms:Profile
    {
        public AutomapperFuncionalityRooms()
        {

            CreateMap<Room, ModelDtoRoomInformation>()
                     .ForMember(dest =>
                dest.idRoom,
                opt => opt.MapFrom(src => src.RoomId))
                     .ForMember(dest =>
                dest.roomNumber,
                opt => opt.MapFrom(src => src.RoomNumber))
            .ForMember(dest =>
                dest.size,
                opt => opt.MapFrom(src => src.RoomSize))
            .ForMember(dest =>
                dest.needsRepair,
                opt => opt.MapFrom(src => src.NeedRepair));

            CreateMap<Room, ModelDtoReturnUpdateRooms>()
             .ForMember(dest =>
               dest.IdRoom,
               opt => opt.MapFrom(src => src.RoomId))
             .ForMember(dest =>
               dest.roomNumber,
               opt => opt.MapFrom(src => src.RoomNumber))
           .ForMember(dest =>
               dest.sizeRoom,
               opt => opt.MapFrom(src => src.RoomSize))
           .ForMember(dest =>
               dest.NeedRepair,
               opt => opt.MapFrom(src => src.NeedRepair));

        }
    }
}
