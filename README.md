📚 Guía de Instalación del Backend (TiendaApi)
Este repositorio contiene la API de .NET Core que gestiona la lógica de negocio y la base de datos.

⚙️ Requisitos Previos
Asegúrate de tener instaladas las siguientes herramientas:

SDK de .NET: Versión 9.0 o superior (puedes verificarlo ejecutando dotnet --version).

Base de Datos: El servidor de base de datos que estés utilizando (ej. SQL Server, PostgreSQL, etc.).

1. Clonación del Repositorio
Abre tu terminal y clona el repositorio del backend. Navega a la carpeta para empezar a trabajar.

Bash

git clone https://github.com/sebascg17/TiendaApi.git
cd TiendaApi
2. Configuración de Dependencias
Restaura los paquetes NuGet necesarios para el proyecto:

Bash

dotnet restore
3. Configuración de la Base de Datos
Debes configurar la conexión a tu base de datos y aplicar los cambios estructurales.

Edita el archivo de configuración: Abre appsettings.json (o appsettings.Development.json) y asegúrate de que la sección ConnectionStrings tenga la cadena de conexión correcta a tu base de datos local.

Aplica las migraciones (si usas Entity Framework Core para la base de datos):

Bash

dotnet ef database update
4. Ejecución de la API
Una vez configurado, puedes ejecutar la aplicación desde la terminal:

Bash

dotnet run
La API se ejecutará y estará disponible en el puerto especificado en Properties/launchSettings.json (normalmente http://localhost:5237/ o similar). Podrás probarla usando Postman o navegando al endpoint de Swagger si lo tienes configurado.
