using BookingApplication.Domain.Models;

namespace BookingApplication.Services.Commands.CommandReservations.CommandReservationDto
{
    public class FilterDuplicateDateModelDto
    {
       public List<RoomReservationDate> ListRoomReservationDates { get; set; }

       public List<HotelReservationDate> ListHotelReservationDates { get; set; }
    }
}
