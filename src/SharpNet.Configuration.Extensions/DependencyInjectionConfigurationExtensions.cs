using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;

namespace SharpNet.Configuration.Extensions
{
    public static class DependencyInjectionConfigurationExtensions
    {
        public static IServiceCollection Configure<T>(this IServiceCollection services, string sectionName) where T : class
        {
            services.AddOptions();
            return services.AddSingleton<IConfigureOptions<T>>(sp => new ConfigureOption<T>(sp.GetRequiredService<IConfiguration>().GetSection(sectionName)));
        }

        public static IServiceCollection Configure<T>(this IServiceCollection services, string name, string sectionName) where T : class
        {
            services.AddOptions();
            return services.AddSingleton<IConfigureOptions<T>>(sp => new ConfigureNamedOption<T>(name, sp.GetRequiredService<IConfiguration>().GetSection(sectionName)));
        }

        public static IServiceCollection Configure<T>(this IServiceCollection services, Action<IServiceProvider, T> configure) where T : class
        {
            services.AddOptions();
            return services.AddSingleton<IConfigureOptions<T>>(sp => new ConfigureOptionsWithSp<T>(sp, configure));
        }

        public static IServiceCollection TryConfigure<T>(this IServiceCollection services, string sectionName) where T : class
        {
            services.AddOptions();
            services.TryAddSingleton<IConfigureOptions<T>>(sp => new ConfigureOption<T>(sp.GetRequiredService<IConfiguration>().GetSection(sectionName)));
            return services;
        }

        public static IServiceCollection TryConfigure<T>(this IServiceCollection services, string name, string sectionName) where T : class
        {
            services.AddOptions();
            services.TryAddSingleton<IConfigureOptions<T>>(sp => new ConfigureNamedOption<T>(name, sp.GetRequiredService<IConfiguration>().GetSection(sectionName)));
            return services;
        }

        public static IServiceCollection TryConfigure<T>(this IServiceCollection services, Action<IServiceProvider, T> configure) where T : class
        {
            services.AddOptions();
            services.TryAddSingleton<IConfigureOptions<T>>(sp => new ConfigureOptionsWithSp<T>(sp, configure));
            return services;
        }

        #region IConfigureOptions implementations

        private sealed class ConfigureOption<T> : IConfigureOptions<T> where T : class
        {
            private readonly IConfiguration _configuration;

            public ConfigureOption(IConfiguration configuration)
            {
                _configuration = configuration;
            }

            void IConfigureOptions<T>.Configure(T options)
            {
                _configuration.Bind(options);
            }
        }

        private sealed class ConfigureNamedOption<T> : IConfigureNamedOptions<T> where T : class
        {
            private readonly IConfiguration _configuration;
            private readonly string _name;

            public ConfigureNamedOption(string name, IConfiguration configuration)
            {
                _name = name;
                _configuration = configuration;
            }

            void IConfigureNamedOptions<T>.Configure(string name, T options)
            {
                ConfigurePrivate(name, options);
            }

            void IConfigureOptions<T>.Configure(T options)
            {
                ConfigurePrivate(Microsoft.Extensions.Options.Options.DefaultName, options);
            }

            private void ConfigurePrivate(string name, T options)
            {
                if (name == _name)
                {
                    _configuration.Bind(options);
                }
            }
        }

        private sealed class ConfigureOptionsWithSp<T> : IConfigureOptions<T> where T : class
        {
            private readonly Action<IServiceProvider, T> _configure;
            private readonly IServiceProvider _serviceProvider;

            public ConfigureOptionsWithSp(IServiceProvider serviceProvider, Action<IServiceProvider, T> configure)
            {
                _serviceProvider = serviceProvider;
                _configure = configure;
            }

            public void Configure(T options)
            {
                _configure(_serviceProvider, options);
            }
        }

        #endregion IConfigureOptions implementations
    }
}