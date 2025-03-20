namespace BookingApplication.Services.Querys.RoomHotelQuery.QueryRoomHotelDto
{
    public class ModelDtoRoomInformation
    {

        public Guid idRoom { get; set; }
        public int roomNumber { get; set; }
        public double size { get; set; }
        public bool needsRepair { get; set; } = false;
        
    }
}
