using AutoMapper;
using BookingApplication.Dal;
using BookingApplication.Domain.Models;
using BookingApplication.Services.MiddlewareGlobal;
using BookingApplication.Services.Querys.HotelQuery.QueryHotelDtos;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;


namespace BookingApplication.Services.Commands.CommandHotel
{
    public class CommandUpdatedHotel
    {
        public class UpdatedHotelSpecificInformartion : IRequest<ModelDto_Hotel_List_Information>
        {
            public Guid idHotelParameter { get; set; }
            public string? hotelNameParameter { get; set; }
            public int? ratingParameter { get; set; }
            public string? addressParameter { get; set; }
            public string? cityParameter { get; set; }
            public string? countryParameter { get; set; }
        }

        public class FluentValidationData : AbstractValidator<UpdatedHotelSpecificInformartion>
        {
            public FluentValidationData()
            {
                RuleFor(xHotelIdParameter => xHotelIdParameter.idHotelParameter)
                    .NotEmpty().WithMessage("Debe Ingresar el 'Id' del Hotel!!")
                    .NotNull().WithMessage("Debe Ingresar el 'Id' del Hotel!!")
                    .Must(BeAValidGuid).WithMessage("El 'Id' de Hotel proporcionado no es el dato esperado!!.");

                RuleFor(xHotelNameParameter => xHotelNameParameter.hotelNameParameter)
                    .NotNull().WithMessage("Debe Ingresar un 'Nombre' para el Hotel.")
                    .NotEmpty().WithMessage("Debe Ingresar un 'Nombre' para el Hotel.")
                    .MaximumLength(100).WithMessage("Debe Ingresar un 'Nombre' de Hotel mas corto!!.")
                    .MinimumLength(5).WithMessage("Debe Ingresar un 'Nombre' de Hotel mas largo!!.");

                RuleFor(xStarAssinedParameter => xStarAssinedParameter.ratingParameter)
                    .NotEmpty().WithMessage("Debe Ingresar la 'Calificacion de Estrellas' del Hotel.")
                    .NotNull().WithMessage("Debe Ingresar la 'Calificacion de Estrellas' del Hotel.")
                    .InclusiveBetween(1, 6).WithMessage("El Número de Estrellas debe ser entre (1) como Mínimo a (6) como Máximo.");

                RuleFor(xAddressParameter => xAddressParameter.addressParameter)
                    .NotEmpty().WithMessage("Debe Ingresar la 'Direccion' del Hotel.")
                    .NotNull().WithMessage("Debe Ingresar la 'Direccion' del Hotel.")
                    .MaximumLength(500).WithMessage("La 'Dirrecion' del Hotel sobrepasa el limite de caracteres!")
                    .MinimumLength(5).WithMessage("La 'Dirrecion' del Hotel es demasiado corto, Debe ingresar una 'Dirrecion' mas larga!!");

                RuleFor(xCityParameter => xCityParameter.cityParameter)
                    .NotEmpty().WithMessage("Debe Ingresar la 'Ciudad' del Hotel.")
                    .NotNull().WithMessage("Debe Ingresar la 'Ciudad' del Hotel.")
                    .MaximumLength(100).WithMessage("La 'Ciudad' donde se encuentra el Hotel sobrepasa el limite de caracteres!!")
                    .MinimumLength(5).WithMessage("La 'Ciudad' donde se encuentra el Hotel es demasiado corto!!");

                RuleFor(xCountryParameter => xCountryParameter.countryParameter)
                    .NotEmpty().WithMessage("Debe Ingresar el 'Pais' donde esta el Hotel.")
                    .NotNull().WithMessage("Debe Ingresar el 'Pais' donde esta el Hotel.")
                    .MaximumLength(100).WithMessage("El 'Pais' donde se encuentra el Hotel sobrepasa el limite de caracteres!!")
                    .MinimumLength(5).WithMessage("El 'Pais' donde se encuentra el Hotel es demasiado corto!!");
            }
            private bool BeAValidGuid(Guid id)
            {
                // Si el tipo es Guid, esta validación se garantiza.
                return Guid.TryParse(id.ToString(), out _);
            }
        }

        public class ModelServiceAndInformationLogic : IRequestHandler<UpdatedHotelSpecificInformartion, ModelDto_Hotel_List_Information>
        {
            private readonly DbContextProyect _DbContextProyectInject;
            private readonly IMapper _AutomapperInject;
            public ModelServiceAndInformationLogic(DbContextProyect DbContextProyectInject, IMapper AutomapperInject)
            {
                _DbContextProyectInject = DbContextProyectInject;
                _AutomapperInject = AutomapperInject;
            }
            public async Task<ModelDto_Hotel_List_Information> Handle(UpdatedHotelSpecificInformartion request, CancellationToken cancellationToken)
            {
                using var timeoutCts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);
                var linkedToken = linkedCts.Token;
                bool transactionCommitted = false; 

                await using var transaction = await _DbContextProyectInject.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    
                    linkedToken.ThrowIfCancellationRequested();
                    var existingHotelInSystem = await _DbContextProyectInject._TableHotels
                     .AsNoTracking()
                     .Where(conditions => conditions.HotelId == request.idHotelParameter)
                     .Select(selectPropertyHotel => new Hotel
                     {
                         HotelId = selectPropertyHotel.HotelId,
                         HotelName = selectPropertyHotel.HotelName,
                         StarsAssigned = selectPropertyHotel.StarsAssigned,
                         Address = selectPropertyHotel.Address,
                         City = selectPropertyHotel.City,
                         Country = selectPropertyHotel.Country
                     }).FirstOrDefaultAsync(cancellationToken);
                    if (existingHotelInSystem == null)
                    {
                        throw new ExecuteMiddlewareGlobalOfProyect(HttpStatusCode.NotFound,
                            new { MessageInformation = "El Hotel a 'Actualizar la informacion', No existe en el sistema, Porfavor intentelo mas tarde.." });
                    }

                    ApplyUpdateHotelInformation(existingHotelInSystem, request, this._DbContextProyectInject);

                    linkedToken.ThrowIfCancellationRequested();
                    var resultToOperation = await _DbContextProyectInject.SaveChangesAsync(cancellationToken);

                    if (resultToOperation <= 0)
                    {
                        throw new ExecuteMiddlewareGlobalOfProyect(HttpStatusCode.InternalServerError, 
                            new { MessageInformation = "Error!!, Problemas con el servidor encargado de 'Editar la Informacion' del Hotel, Porfavor intentelo mas tarde.." });
                    }

                    linkedToken.ThrowIfCancellationRequested();
                    await transaction.CommitAsync(cancellationToken);
                    transactionCommitted = true;

                    return _AutomapperInject.Map<ModelDto_Hotel_List_Information>(existingHotelInSystem);
                }
                catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
                {
                    if (!transactionCommitted) await transaction.RollbackAsync(cancellationToken);
                    throw new ExecuteMiddlewareGlobalOfProyect(HttpStatusCode.RequestTimeout, new
                    {
                        MessageInformation = "¡Tiempo de espera agotado! La actualización del hotel tomó más de 5 minutos y fue cancelada."
                    });
                }
                catch (ExecuteMiddlewareGlobalOfProyect)
                {
                    if (!transactionCommitted) await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
                catch (Exception)
                {
                    if (!transactionCommitted) await transaction.RollbackAsync(cancellationToken);
                    throw new ExecuteMiddlewareGlobalOfProyect(HttpStatusCode.Conflict,
                        new { MessageInformation = "Error al actualizar la información del hotel. Inténtalo nuevamente." });
                }
            }

            private void ApplyUpdateHotelInformation(Hotel hotelInformation, UpdatedHotelSpecificInformartion request, 
                DbContextProyect dbContextProyect)
            {
                hotelInformation.HotelName = request.hotelNameParameter ?? hotelInformation.HotelName;
                hotelInformation.StarsAssigned = request.ratingParameter ?? hotelInformation.StarsAssigned;
                hotelInformation.Address = request.addressParameter ?? hotelInformation.Address;
                hotelInformation.City = request.cityParameter ?? hotelInformation.City;
                hotelInformation.Country = request.countryParameter ?? hotelInformation.Country;
                dbContextProyect._TableHotels.Update(hotelInformation);
            }
        }
    }
}
