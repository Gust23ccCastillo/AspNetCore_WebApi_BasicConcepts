using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BookingApplication.WebApi.SwaggerConfiguration
{
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;

        //También necesitamos una forma de informarle a Swashbuckle sobre las versiones de API en la aplicación. 
        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) => _provider = provider;

        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
            }
        }

        private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var info = new OpenApiInfo()
            {
                Title = "Web Api para Crear,Eliminar,Actualizar y Listar hoteles, habitaciones y mucho mas!",
                Version = description.ApiVersion.ToString(),
                Description = "Esta Api contiene la Funcionalidad para Crear Hoteles, Habitaciones de Hoteles y Reservaciones en un Hotel en Especifico.",
                Contact = new OpenApiContact { Name = "Author Name", Email = "SoftwareLife506@gmail.com" },
                License = new OpenApiLicense { Name = "MIT", Url = new Uri("https://opensource.org/licenses/MIT") }
            };

            if (description.IsDeprecated)
            {
                info.Description += "Esta Version de API esta Desactualizada..";
            }

            return info;
        }
    }
}
