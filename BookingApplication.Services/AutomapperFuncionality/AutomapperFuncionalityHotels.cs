using AutoMapper;
using BookingApplication.Domain.Models;
using BookingApplication.Services.Querys.HotelQuery.QueryHotelDtos;

namespace BookingApplication.Services.AutomapperFuncionality
{
    public class AutomapperFuncionalityHotels:Profile
    {
        public AutomapperFuncionalityHotels()
        {
            //MAPPING TO Hotel A HotelInformationDto
            CreateMap<Hotel, ModelDto_Hotel_List_Information>()
                .ForMember(dest =>
                dest.Hotel_Id_Information,
                opt => opt.MapFrom(src => src.HotelId))
            .ForMember(dest =>
                dest.HotelName,
                opt => opt.MapFrom(src => src.HotelName))
            .ForMember(dest =>
                dest.HotelRating,
                opt => opt.MapFrom(src => src.StarsAssigned))
            .ForMember(dest =>
                dest.Country,
                opt => opt.MapFrom(src => src.Country))
            .ForMember(dest =>
                dest.City,
                opt => opt.MapFrom(src => src.City))
            .ForMember(dest =>
                dest.Address,
                opt => opt.MapFrom(src => src.Address));


          

            //MAPPING TO Hotel A HotelSpecificInformationDto
            CreateMap<Hotel, ModelDto_Specific_Hotel_Information>()
               .ForMember(dest =>
                dest.HotelName,
                opt => opt.MapFrom(src => src.HotelName))
            .ForMember(dest =>
                dest.HotelRating,
                opt => opt.MapFrom(src => src.StarsAssigned))
            .ForMember(dest =>
                dest.Country,
                opt => opt.MapFrom(src => src.Country))
            .ForMember(dest =>
                dest.City,
                opt => opt.MapFrom(src => src.City))
            .ForMember(dest =>
                dest.Address,
                opt => opt.MapFrom(src => src.Address))
             .ForMember(dest => dest.RoomsForHotel, opt => opt.MapFrom(src => src.ListOfRooms)); // Mapeo explícito de la lista



            //MAPPING TO Room A RoomForHotel
            CreateMap<Room, ModelDto_Hotel_Room_Information>()
            .ForMember(dest =>
                dest.RoomNumber,
                opt => opt.MapFrom(src => src.RoomNumber))
            .ForMember(dest =>
                dest.Size,
                opt => opt.MapFrom(src => src.RoomSize))
            .ForMember(dest =>
                dest.NeedsRepair,
                opt => opt.MapFrom(src => src.NeedRepair));
         


        }
    }
}
