using BookingApplication.Dal;
using BookingApplication.Domain.Models;
using BookingApplication.Services.MiddlewareGlobal;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BookingApplication.Services.Commands.CommandHotel
{
    public class CommandCreateHotel
    {
        public class CreateNewHotelInformation : IRequest<Unit>
        {
            public string? hotelNameParameter { get; set; }
            public int? RatingParameter { get; set; }
            public string? addressParameter { get; set; }
            public string? cityParameter { get; set; }
            public string? countryParameter { get; set; }
        }

        public class FluentValidationData : AbstractValidator<CreateNewHotelInformation>
        {
            //La validación con FluentValidation ofrece una mayor flexibilidad que los atributos de Data Annotations.
            //Por ejemplo, puedes agregar validaciones condicionales, reglas personalizadas o
            //combinaciones más complejas sin mezclar la lógica de validación con el modelo de datos.
            public FluentValidationData()
            {

                RuleFor(xHotelNameParameter => xHotelNameParameter.hotelNameParameter)
                    .NotNull().WithMessage("Debe Ingresar un 'Nombre' para el Hotel.")
                    .NotEmpty().WithMessage("Debe Ingresar un 'Nombre' para el Hotel.")
                    .MaximumLength(100).WithMessage("Debe Ingresar un 'Nombre' de Hotel mas corto!!."
                    ).MinimumLength(5).WithMessage("Debe Ingresar un 'Nombre' de Hotel mas largo!!.");

                RuleFor(xStarAssinedParameter => xStarAssinedParameter.RatingParameter)
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
        }

        public class ModelServiceAndInformationLogic : IRequestHandler<CreateNewHotelInformation, Unit>
        {
            private readonly DbContextProyect _DbContextProyectInject;
            public ModelServiceAndInformationLogic(DbContextProyect DbContextProyectInject)
            {
                _DbContextProyectInject = DbContextProyectInject;
            }
            public async Task<Unit> Handle(CreateNewHotelInformation request, CancellationToken cancellationToken)
            {
                using var timeoutCts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);
                var linkedToken = linkedCts.Token;
                bool transactionCommitted = false;

                await using var transaction = await _DbContextProyectInject.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    // Verificar si la operación ha sido cancelada antes de consultar la base de datos
                    linkedToken.ThrowIfCancellationRequested();
                    var existingHotelByName = await _DbContextProyectInject._TableHotels
                                   .AnyAsync(searchHotel => searchHotel.HotelName == request.hotelNameParameter &&
                                   searchHotel.Address == request.addressParameter, cancellationToken);

                    if (existingHotelByName == true)
                        throw new ExecuteMiddlewareGlobalOfProyect(HttpStatusCode.BadRequest,
                            new
                            { MessageInformation = $"El Hotel con el nombre: '{request.hotelNameParameter}'  y la dirreccion: '{request.addressParameter}', " +
                               $"Ya se encuetra registrados, Porfavor ingrese otros valores validos!!."});

                    // Verificar si la operación ha sido cancelada antes de continuar
                    linkedToken.ThrowIfCancellationRequested();
                    var newHotel = new Hotel
                    {
                        HotelId = Guid.NewGuid(),
                        HotelName = request.hotelNameParameter,
                        StarsAssigned = request.RatingParameter.Value,
                        Address = request.addressParameter,
                        City = request.cityParameter,
                        Country = request.countryParameter,
                    };

                    this._DbContextProyectInject._TableHotels.Add(newHotel);

                    // Verificar si la operación ha sido cancelada antes de guardar los cambios
                    linkedToken.ThrowIfCancellationRequested();
                    var resultToOperation = await _DbContextProyectInject.SaveChangesAsync(cancellationToken);

                    if (resultToOperation <= 0)
                        throw new ExecuteMiddlewareGlobalOfProyect(HttpStatusCode.InternalServerError,
                            new { MessageInformation = "Error!!, Problemas con el servidor encargado de crear el hotel, Porfavor intentelo mas tarde.." });

                    // Verificar si la operación ha sido cancelada antes de hacer commit
                    linkedToken.ThrowIfCancellationRequested();
                    await transaction.CommitAsync(cancellationToken);
                    transactionCommitted = true; // Marcar que la transacción fue confirmada
                    return Unit.Value;
                }
                catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
                {
                    if (!transactionCommitted) await transaction.RollbackAsync(cancellationToken);
                    throw new ExecuteMiddlewareGlobalOfProyect(HttpStatusCode.RequestTimeout,
                        new { MessageInformation = "¡Tiempo de espera agotado! La creacion del hotel tomó más de 5 minutos y fue cancelada." });
                }
                catch (ExecuteMiddlewareGlobalOfProyect) // Excepciones personalizadas
                {
                    if (!transactionCommitted) await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
                catch (Exception)
                {
                    if (!transactionCommitted) await transaction.RollbackAsync(cancellationToken);
                    throw new ExecuteMiddlewareGlobalOfProyect(HttpStatusCode.Conflict,
                        new { MessageInformation = "Error no se pudo agregar la informacion del nuevo hotel, Porfavor intentelo nuevamente." });
                } 
            }
        }
    }
}
