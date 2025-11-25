# 💻 TiendaApi

Este proyecto contiene la **API RESTful** del sistema de tienda, desarrollada con **.NET Core 9.0**.

-----

## ⚙️ Requisitos Previos

Antes de empezar, asegúrate de tener instalado:

  * **SDK de .NET:** Versión **9.0** o superior.
  * **Base de Datos:** El servidor de base de datos que estés utilizando (ej. SQL Server, PostgreSQL).

-----

## Instalación y Configuración

Para poner la API en funcionamiento localmente, sigue los siguientes pasos.

### 1\. Clonación del Repositorio

Clona el proyecto y navega al directorio:

```bash
git clone https://github.com/sebascg17/TiendaApi.git
cd TiendaApi
```

### 2\. Restauración de Dependencias

Restaura todos los paquetes NuGet necesarios para el proyecto:

```bash
dotnet restore
```

### 3\. Configuración de la Base de Datos

1.  **Configura la Conexión:** Abre el archivo **`appsettings.json`** y actualiza la cadena de conexión (`ConnectionStrings`) para apuntar a tu base de datos local.
2.  **Aplica Migraciones (EF Core):** Si utilizas Entity Framework Core, aplica las migraciones pendientes:
    ```bash
    dotnet ef database update
    ```

-----

## Development server

Para iniciar la API en modo de desarrollo, ejecuta:

```bash
dotnet run
```

Una vez en ejecución, la API estará disponible en el puerto especificado en `Properties/launchSettings.json` (generalmente `http://localhost:5xxx`).

## Testing

Para ejecutar los *tests* unitarios del proyecto (si están implementados), usa el siguiente comando:

```bash
dotnet test
```

## Building

Para compilar el proyecto y generar los artefactos de producción (binarios), ejecuta:

```bash
dotnet build --configuration Release
```

Esto compilará tu proyecto y guardará los archivos `.dll` resultantes en el directorio `bin/Release/net9.0/publish`.

-----

## Additional Resources

Para más información sobre los comandos del SDK de .NET, visita la [referencia oficial del comando `dotnet`](https://www.google.com/search?q=%5Bhttps://learn.microsoft.com/es-es/dotnet/core/tools/dotnet%5D\(https://learn.microsoft.com/es-es/dotnet/core/tools/dotnet\)).
