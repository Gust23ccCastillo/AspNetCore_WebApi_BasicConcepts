using BookingApplication.Dal;
using BookingApplication.Domain.Models;
using BookingApplication.Services.MiddlewareGlobal;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BookingApplication.Services.Commands.CommandRooms
{
    public class CommandCreateRoomForSpecificHotel
    {
        public class CreateNewRoomForSpecificHotelInformation : IRequest<Unit>
        {
            public Guid hotelIdParameter{ get; set; }
            public int? roomNumberParameter { get; set; }
            public double? sizeRoomParameter { get; set; }
            public bool? needsRepairParameter { get; set; } = false;
        }

        public class FluentValidationData : AbstractValidator<CreateNewRoomForSpecificHotelInformation>
        {
            public FluentValidationData()
            {
                RuleFor(xHotelIdParameter => xHotelIdParameter.hotelIdParameter)
                    .NotEmpty().WithMessage("Debe Ingresar el 'Id' del Hotel!!")
                    .Must(BeAValidGuid).WithMessage("El 'Id' de Hotel proporcionado no es el dato esperado!!.");

                RuleFor(xroomNumberParameter => xroomNumberParameter.roomNumberParameter)
                      .NotNull().WithMessage("Debe ingresar un numero de habitacion a crear!!")
                     .InclusiveBetween(1, 150).WithMessage("El número de habitacion a crear debe ser entre (1) como Mínimo a (150) como Máximo."); 

                RuleFor(x => x.sizeRoomParameter)
                     .NotNull().WithMessage("Debe ingresar el tamaño de la habitación en metros cuadrados.")
                     .GreaterThan(0).WithMessage("El tamaño de la habitación debe ser mayor a 0.");

                RuleFor(x => x.needsRepairParameter)
                      .NotNull().WithMessage("Debe especificar si la reparación es necesaria.")
                      .Must(value => value == true || value == false)
                      .WithMessage("El valor debe ser verdadero o falso.");
            }
            private bool BeAValidGuid(Guid id)
            {
                return Guid.TryParse(id.ToString(), out _);
            }
        }

        public class ModelServiceAndInformationLogic : IRequestHandler<CreateNewRoomForSpecificHotelInformation, Unit>
        {
            private readonly DbContextProyect _DbContextProyectInject;
            
            public ModelServiceAndInformationLogic(DbContextProyect DbContextProyectInject)
            {
                this._DbContextProyectInject = DbContextProyectInject;   
            }
            public async Task<Unit> Handle(CreateNewRoomForSpecificHotelInformation request, CancellationToken cancellationToken)
            {
                using var timeoutCts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);
                var linkedToken = linkedCts.Token;
                bool transactionCommitted = false;

                await using var transaction = await _DbContextProyectInject.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    linkedToken.ThrowIfCancellationRequested();

                    var hotelExists = await _DbContextProyectInject._TableHotels
                        .AnyAsync(searchHotelById => searchHotelById.HotelId == request.hotelIdParameter, cancellationToken);
                    if (hotelExists == false)
                    {
                        throw new ExecuteMiddlewareGlobalOfProyect(HttpStatusCode.NotFound,
                            new { MessageInformation = "El hotel especificado no existe en el sistema." });
                    }

                    linkedToken.ThrowIfCancellationRequested();
                    var existingRoomByNumber = await _DbContextProyectInject
                                          ._TableRooms
                                          .AnyAsync(searchRoom =>
                                          searchRoom.HotelId == request.hotelIdParameter &&
                                          searchRoom.RoomNumber == request.roomNumberParameter, cancellationToken);
                                          

                    if (existingRoomByNumber == true)
                    {
                        throw new ExecuteMiddlewareGlobalOfProyect(HttpStatusCode.BadRequest, 
                            new { MessageInformation = $"El Numero de habitacion: '{request.roomNumberParameter}' a crear, Ya se encuetra registrado, Porfavor ingrese otro numero de habitacion valido!." });
                    }

                    var newRoomInfo = ApplyCreateRoomInformation(request);
                    this._DbContextProyectInject._TableRooms.Add(newRoomInfo);

                    linkedToken.ThrowIfCancellationRequested();
                    var _ResultToOperation = await this._DbContextProyectInject.SaveChangesAsync(cancellationToken);

                    if (_ResultToOperation <= 0)
                    {
                        throw new ExecuteMiddlewareGlobalOfProyect(HttpStatusCode.InternalServerError, 
                            new { MessageInformation = "Error!!, Problemas con el servidor encargado de crear las habitaciones del hotel, Porfavor intentelo mas tarde.." });
                    }

                    linkedToken.ThrowIfCancellationRequested();
                    await transaction.CommitAsync(cancellationToken);
                    transactionCommitted = true;
                    return Unit.Value;
                }
                catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
                {
                    if (!transactionCommitted) await transaction.RollbackAsync(cancellationToken);
                    throw new ExecuteMiddlewareGlobalOfProyect(HttpStatusCode.RequestTimeout, new
                    {
                        MessageInformation = "¡Tiempo de espera agotado! La creacion de la habitacion tomó más de 5 minutos y fue cancelada."
                    });
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
                        new { MessageInformation = "Error no se pudo agregar la informacion del la habitacion, Porfavor intentelo nuevamente." });
                }
            }

            private Room ApplyCreateRoomInformation(CreateNewRoomForSpecificHotelInformation informationNewRoom)
            {
                var newRoom = new Room
                {
                    RoomId = Guid.NewGuid(),
                    HotelId = informationNewRoom.hotelIdParameter,
                    RoomNumber = informationNewRoom.roomNumberParameter,
                    RoomSize = informationNewRoom.sizeRoomParameter,
                    NeedRepair = informationNewRoom.needsRepairParameter
                };
                return newRoom;
            }
        }
    } 
}
