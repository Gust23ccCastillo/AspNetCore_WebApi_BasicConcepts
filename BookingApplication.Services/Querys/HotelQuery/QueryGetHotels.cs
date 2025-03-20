using AutoMapper;
using BookingApplication.Dal;
using BookingApplication.Domain.Models;
using BookingApplication.Services.MiddlewareGlobal;
using BookingApplication.Services.Querys.HotelQuery.QueryHotelDtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BookingApplication.Services.Querys.HotelQuery
{

    public class QueryGetHotels
    {
        public class GetListHotelInformation : IRequest<List<ModelDto_Hotel_List_Information>> { }

        public class ModelServiceAndInformationLogic : IRequestHandler<GetListHotelInformation, List<ModelDto_Hotel_List_Information>>
        {
            private readonly DbContextProyect _DbContextProyectInject;
            private readonly IMapper _AutomapperInject;
            public ModelServiceAndInformationLogic(DbContextProyect DbContextProyectInject, IMapper AutomapperInject)
            {
               this._DbContextProyectInject = DbContextProyectInject;
              this._AutomapperInject = AutomapperInject;
            }

            public async Task<List<ModelDto_Hotel_List_Information>> Handle(GetListHotelInformation request, CancellationToken cancellationToken)
            {
                    // Verificar si la solicitud ha sido cancelada antes de ejecutar la consulta
                    cancellationToken.ThrowIfCancellationRequested();
                    var getAllsHotelInformation = await this._DbContextProyectInject._TableHotels.AsNoTracking()
                    .Select(xSelect => new Hotel
                    {
                        HotelId = xSelect.HotelId,
                        HotelName = xSelect.HotelName,
                        StarsAssigned = xSelect.StarsAssigned,
                        City = xSelect.City,
                        Country = xSelect.Country,
                        Address = xSelect.Address
                    }).ToListAsync(cancellationToken);

                    // Verificar si la solicitud fue cancelada después de la consulta
                    cancellationToken.ThrowIfCancellationRequested();

                    if (getAllsHotelInformation.Count <= 0)
                    {
                       throw new ExecuteMiddlewareGlobalOfProyect(HttpStatusCode.NotFound, 
                           new { MessageInformation = "No se encuentran registros de hoteles de momento.." });   
                    }
                    // Verificar nuevamente antes de hacer el mapeo
                    cancellationToken.ThrowIfCancellationRequested();

                    return this._AutomapperInject.Map<List<ModelDto_Hotel_List_Information>>(getAllsHotelInformation);
            }
        }
    }
}
