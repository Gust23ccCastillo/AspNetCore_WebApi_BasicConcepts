using Asp.Versioning;
using BookingApplication.Services.Commands.CommandRooms;
using BookingApplication.Services.Commands.CommandRooms.CommandModelDto;
using BookingApplication.Services.Querys.RoomHotelQuery;
using BookingApplication.Services.Querys.RoomHotelQuery.QueryRoomHotelDto;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookingApplication.WebApi.Controllers.V2
{
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class RoomsActionsController : FatherBaseInProyectController
    {
        //HTTPS OF HOTEL ROOMS
        [MapToApiVersion("2.0")]
        [HttpGet]
        [Route("GetAllTheRoomsOfSpecificHotel")]
        public async Task<ActionResult<List<ModelDtoRoomInformation>>> GetAllTheRoomsOfSpecificHotel([FromQuery] QueryGetAllRoomsForHotel.GetAllRoomForSpecificHotel hotelIdParameter, CancellationToken cancellationToken)
        {
            return await MediatorInjectInProyect.Send(hotelIdParameter,cancellationToken);

        }
        [MapToApiVersion("2.0")]
        [HttpGet]
        [Route("ObtainHotelRoomInformationById")]
        public async Task<ActionResult<ModelDtoSpecificInfoRoom>> ObtainHotelRoomInformationById([FromQuery] QueryGetSpecificRoomForSpecificHotel.GetSpecificRoomForSpecificHotelInformation hotelIdAndRoomIdParameter,
            CancellationToken cancellationToken)
        {
            return await MediatorInjectInProyect.Send(hotelIdAndRoomIdParameter,cancellationToken);

        }

        [MapToApiVersion("2.0")]
        [HttpPost]
        [Route("CreateRooms")]
        public async Task<ActionResult<Unit>> CreateRooms([FromBody] CommandCreateRoomForSpecificHotel.CreateNewRoomForSpecificHotelInformation createRoomsParameter,CancellationToken cancellationToken)
        {
            return await MediatorInjectInProyect.Send(createRoomsParameter,cancellationToken);

        }
        [MapToApiVersion("2.0")]
        [HttpPut]
        [Route("UpdateRooms")]
        public async Task<ActionResult<ModelDtoReturnUpdateRooms>> UpdateRooms([FromBody] CommandUpdatedRoomForSpecificHotel.UpdatedRoomForSpecficHotelInformation updateRoomParameter,CancellationToken cancellationToken)
        {
            return await MediatorInjectInProyect.Send(updateRoomParameter, cancellationToken);
        }
        [MapToApiVersion("2.0")]
        [HttpDelete]
        [Route("RemoveRooms")]
        public async Task<ActionResult<Unit>> RemoveRooms([FromQuery] CommandDeleteRoomForSpecificHotel.DeleteRoomForSpecificHotelInformation deleteRoomById,CancellationToken cancellationToken)
        {

            return await MediatorInjectInProyect.Send(deleteRoomById,cancellationToken);
        }
    }
}
