using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using SignalProcessingToolApp.Applications.Factories;
using SignalProcessingToolApp.Applications.Chains;
using SignalProcessingToolApp.Applications.Services;
using SignalProcessingToolApp.Domains.SignalProcessing;
using SignalProcessingToolApp.Domains.WaveformGeneration;
using SignalProcessingToolApp.Presentations.Factories;
using SignalProcessingToolApp.Presentations.Views;
using SignalProcessingToolApp.Presentations.ViewModels;

namespace SignalProcessingToolApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider? _serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();

            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _serviceProvider?.Dispose();
            base.OnExit(e);
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<SineWaveGenerator>();
            services.AddSingleton<UnitImpulseGenerator>();
            services.AddSingleton<WaveformGenerator>();
            services.AddSingleton<FrameProcessor>();
            services.AddSingleton<SignalProcessingChain>();
            services.AddSingleton<SignalProcessingService>();
            services.AddSingleton<ParameterFactory>();
            services.AddSingleton<ConfigFactory>();
            services.AddSingleton<SignalExecutionSettingsFactory>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<MainWindow>();
        }
    }

}
