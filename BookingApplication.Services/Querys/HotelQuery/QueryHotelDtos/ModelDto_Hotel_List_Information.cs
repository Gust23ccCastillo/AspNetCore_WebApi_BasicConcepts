namespace BookingApplication.Services.Querys.HotelQuery.QueryHotelDtos
{
    public class ModelDto_Hotel_List_Information
    {
        public Guid Hotel_Id_Information { get; set; }
        public string? HotelName { get; set; }
        public int HotelRating { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
    }
}
