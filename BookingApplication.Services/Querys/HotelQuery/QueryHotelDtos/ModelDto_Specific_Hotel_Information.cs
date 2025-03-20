namespace BookingApplication.Services.Querys.HotelQuery.QueryHotelDtos
{
    public class ModelDto_Specific_Hotel_Information
    {
        public string? HotelName { get; set; }
        public int HotelRating { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public List<ModelDto_Hotel_Room_Information> RoomsForHotel { get; set; }

    }
}
