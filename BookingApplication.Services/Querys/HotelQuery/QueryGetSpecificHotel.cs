using AutoMapper;
using BookingApplication.Dal;
using BookingApplication.Services.MiddlewareGlobal;
using BookingApplication.Services.Querys.HotelQuery.QueryHotelDtos;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BookingApplication.Services.Querys.HotelQuery
{
    public class QueryGetSpecificHotel
    {
        public class GetSpecificHotelInformation: IRequest<ModelDto_Specific_Hotel_Information> 
        {
            public Guid idHotelParameter { get; set; }
        }

        public class FluentValidationData : AbstractValidator<GetSpecificHotelInformation>
        {
            public FluentValidationData()
            {
                RuleFor(xHotelIdParameter => xHotelIdParameter.idHotelParameter).NotEmpty().WithMessage("Debe Ingresar el 'Id' del Hotel!!")
                    .Must(BeAValidGuid).WithMessage("El 'Id' de Hotel proporcionado no es el dato esperado!!.");
            }
            private bool BeAValidGuid(Guid id)
            {
                // Si el tipo es Guid, esta validación se garantiza.
                return Guid.TryParse(id.ToString(), out _);
            }
        }

        public class ModelServiceAndInformationLogic : IRequestHandler<GetSpecificHotelInformation, ModelDto_Specific_Hotel_Information>
        {
            private readonly DbContextProyect _DbContextProyectInject;
            private readonly IMapper _AutomapperInject;
            public ModelServiceAndInformationLogic(DbContextProyect DbContextProyectInject,IMapper AutomapperInject)
            {

                this._DbContextProyectInject = DbContextProyectInject;
                this._AutomapperInject = AutomapperInject;
            }
            public async Task<ModelDto_Specific_Hotel_Information> Handle(GetSpecificHotelInformation request, CancellationToken cancellationToken)
            {
               
                cancellationToken.ThrowIfCancellationRequested();

                var specificInformationHotel = await _DbContextProyectInject._TableHotels
                    .AsNoTracking()
                    .Where(conditions => conditions.HotelId == request.idHotelParameter)
                    .Include(includeRoomsInfo => includeRoomsInfo.ListOfRooms)
                    .FirstOrDefaultAsync(cancellationToken);

                if (specificInformationHotel == null)
                {
                    throw new ExecuteMiddlewareGlobalOfProyect(HttpStatusCode.NotFound,
                        new { MessageInformation = $"El hotel con el ID: {request.idHotelParameter}, no pudo ser encontrado." });
                }
                
                cancellationToken.ThrowIfCancellationRequested();
                return _AutomapperInject.Map<ModelDto_Specific_Hotel_Information>(specificInformationHotel);
            }
        }

       

    }
}
