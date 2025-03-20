
using Asp.Versioning;
using BookingApplication.Services.Commands.CommandHotel;
using BookingApplication.Services.Querys.HotelQuery;
using BookingApplication.Services.Querys.HotelQuery.QueryHotelDtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookingApplication.WebApi.Controllers.V1
{

    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class HotelActionsController:ControllerBase
    {
        private readonly IMediator MediatorInjectInProyect;
        public HotelActionsController(IMediator _MediatorInjectInProyect)
        {

            MediatorInjectInProyect = _MediatorInjectInProyect;

        }

        [MapToApiVersion("1.0")]
        [HttpGet]
        [Route("GetAllHotelsInApplication")]
        public async Task<ActionResult<List<ModelDto_Hotel_List_Information>>> GetAllHotelsInApplication(CancellationToken cancellationToken)
        {

             return await this.MediatorInjectInProyect.Send(new QueryGetHotels.GetListHotelInformation(),cancellationToken);
            
        }

        [MapToApiVersion("1.0")]
        [HttpGet]
        [Route("GetHotelById")]
        public async Task<ActionResult<ModelDto_Specific_Hotel_Information>> GetHotelById([FromQuery] QueryGetSpecificHotel.GetSpecificHotelInformation idHotel, 
            CancellationToken cancellationToken)
        {
            return await MediatorInjectInProyect.Send(idHotel,cancellationToken);
        }

        [MapToApiVersion("1.0")]
        [HttpPost]
        [Route("CreateNewHotelInApplication")]
        public async Task<ActionResult<Unit>> CreateNewHotelInApplication([FromBody] CommandCreateHotel.CreateNewHotelInformation createHotelParameter, CancellationToken cancellationToken)
        {
            
           return await MediatorInjectInProyect.Send(createHotelParameter, cancellationToken);
        }

        [MapToApiVersion("1.0")]
        [HttpPut]
        [Route("UpdateSpecificHotelInApplication")]
        public async Task<ActionResult<ModelDto_Hotel_List_Information>> UpdateSpecificHotelInApplication([FromBody] CommandUpdatedHotel.UpdatedHotelSpecificInformartion updateInfoSpecificHotelById,CancellationToken cancellationToken)
        {

            return await MediatorInjectInProyect.Send(updateInfoSpecificHotelById, cancellationToken);

        }
        [MapToApiVersion("1.0")]
        [HttpDelete]
        [Route("DeleteSpecificHotelInApplication")]
        public async Task<ActionResult<Unit>> DeleteSpecificHotelInApplication([FromQuery] CommandDeleteHotel.DeleteSpecificHotelInformation removeInfoSpecificHotelById,CancellationToken cancellationToken)
        {
            return await MediatorInjectInProyect.Send(removeInfoSpecificHotelById, cancellationToken);
        }


    }
}
