# 📌 API de Gestión de Hoteles, Habitaciones y Reservaciones

## 📖 Descripción

Este proyecto es una **Web API en .NET 6** con **Clean Architecture**, diseñada para gestionar hosteles, habitaciones y reservaciones.  
Se implementan tecnologías y patrones como:

## 📌 Características Principales  
- 🔹 **.NET 6** como framework de desarrollo.  
- 🔹 **Entity Framework Core** como ORM para acceso a datos en SQL Server.  
- 🔹 **Arquitectura Limpia (Clean Architecture)** para una estructura modular y escalable.  
- 🔹 **Fluent Validation** para validar entradas en métodos `POST`, `DELETE` y `PUT`.  
- 🔹 **AutoMapper** para mapear entidades y DTOs.  
- 🔹 **Middleware de Excepciones Globales** para manejar errores de manera centralizada.  
- 🔹 **Docker** para la implementación y prueba en entornos productivos.

## 🎯 ¿Qué Aprenderás?
Al trabajar con esta API, aprenderás:  
✅ Cómo implementar **.NET 6** con **Clean Architecture** en una Web API.  
✅ Cómo utilizar **Entity Framework Core** para interactuar con **SQL Server**.  
✅ Cómo manejar **validaciones avanzadas** con **Fluent Validation**.  
✅ Cómo mapear objetos eficientemente con **AutoMapper**.  
✅ Cómo manejar **excepciones globales** con **Middleware**.  
✅ Cómo **publicar y probar** la API usando **Docker**.  

## 🚀 Tecnologías Utilizadas y Sus Versiones  

| Tecnología           | Descripción | Versión |
|----------------------|------------|---------|
| <img src="https://github.com/Gust23ccCastillo/AspNetCore_WebApi_BasicConcepts/tree/main/BookingApplication.WebApi/Img/dotnetcore-original.svg" width="60"> | Framework principal de desarrollo | 🟦 6.0 |
| <img src="Img/EfCore.png" width="60"> | ORM para el manejo de la base de datos | 🟨 6.0 |
| <img src="Img/MediatR.png" width="60"> | Implementación del patrón Mediator | 🟥 Última |
| <img src="Img/fluent-validation-logo.png" width="60"> | Validación de datos de entrada | 🟦 11.0 |
| <img src="Img/Automapper.png" width="60"> | Mapeo automático entre modelos | 🟨 12.0 |
| <img src="Img/docker-original-wordmark.svg" width="60"> | Contenedor para publicar y probar la API | 🟥 Última |

## 💻 Imagenes de Tecnologías Utilizadas

<p align="center" style="background-color: white; padding: 20px;">
    <img src="Img/dotnetcore-original.svg" width="80" style="margin: 10px;">
    <img src="Img/EfCore.png" width="80" style="margin: 10px;">
    <img src="Img/csharp-original.svg" width="80" style="margin: 10px;">
    <img src="Img/MediatR.png" width="100" style="margin: 10px;">
    <img src="Img/fluent-validation-logo.png" width="80" style="margin: 10px;">
    <img src="Img/Automapper.png" width="80" style="margin: 10px;">
    <img src="Img/docker-original-wordmark.svg" width="80" style="margin: 10px;">
    <img src="Img/github-original-wordmark.svg" width="80" style="margin: 10px;">
    <img src="Img/postman-original-wordmark.svg" width="80" style="margin: 10px;">
    <img src="Img/swagger-original-wordmark.svg" width="80" style="margin: 10px;">
</p>



## 🏗️ Arquitectura del Proyecto

```bash


📂 src
 ├── 📁 BookingApplication.Dal  ->  # Lógica de negocio, validaciones y comandos MediatR
 ├── 📁 BookingApplication.Domain -> # Entidades principales de la aplicación
 ├── 📁 BookingApplication.Services -> # Persistencia con EF Core, Repositorios, Unit of Work
 ├── 📁 BookingApplication.WebApi  -> # Controladores y configuración de la API
```
## 📄 Documentación de la API en Postman

La documentación completa de la API de Gestión de Hoteles, Habitaciones y Reservaciones puede consultarse en el siguiente enlace:
<div style="display: flex; align-items: center;">
  <img src="Img/postman-original-wordmark.svg" width="80" style="margin-right: 5px;">
  <span style="font-size: 20px; margin-right: 5px;">➡️</span>
  <a href="https://documenter.getpostman.com/view/19070925/2sAYkGKeYP" style="font-size: 18px; text-decoration: none;">
    🔗 Ver Documentación en Postman
  </a>
</div>


 ### 📌Algunos ejemplos de la API en Postman:

### **🔹 Versión  V 1.0 - Hoteles**
#### 🏨 Obtener todos los hoteles
- **Método:** `GET`
- **URL:** `(https://localhost:/api/v1/HotelActions/HotelActions/GetAllHotelsInApplication)`
- **Ejemplo en Postman:**
  ```json
  [
  {
    "hotel_Id_Information": "2fa02f92-f5b3-4584-8c75-7738e02084c0",
    "hotelName": "El Clan Hostel",
    "hotelRating": 4,
    "address": "Detrás de Koki Beach Restaurant, Avenida 69 entre calles 217 y 219",
    "city": "Puerto Viejo",
    "country": "Limon"
  },
  {
    "hotel_Id_Information": "2db3cb06-ad8c-4f4e-8fb5-f67b327d5191",
    "hotelName": "Hotel Green Mountain",
    "hotelRating": 4,
    "address": "400 metros al Norte del Hogar de Ancianos, El Recreo Turrialba, Turrialba, Cartago",
    "city": "Cartago",
    "country": "Costa Rica"
  },
  {
    "hotel_Id_Information": "69cbf217-d263-4b14-ae12-fb0e20e90d9f",
    "hotelName": "Hotel Arenal Rabfer",
    "hotelRating": 4,
    "address": "175 metros norte del banco Nacional, La Fortuna, Alajuela, 21007",
    "city": "Alajuela",
    "country": "Costa Rica"
  }
]


### **🔹 Versión  V 2.0 - Habitaciones de Hoteles**
#### 🏨 Crea una habitacion
- **Método:** `POST`
- **URL:** `(https://localhost:/api/v2/RoomsActions/CreateRooms)`
- **Parámetros:**
  - `hotelIdParameter` _(obligatorio)_ → ID del hotel
  - `roomNumberParameter` _(obligatorio)_ → Número de la habitación del hotel
  - `sizeRoomParameter` _(obligatorio)_ → Tamaño de la habitación del hotel
  -  `needsRepairParameter` _(obligatorio)_ → Reperaciones de la habitación del hotel
  
  - **Respuesta de ejemplo 200 OK:**
  ```json
  {

  }
- **Respuesta de ejemplo 404 Not Found:**
  ```json
  {
        "errors": {
      "messageInformation": "El hotel especificado no existe en el sistema."
    }
  }
    

- **Respuesta de ejemplo 400 Bad Request:**
   ```json
   {
  "errors": {
    "messageInformation": "El Numero de habitacion a crear, Ya se encuetra registrado, Porfavor ingrese otro numero de habitacion valido!."
  }
}

## 🎉 ¡Gracias por llegar hasta aquí!  
Tu interés en este proyecto es realmente valioso. ¡Espero que te haya sido útil y que puedas aplicar lo aprendido en tus propias implementaciones!  

💡 *"El éxito es la suma de pequeños esfuerzos repetidos día tras día."* – Robert Collier  

🚀 **¡Sigue adelante y nunca dejes de aprender!** 😃  
