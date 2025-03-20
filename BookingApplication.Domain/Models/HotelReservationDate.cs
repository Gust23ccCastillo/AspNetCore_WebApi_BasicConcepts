using System.ComponentModel.DataAnnotations;

namespace BookingApplication.Domain.Models
{
    public class HotelReservationDate
    {
        [Key]
        public Guid HotelReservationDateId { get; set; } = Guid.NewGuid();
        public DateTime ReservationDate { get; set; }

        [Required]
        public Guid ReservationId { get; set; }

        public Reservation Reservation { get; set; }
    }
}
