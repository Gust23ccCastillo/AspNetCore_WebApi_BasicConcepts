using Asp.Versioning;
using BookingApplication.Dal;
using BookingApplication.Services.Commands.CommandHotel;
using BookingApplication.Services.Querys.HotelQuery;
using BookingApplication.WebApi.MiddlewareApplication;
using BookingApplication.WebApi.SwaggerConfiguration;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);
//builder.Services.AddValidatorsFromAssemblyContaining<Program>();
//builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddControllers()
    .AddFluentValidation(agregatedFluentValidationConfig => agregatedFluentValidationConfig.RegisterValidatorsFromAssemblyContaining(typeof(CommandCreateHotel.CreateNewHotelInformation)));

builder.Services.AddEndpointsApiExplorer();

//file and change Swagger registration
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<SwaggerDefaultValues>();
});

//USE SQL SERVER AND CONNECTION STRING
builder.Services.AddDbContext<DbContextProyect>(Configuration => { Configuration.UseSqlServer(builder.Configuration.GetRequiredSection("SqlServerConnectionString:StringKey").Value);});

//SERVICES FOR MEDIATR IN PROYECT
builder.Services.AddMediatR(ConfiguratioOfMediatR => ConfiguratioOfMediatR.RegisterServicesFromAssemblies(typeof(QueryGetHotels.ModelServiceAndInformationLogic).Assembly));

//SERVICES FOR AUTOMAPPER IN PROYECT
builder.Services.AddAutoMapper(typeof(CommandUpdatedHotel.ModelServiceAndInformationLogic));

//CONFIGURATION FOR SERVICES VERSIONING API
var apiVersioningBuilder = builder.Services.AddApiVersioning(_Configurations =>
{
    //aceptamos la versión 1.0 si un cliente no especifica la versión de la API
    _Configurations.AssumeDefaultVersionWhenUnspecified = true;
    _Configurations.DefaultApiVersion = new ApiVersion(1, 0);

    _Configurations.ReportApiVersions = true;//PODER VER LAS DIFERENTES VERSIONES QUE EXISTAN
    //_Configurations.ApiVersionReader = ApiVersionReader.Combine(
    ////TRES OPCIONES PARA LEER LA VERSION DE LA API
    //new QueryStringApiVersionReader("api-version"));
    //   new HeaderApiVersionReader("x-version"));
    //new MediaTypeApiVersionReader("version-for-api"));
});
apiVersioningBuilder.AddApiExplorer(
    options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });
var app = builder.Build();

////ESTO ES PARA QUE CREE LAS MIGRACIONES AUTIMATICAS AL LEVANTAR MI SERVIDOR CON DOCKER
//using (var scope = app.Services.CreateScope())
//{
//    var dbContext = scope.ServiceProvider.GetRequiredService<DbContextProyect>();
//    dbContext.Database.Migrate();
//}

if (app.Environment.IsDevelopment())
{
    app.UseMiddleware<GlobalMiddleware>();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var descriptions = app.DescribeApiVersions();

        // Build a swagger endpoint for each discovered API version
        foreach (var description in descriptions)
        {
            var url = $"/swagger/{description.GroupName}/swagger.json";
            var name = description.GroupName.ToUpperInvariant();
            options.SwaggerEndpoint(url, name);
        }
    });
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

