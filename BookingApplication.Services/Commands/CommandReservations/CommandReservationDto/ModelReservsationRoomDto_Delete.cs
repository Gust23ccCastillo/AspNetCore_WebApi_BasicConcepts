namespace BookingApplication.Services.Commands.CommandReservations.CommandReservationDto
{
    public class ModelReservsationRoomDto_Delete
    {
        public Guid idRoom { get; set; }
        public int roomNumber { get; set; }
        public double size { get; set; }
        public bool needsRepair { get; set; } = false;
        public List<ModelDaysReservationsRoomInfo_Dto> infoDaysReservate { get; set; }
    }

}
