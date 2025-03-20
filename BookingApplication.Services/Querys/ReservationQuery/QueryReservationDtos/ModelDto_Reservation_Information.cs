using BookingApplication.Domain.Models;

namespace BookingApplication.Services.Querys.ReservationQuery.QueryReservationDtos
{
    public class ModelDto_Reservation_Information
    {
         public Guid reservationId { get; set; }
        public ModelDto_Hotel_Information? hotel_Information { get; set; }
        public ModelDto_Room_Information? room_Information { get; set; }
        public List<ModelDto_InfoDaysReservated> daysOfReservation { get; set; }
        public string? nameClient { get; set; }
    }
}
