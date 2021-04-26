# Primary Information Identity Plugin

This project is a plugin to use [Primary Information Identity](https://i.primary.com.ar/pid/landing/?s=id.primary.com.ar) services.

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

Inject and configure PidPlugin service:

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