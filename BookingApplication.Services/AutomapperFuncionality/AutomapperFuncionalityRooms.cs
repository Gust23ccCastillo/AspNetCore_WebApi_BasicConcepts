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


           // //MAPEO UTILIZADO PERO EN LE COMMAND DE DELETE RESERVATION
           // CreateMap<ModelReservsationRoomDto_Delete, Room>()
           //.ForMember(dest => dest.RoomId, opt => opt.MapFrom(src => src.idRoom))
           //.ForMember(dest => dest.RoomNumber, opt => opt.MapFrom(src => src.roomNumber))
           //.ForMember(dest => dest.RoomSize, opt => opt.MapFrom(src => src.size))
           //.ForMember(dest => dest.NeedRepair, opt => opt.MapFrom(src => src.needsRepair))
           //.ForMember(dest => dest.DateReservationForClient, opt => opt.MapFrom(src => src.infoDaysReservate));

           // CreateMap<ModelDaysReservationsRoomInfo_Dto, RoomReservationDate>()
           //     .ForMember(dest => dest.RoomReservationDateId, opt => opt.MapFrom(src => src.Id))
           //     .ForMember(dest => dest.ReservationDate, opt => opt.MapFrom(src => src.DayReservated));
        }
    }
}
