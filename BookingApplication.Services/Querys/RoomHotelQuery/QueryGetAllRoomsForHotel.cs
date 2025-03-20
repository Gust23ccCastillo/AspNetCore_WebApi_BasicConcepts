using AutoMapper;
using BookingApplication.Dal;
using BookingApplication.Domain.Models;
using BookingApplication.Services.MiddlewareGlobal;
using BookingApplication.Services.Querys.RoomHotelQuery.QueryRoomHotelDto;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BookingApplication.Services.Querys.RoomHotelQuery
{
    public class QueryGetAllRoomsForHotel
    {
        public class GetAllRoomForSpecificHotel : IRequest<List<ModelDtoRoomInformation>> 
        {
          
           public Guid valueHotelIdParameter { get; set; }
        }
        public class FluentValidationData : AbstractValidator<GetAllRoomForSpecificHotel>
        {
            public FluentValidationData()
            {
                RuleFor(xHotelIdParameter => xHotelIdParameter.valueHotelIdParameter).NotEmpty().WithMessage("Debe Ingresar el 'Id' del Hotel!!")
                    .Must(BeAValidGuid).WithMessage("El 'Id' de Hotel proporcionado no es el dato esperado!!.");
            }
            private bool BeAValidGuid(Guid id)
            {
                return Guid.TryParse(id.ToString(), out _);
            }
        }

        public class ModelServiceAndInformationLogic : IRequestHandler<GetAllRoomForSpecificHotel, List<ModelDtoRoomInformation>>
        {
            private readonly DbContextProyect _DbContextProyectInject;
            private readonly IMapper _AutomapperInject;
            public ModelServiceAndInformationLogic(DbContextProyect DbContextProyectInject, IMapper AutomapperInject)
            {
                this._DbContextProyectInject = DbContextProyectInject;
                this._AutomapperInject = AutomapperInject;
            }
            public async Task<List<ModelDtoRoomInformation>> Handle(GetAllRoomForSpecificHotel request, CancellationToken cancellationToken)
            {
                
                cancellationToken.ThrowIfCancellationRequested();

                var hotelExists = await _DbContextProyectInject._TableHotels
                    .AnyAsync(searchHotelById => searchHotelById.HotelId == request.valueHotelIdParameter, cancellationToken);

                if(hotelExists == false)
                {
                    throw new ExecuteMiddlewareGlobalOfProyect(HttpStatusCode.NotFound,
                        new { MessageInformation = "El hotel especificado no existe en el sistema." });
                }

                cancellationToken.ThrowIfCancellationRequested();

                var getListRoomsForSpecificHotel = await this._DbContextProyectInject._TableRooms
                    .AsNoTracking()
                    .Where(conditions => conditions.HotelId == request.valueHotelIdParameter)
                    .Include(includeHotelInfo => includeHotelInfo.Hotel)
                    .Select(selectPropertyRooms => new Room { RoomId = selectPropertyRooms.RoomId,
                                  RoomNumber = selectPropertyRooms.RoomNumber, 
                                  RoomSize = selectPropertyRooms.RoomSize, 
                                  NeedRepair = selectPropertyRooms.NeedRepair})
                   .ToListAsync(cancellationToken);

                if(getListRoomsForSpecificHotel.Count <= 0) 
                {
                    throw new ExecuteMiddlewareGlobalOfProyect(HttpStatusCode.NotFound, 
                        new { MessageInformation = "El Hotel no cuenta todavia con habitaciones creadas.." });
                }
              
                cancellationToken.ThrowIfCancellationRequested();
                return this._AutomapperInject.Map<List<ModelDtoRoomInformation>>(getListRoomsForSpecificHotel);

            }
        }

    }
}
