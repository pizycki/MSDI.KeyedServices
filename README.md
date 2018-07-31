# MSDI Keyed Services
_Microsoft Dependency Injection Keyed Services_

[![NuGet](https://img.shields.io/nuget/v/MSDI.KeyedServices.svg)](https://www.nuget.org/packages/MSDI.KeyedServices)[![Build status](https://ci.appveyor.com/api/projects/status/af95g68wsxrysv5a/branch/master?svg=true)](https://ci.appveyor.com/project/pizycki/msdi-keyedservices/branch/master)


Plugin for [Microsoft.Extensions.DependencyInjection](https://github.com/aspnet/DependencyInjection) enabling registration of multiple service implemantations under the same interface.

You can think about it as equivalent of [Autofac Keyed Services](http://autofaccn.readthedocs.io/en/latest/advanced/keyed-services.html) or [Ninject Named bindings](https://github.com/ninject/Ninject/wiki/Contextual-Binding#simple-constrained-resolution-named-bindings).

## How to use

Install package

```
dotnet add package MSDI.KeyedServices
```

Register service with one extensions method call passing key and registration action

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddKeyedService<IGreeter, EnglishGreeter, Language>(
        key: Language.En, 
        registration: serviceCollection => 
        {            
        	sc.AddTransient<EnglishGreeter>();
        });
}
```

Or do it separately

```csharp
services.AddKeyedService<IGreeter, PolishGreeter, Language>(key: Language.Pl, registration: null);
services.AddTransient<PolishGreeter>();
```

 In both cases, remember to not wire implementation types with service they implement.

To resolve registered services you have two options

#### Dependency Injection via constructor

```csharp
[ApiController]
public class GreetingsController : ControllerBase
{
    private readonly IKeyedServiceProvider<IGreeter, Language> _greeterProvider;

    public GreetingsController(IKeyedServiceProvider<IGreeter, Language> greeterProvider)
    {
        _greeterProvider = greeterProvider;
    }

    [HttpGet, Route("api/hello")]
    public IActionResult SayHello(string lang)
    {
        if (Enum.TryParse<Language>(lang, ignoreCase: true, result: out var language))
        {
            IGreeter greeter = _greeterProvider.GetKeyedService(language);
            string greetings = greeter.Greet();
            return Ok(greetings);
        }

        return BadRequest("Unknown language");
    }
}
```

To try it out for yourself, download this repository, run `MSDI.KeyedServices.Example` project and call `http://localhost:59792/api/hello?lang=pl` in your browser. 

You can find presented examples in `GreetingsController`. Put some breakpoints and play around.

#### Service Locator

Sometime [service locator pattern](https://en.wikipedia.org/wiki/Service_locator_pattern) is a way to go. 

Here is how you can use it with this library.

```csharp
[HttpGet, Route("api/hello/en")]
public IActionResult SayHelloEn()
{
    var keyedServiceProvider =
        _serviceProvider.GetService(typeof(IKeyedServiceProvider<IGreeter, Language>)) 
        as IKeyedServiceProvider<IGreeter, Language>;

    IGreeter greeter = keyedServiceProvider.GetKeyedService(Language.En);

    string greetings = greeter.Greet();
    return Ok(greetings);
}
```

