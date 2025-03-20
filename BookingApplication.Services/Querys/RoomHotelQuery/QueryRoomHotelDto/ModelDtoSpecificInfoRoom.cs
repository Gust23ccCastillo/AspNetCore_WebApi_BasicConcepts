namespace BookingApplication.Services.Querys.RoomHotelQuery.QueryRoomHotelDto
{
    public class ModelDtoSpecificInfoRoom
    {
        public int roomNumber { get; set; }
        public double size { get; set; }
        public bool needsRepair { get; set; } = false;
        public List<string> dateReservations { get; set; }
    }
}

