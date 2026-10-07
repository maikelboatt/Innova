using System.Reflection;
using Innova.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MvvmCross.IoC;
using MvvmCross.Platforms.Wpf.Core;
using QuestPDF;
using QuestPDF.Infrastructure;
using Serilog;
using Serilog.Extensions.Logging;

namespace Innova.Presentation.WPF
{
    public class Setup:MvxWpfSetup<Core.App>
    {
        private IConfigurationRoot _configuration;

        protected override ILoggerProvider? CreateLogProvider() => new SerilogLoggerProvider();

        protected override ILoggerFactory? CreateLogFactory()
        {
            // Build config inline if not yet set — defensive pattern
            _configuration ??= new ConfigurationBuilder()
                               .SetBasePath(AppContext.BaseDirectory)
                               .AddJsonFile("appsettings.json", false, true)
                               .Build();

            Log.Logger = new LoggerConfiguration()
                         .ReadFrom.Configuration(_configuration)
                         .Enrich.FromLogContext()
                         .Enrich.WithMachineName()
                         .WriteTo.Console()
                         .WriteTo.File("logs/innova-.txt", rollingInterval: RollingInterval.Day)
                         .CreateLogger();

            return new SerilogLoggerFactory();
        }

        protected override void InitializeFirstChance( IMvxIoCProvider iocProvider )
        {
            base.InitializeFirstChance(iocProvider);

            // Build configuration here — before CompositionRoot runs
            _configuration = new ConfigurationBuilder()
                             .SetBasePath(AppContext.BaseDirectory)
                             .AddJsonFile("appsettings.json", false, true)
                             .Build();

            // Store it so InitializeLastChance can pass it to CompositionRoot
            iocProvider.RegisterSingleton(_configuration);
        }


        protected override void InitializeLastChance( IMvxIoCProvider iocProvider )
        {
            base.InitializeLastChance(iocProvider);

            Settings.License = LicenseType.Community;

            IConfigurationRoot configuration = iocProvider.Resolve<IConfigurationRoot>();

            CompositionRoot.RegisterAllDependencies(
                iocProvider,
                configuration,
                typeof(Core.App).Assembly,
                PresentationCompositionRoot.RegisterPresentationServices);
        }

        public override IEnumerable<Assembly> GetViewAssemblies() =>
        [
            typeof(Setup).Assembly // Innova.WPF assembly
        ];

        public override IEnumerable<Assembly> GetViewModelAssemblies() =>
        [
            typeof(Core.App).Assembly
        ];
    }
}
