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

namespace SignalProcessingTool
{
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;

            _viewModel.PlotVm.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(PlotViewModel.InputWaveformData))
                {
                    UpdateInputWaveform();
                }
                else if (e.PropertyName == nameof(PlotViewModel.ProcessedWaveformData))
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
            int initialDataSize = _viewModel.CommonParamVm.DataSizes[_viewModel.CommonParamVm.DataSizeIndex];

            waveformPlot.Plot.Title("Input Signal");
            waveformPlot.Plot.XLabel("Sample");
            waveformPlot.Plot.YLabel("Amplitude");
            waveformPlot.Plot.Axes.SetLimits(0, initialDataSize - 1, -1.2, 1.2);

            gainedWaveformPlot.Plot.Title("Processed Signal");
            gainedWaveformPlot.Plot.XLabel("Sample");
            gainedWaveformPlot.Plot.YLabel("Amplitude");
            gainedWaveformPlot.Plot.Axes.SetLimits(0, initialDataSize - 1, -1.2, 1.2);
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
                xs: Enumerable.Range(0, data.Length).Select(i => (double)i).ToArray(),
                ys: data);

            int currentDataSize = _viewModel.CommonParamVm.DataSizes[_viewModel.CommonParamVm.DataSizeIndex];
            waveformPlot.Plot.Axes.SetLimits(0, currentDataSize - 1, -1.2, 1.2);
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
                xs: Enumerable.Range(0, data.Length).Select(i => (double)i).ToArray(),
                ys: data);

            int currentDataSize = _viewModel.CommonParamVm.DataSizes[_viewModel.CommonParamVm.DataSizeIndex];
            gainedWaveformPlot.Plot.Axes.SetLimits(0, currentDataSize - 1, -1.2, 1.2);
            gainedWaveformPlot.Refresh();
        }
    }

    public class EqualityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is int intValue && parameter is string paramStr && int.TryParse(paramStr, out int paramValue))
            {
                return intValue == paramValue;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool boolValue && boolValue && parameter is string paramStr && int.TryParse(paramStr, out int paramValue))
            {
                return paramValue;
            }
            return Binding.DoNothing;
        }
    }
}