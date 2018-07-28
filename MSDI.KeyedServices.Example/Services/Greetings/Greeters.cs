namespace MSDI.KeyedServices.Example.Services
{
    public interface IGreeter
    {
        string Greet();
    }

    public class EnglishGreeter : IGreeter
    {
        public string Greet() => "What's up!";
    }

    public class PolishGreeter : IGreeter
    {
        public string Greet() => "Siema!";
    }
}
