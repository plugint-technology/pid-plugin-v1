# Primary Information Identity Plugin

This project is a plugin to use [Primary Information Identity](https://i.primary.com.ar/pid/landing/?s=id.primary.com.ar) services.

## Api Reference

- https://i.primary.com.ar/pid/landing

## Configuration

- `BaseUrl`: Url Api Primary Information Indentity.
- `TimeoutInMinutes`: Timeout for Api requests.
- `SubscriptionKey`: SubscriptionKey.
- `RetryAttempts`: Retry attempts for each request.

**Cache**

PidPlugin have an internal caching strategy. This feature allows to reduce the
number of request to Primary Information Identity, and increase its performance.

- `EntityDataBasicInMinutes`: Total minutes to cache entity data basic response.
- `EntityDataFullInMinutes`: Total minutes to cache entity data full response.
- `SpecialRecordsInMinutes"`: Total minutes to cache special records response.
- `BankAccountDetailInMinutes`: Total minutes to cache bank account details response.
- `BankAccountOwnershipInMinutes`: Total minutes to cache bank account ownership response.

## Usage Sample

Import namespace:

```C#
	using PidPlugin.Extensions;
```

Register and configure PidPlugin service:

```C#
    static void AddPidPlugin(IServiceCollection services, IConfiguration configuration)
    {
        PidPluginSettings pidConnectorSettings = 
            new PidPluginSettings();

        configuration.GetSection("PidPluginSettings")
            .Bind(pidConnectorSettings);

        services.AddPidPlugin(pidConnectorSettings);
    }
```

## Support

You can get support for use this plugin at [Plugint Technology SAS](https://www.plugint.com/).

Mail: support@plugint.com

## License

MIT




# Primary Information Identity Plugin

Este proyecto es un plugin para utilizar los servicios de [Primary Information Identity](https://i.primary.com.ar/pid/landing/?s=id.primary.com.ar).

## Api Referencia

- https://i.primary.com.ar/pid/landing

## Configuración

- `BaseUrl`: Url Api Primary Information Indentity.
- `TimeoutInMinutes`: Timeout para cada api request.
- `SubscriptionKey`: SubscriptionKey.
- `RetryAttempts`: Reintentos por cada request.

**Cache**

PidPlugin tiene una estrategia interna de cache. Esta funcionalidad permite
reducir la cantidad de consultas a Primary Information Identity, e 
incrementar su performance.

- `EntityDataBasicInMinutes`: Total en minutos de cache para entity data basic.
- `EntityDataFullInMinutes`: Total en minutos de cache para entity data full.
- `SpecialRecordsInMinutes"`: Total en minutos de cache para special records.
- `BankAccountDetailInMinutes`: Total en minutos de cache para bank account details.
- `BankAccountOwnershipInMinutes`: Total en minutos de cache para bank account ownership.

## Ejemplo de uso

Importar namespace:

```C#
    using PidPlugin.Extensions;
```

Registrar y configurar el servicio:

```C#
    static void AddPidPlugin(IServiceCollection services, IConfiguration configuration)
    {
        PidPluginSettings pidConnectorSettings = 
            new PidPluginSettings();

        configuration.GetSection("PidPluginSettings")
            .Bind(pidConnectorSettings);

        services.AddPidPlugin(pidConnectorSettings);
    }
```

## Soporte

Soporte para el uso de este plugin en [Plugint Technology SAS](https://www.plugint.com/).

Mail: support@plugint.com

## License

MIT