using Microsoft.AspNetCore.Mvc;
using MSDI.KeyedServices.Contracts;
using MSDI.KeyedServices.Example.Services;
using System;

namespace MSDI.KeyedServices.Example.Controllers
{
    [ApiController]
    public class GreetingsController : ControllerBase
    {
        private readonly IKeyedServiceProvider<IGreeter, Language> _greeterProvider;
        private readonly IServiceProvider _serviceProvider;

        public GreetingsController(IServiceProvider serviceProvider, IKeyedServiceProvider<IGreeter, Language> greeterProvider)
        {
            _serviceProvider = serviceProvider;
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
}
