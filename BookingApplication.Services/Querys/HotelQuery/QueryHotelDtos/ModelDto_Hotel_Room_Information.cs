using System.ComponentModel.DataAnnotations;

namespace BookingApplication.Services.Querys.HotelQuery.QueryHotelDtos
{
    public class ModelDto_Hotel_Room_Information
    {
        public int RoomNumber { get; set; }
        public double Size { get; set; }
        public bool NeedsRepair { get; set; } = false;
    }
}
