using Microsoft.Extensions.Options;

namespace SharpNet.Configuration.Extensions
{
    public class NamedOptionWrapper<T> : IOptions<T> where T : class, new()
    {
        private readonly string _name;
        private readonly IOptionsMonitor<T> _monitor;

        public NamedOptionWrapper(string name, IOptionsMonitor<T> monitor)
        {
            _name = name;
            _monitor = monitor;
            Value = monitor.Get(name);
            _monitor.OnChange(OnChanged);
        }

        private void OnChanged(T options, string name)
        {
            if (name == _name)
            {
                Value = _monitor.Get(name);
            }
        }

        public T Value { get; private set; }
    }
}