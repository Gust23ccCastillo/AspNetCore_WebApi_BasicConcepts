using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingApplication.Domain.Models
{
    public class Reservation
    {
        [Required]
        [Key]
        public Guid ReservationId { get; set; } = Guid.NewGuid();

        [ForeignKey("Fk_RoomIdReservation")]
        public Guid? RoomId { get; set; }

        public Room? RoomReservated { get; set; }

        [ForeignKey("Fk_HotelIdReservation")]
        public Guid HotelId { get; set; }
       
        public Hotel? HotelReservated { get; set; }
        public List<HotelReservationDate> ListToDateReservatedInHotel {  get; set; }  = new List<HotelReservationDate>();
             
        [Required(ErrorMessage ="Debe Ingresar el Nombre del Reservante.")]
        [MaxLength(100,ErrorMessage ="El Nombre del Reservante es Demasido Largo, Ingrese un Nombre mas Corto!!")]
        [MinLength(5,ErrorMessage ="El Nombre del Reservante es Muy Corto, Debe Ingresar un Nombre mas Largo!!")]
        [DataType("nvarchar(100)")]
        public string? Customer { get; set; }
    }
}
