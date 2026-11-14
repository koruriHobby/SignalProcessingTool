#nullable enable
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SignalProcessingToolApp.Presentations.ViewModels;

namespace SignalProcessingToolApp.Presentations.Views
{
    public partial class MainWindow : Window
    {
        private const double AmplitudeAxisMinimum = -1.2;
        private const double AmplitudeAxisMaximum = 1.2;
        private const int SampleAxisMinimum = 0;

        private readonly MainWindowViewModel _viewModel;

        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;
            DataContext = _viewModel;

            _viewModel.PlotVm.PropertyChanged += (sender, propertyChangedEventArgs) =>
            {
                if (propertyChangedEventArgs.PropertyName == nameof(PlotViewModel.InputWaveformData))
                {
                    UpdateInputWaveform();
                }
                else if (propertyChangedEventArgs.PropertyName == nameof(PlotViewModel.ProcessedWaveformData))
                {
                    UpdateProcessedWaveform();
                }
            };

            _viewModel.ErrorOccurred += (errorMessage) =>
            {
                MessageBox.Show(
                    errorMessage,
                    "エラー",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            };

            InitializePlots();
        }

        private void InitializePlots()
        {
            int initialDataSize = _viewModel.CurrentDataSize;

            waveformPlot.Plot.Title("Input Signal");
            waveformPlot.Plot.XLabel("Sample");
            waveformPlot.Plot.YLabel("Amplitude");
            waveformPlot.Plot.Axes.SetLimits(SampleAxisMinimum, initialDataSize - 1, AmplitudeAxisMinimum, AmplitudeAxisMaximum);

            gainedWaveformPlot.Plot.Title("Processed Signal");
            gainedWaveformPlot.Plot.XLabel("Sample");
            gainedWaveformPlot.Plot.YLabel("Amplitude");
            gainedWaveformPlot.Plot.Axes.SetLimits(SampleAxisMinimum, initialDataSize - 1, AmplitudeAxisMinimum, AmplitudeAxisMaximum);
        }

        private void UpdateInputWaveform()
        {
            var data = _viewModel.PlotVm.InputWaveformData;
            if (data == null || data.Length == 0)
            {
                waveformPlot.Plot.Clear();
                waveformPlot.Refresh();
                return;
            }

            waveformPlot.Plot.Clear();
            waveformPlot.Plot.Add.Scatter(
                xs: Enumerable.Range(0, data.Length).Select(index => (double)index).ToArray(),
                ys: data);

            waveformPlot.Plot.Axes.SetLimits(SampleAxisMinimum, _viewModel.CurrentDataSize - 1, AmplitudeAxisMinimum, AmplitudeAxisMaximum);
            waveformPlot.Refresh();
        }

        private void UpdateProcessedWaveform()
        {
            var data = _viewModel.PlotVm.ProcessedWaveformData;
            if (data == null || data.Length == 0)
            {
                gainedWaveformPlot.Plot.Clear();
                gainedWaveformPlot.Refresh();
                return;
            }

            gainedWaveformPlot.Plot.Clear();
            gainedWaveformPlot.Plot.Add.Scatter(
                xs: Enumerable.Range(0, data.Length).Select(index => (double)index).ToArray(),
                ys: data);

            gainedWaveformPlot.Plot.Axes.SetLimits(SampleAxisMinimum, _viewModel.CurrentDataSize - 1, AmplitudeAxisMinimum, AmplitudeAxisMaximum);
            gainedWaveformPlot.Refresh();
        }
    }

    public class EqualityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is int intValue && parameter is string parameterText && int.TryParse(parameterText, out int parameterValue))
            {
                return intValue == parameterValue;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool boolValue && boolValue && parameter is string parameterText && int.TryParse(parameterText, out int parameterValue))
            {
                return parameterValue;
            }
            return Binding.DoNothing;
        }
    }
}