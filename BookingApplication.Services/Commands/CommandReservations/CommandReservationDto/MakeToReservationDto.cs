using System.ComponentModel.DataAnnotations;

namespace BookingApplication.Services.Commands.CommandReservations.CommandReservationDto
{
    public class MakeToReservationDto
    {
        
        [Required(ErrorMessage = "Debe Ingresar un Hotel a Reservar.")]
        public Guid? HotelIdInformation { get; set; }
        [Required(ErrorMessage = "Debe Ingresar una Habitacion del Hotel a Reservar.")]
        public Guid? RoomIdInformation { get; set; }

        [Required(ErrorMessage = "Debe Ingresar un Fecha de Inicio de la Reservacion.")]
        [DataType(DataType.Date)]
        public DateTime? ReservationCheckInDate { get; set; }

        [Required(ErrorMessage = "Debe Ingresar un Fecha de Finalizacion de la Reservacion.")]
        [DataType(DataType.Date)]
        public DateTime? ReservationCheckOutDate { get; set; }

        [Required(ErrorMessage = "Debe Ingresar el Nombre del Reservante.")]
        [MaxLength(100, ErrorMessage = "El Nombre del Reservante es Demasido Largo, Ingrese un Nombre mas Corto!!")]
        [MinLength(5, ErrorMessage = "El Nombre del Reservante es Muy Corto, Debe Ingresar un Nombre mas Largo!!")]
        public string? NameOfReservation { get; set; }
    }
}
