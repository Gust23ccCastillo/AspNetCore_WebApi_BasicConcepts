namespace BookingApplication.Services.Commands.CommandRooms.CommandModelDto
{
    public class ModelDtoReturnUpdateRooms
    {
        public Guid IdRoom { get; set; }

        public int roomNumber { get; set; }
        public double sizeRoom { get; set; }

        public bool NeedRepair { get; set; }
    }
}
