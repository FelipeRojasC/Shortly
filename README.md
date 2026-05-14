# Shortly Platform
Proyecto de acortador de URLs desarrollado como parte del curso de **Arquitecturas de Software** en la **Universidad Católica del Norte**.

## Descripción
Shortly es una aplicación web construida con ASP.NET Core (Razor Pages). El sistema implementa una arquitectura por capas detallada en el diseño del curso: Domain, Infrastructure y Application.

## Stack Tecnológico
* **Framework:** .NET 10.0. (ASP.NET Core).
* **Base de Datos:** SQLite.
* **ORM:** Entity Framework Core.
* **Logging:** Serilog 10.0.0 con enriquecedores de Proceso, Hilos y Caller.
* **Seguridad:** BCrypt.Net-Next 4.2.0.

## Arquitectura del Proyecto
El proyecto está organizado siguiendo el Layout oficial:
* **Domain**: Entidades de negocio (`User`, `Link`).
* **Infrastructure**: Persistencia de datos, migraciones y sembrado (Seed).
* **Application**: Lógica de servicios e interfaces (`IUserService`, `LinkService`).

## Instalación y Ejecución
1.  **Clonar el repositorio:**
    ```bash
    git clone [https://github.com/FelipeRojasC/Shortly.git](https://github.com/FelipeRojasC/Shortly.git)
    cd Shortly
    ```
2.  **Restaurar paquetes e instalar herramientas:** 
    ```bash
    dotnet restore
    dotnet tool install --global dotnet-ef
    ```
3.  **Ejecutar migraciones e iniciar:**
    ```bash
    dotnet build
    dotnet ef database update
    dotnet run
    ```

## Autores
* **Felipe Rojas** - Estudiante de Ingeniería Civil en Computación e Informática, UCN.