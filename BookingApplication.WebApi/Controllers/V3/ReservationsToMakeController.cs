using Asp.Versioning;
using BookingApplication.Services.Commands.CommandReservations;
using BookingApplication.Services.Querys.ReservationQuery;
using BookingApplication.Services.Querys.ReservationQuery.QueryReservationDtos;
using Microsoft.AspNetCore.Mvc;

namespace BookingApplication.WebApi.Controllers.V3
{
   
    [ApiController]
    [ApiVersion("3.0")]
    [Route("api/v{version:apiVersion}/[controller]")]

    public class ReservationsToMakeController : FatherBaseInProyectController
    {
        [MapToApiVersion("3.0")]
        [HttpGet]
        [Route("GetAllListReservationInformation")]
        public async Task<ActionResult<List<ModelDto_Reservation_Information>>> GetAllListReservationInformation(CancellationToken cancellationToken)
        {
            return await MediatorInjectInProyect.Send(new QueryGetListReservationsForHotel.GetListAllReservationForSpecificHotel(),cancellationToken);
        }

        
        [MapToApiVersion("3.0")]
        [HttpGet]
        [Route("GetSpecificReservationById")]
        public async Task<ActionResult<ModelDto_Reservation_Information>> GetSpecificReservationById([FromQuery] QueryGetSpecificReservationForHotel.GetSpecificReservationInformation reservationIdParameter,
            CancellationToken cancellationToken)
        {
            return await MediatorInjectInProyect.Send(reservationIdParameter,cancellationToken);
        }

        [MapToApiVersion("3.0")]
        [HttpPost]
        [Route("CreateOfReservationForHotel")]
        public async Task<ActionResult<string>> CreateOfReservationForHotel([FromBody] CommandCreateNewReservationForHotel.CreateNewReservationInformation createReservationTheHotel,
            CancellationToken cancellationToken)
        {
            return await MediatorInjectInProyect.Send(createReservationTheHotel,cancellationToken);
        }

        [MapToApiVersion("3.0")]
        [HttpDelete]
        [Route("DeleteSpecificReservationForHotel")]
        public async Task<ActionResult<string>> DeleteSpecificReservationForHotel([FromBody] CommandDeleteSpecificReservationForHotel.DeleteSpecificReservationInformation reservationIdParameter,
            CancellationToken cancellationToken)
        {
            return await MediatorInjectInProyect.Send(reservationIdParameter, cancellationToken);

        }
    }
}
