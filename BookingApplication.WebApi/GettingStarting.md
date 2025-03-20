# 📌 API de Gestión de Hoteles, Habitaciones y Reservaciones

## 📖 Descripción

Este proyecto es una **Web API en .NET 6** con **Clean Architecture**, diseñada para gestionar hoteles, habitaciones y reservaciones.  
Se implementan tecnologías y patrones como:

- **MediatR** para la comunicación entre capas.
- **Entity Framework Core** como ORM.
- **Swagger** para la documentación interactiva.
- **Versionado de API** con tres versiones principales.
- **AutoMapper** para la conversión de modelos.
- **Fluent Validation** para la validación de datos.

## 🚀 Tecnologías Utilizadas y Sus Versiones

| <span style="color:yellow;">Tecnología</span>         | <span style="color:orange;">Descripción</span>                               | <span style="color:red;">Versión</span> |
|-------------------|-----------------------------------------|---------|
| .NET             | Framework principal de desarrollo       | <span style="color:red;">6.0</span> |
| MediatR          | Implementación del patrón Mediator      | <span style="color:red;">12.2.0</span> |
| EF Core          | ORM para el manejo de la base de datos  | <span style="color:red;">6.0.23</span> |
| Swagger         | Documentación interactiva de la API     | <span style="color:red;">Ultima Version</span> |
| AutoMapper      | Mapeo automático entre modelos          | <span style="color:red;">12.0.0</span> |
| FluentValidation | Validación de datos de entrada         | <span style="color:red;">11.3.0</span> |
| Versionado de API | Soporte para múltiples versiones      | <span style="color:red;">v1, v2, v3</span> |


---

## 💻 Imagenes de Tecnologías Utilizadas en el Proyecto

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


 ### 📌 Endpoints de la API

### **🔹 Versión  V 1.0 - Hoteles**
#### 🏨 Obtener todos los hoteles
- **Método:** `GET`
- **URL:** `/api/v1/hoteles`
- **Ejemplo en Postman:**
  ![Get Hoteles](https://postman.com/example-screenshot.jpg)

#### 🏨 Obtener un hotel por ID
- **Método:** `GET`
- **URL:** `/api/v1/hoteles/{id}`
- **Parámetros:**
  - `id` _(obligatorio)_ → ID del hotel
- **Respuesta de ejemplo:**
  ```json
  {
    "id": 1,
    "nombre": "Hotel Central",
    "direccion": "Av. Principal 123"
  }

#### 🏨 Para Crear un Hotel
- **Método:** `Post`
- **URL:** `/api/v1/hoteles/{id}`
- **Parámetros:**
  - `id` _(obligatorio)_ → ID del hotel