namespace BookingApplication.Services.Commands.CommandReservations.CommandReservationDto
{
    public class ModelReservationDto_Delete
    {
        public Guid reservationId { get; set; }
        public string customerName { get; set; }

        public ModelReservationHotelDto_Delete hotelReservated { get; set; }

        public ModelReservsationRoomDto_Delete roomReservated { get; set; }

        public List<ModelReservationListDaysReservatedDto_Delete> daysReservated { get; set; }
    }
}
