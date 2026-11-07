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
using SignalProcessingToolApp.Presentation.ViewModel;

namespace SignalProcessingTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;

            // PropertyChanged イベントで波形データをプロットに反映
            _viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(MainWindowViewModel.InputWaveformData))
                {
                    UpdateInputWaveform();
                }
                else if (e.PropertyName == nameof(MainWindowViewModel.ProcessedWaveformData))
                {
                    UpdateProcessedWaveform();
                }
            };

            // エラーイベント購読
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
            // グラフの初期設定
            waveformPlot.Plot.Title("Input Signal");
            waveformPlot.Plot.XLabel("Sample");
            waveformPlot.Plot.YLabel("Amplitude");
            waveformPlot.Plot.Axes.SetLimits(0, _viewModel.DataSizes[_viewModel.DataSizeIndex] - 1, -1.2, 1.2);

            gainedWaveformPlot.Plot.Title("Processed Signal");
            gainedWaveformPlot.Plot.XLabel("Sample");
            gainedWaveformPlot.Plot.YLabel("Amplitude");
            gainedWaveformPlot.Plot.Axes.SetLimits(0, _viewModel.DataSizes[_viewModel.DataSizeIndex] - 1, -1.2, 1.2);
        }

        private void UpdateInputWaveform()
        {
            if (_viewModel.InputWaveformData == null || _viewModel.InputWaveformData.Length == 0)
            {
                waveformPlot.Plot.Clear();
                waveformPlot.Refresh();
                return;
            }

            waveformPlot.Plot.Clear();
            waveformPlot.Plot.Add.Scatter(
                xs: Enumerable.Range(0, _viewModel.InputWaveformData.Length).Select(i => (double)i).ToArray(),
                ys: _viewModel.InputWaveformData);
            // X軸を常に [0, データサイズ-1] に固定
            waveformPlot.Plot.Axes.SetLimits(0, _viewModel.CurrentDataSize - 1, -1.2, 1.2);
            waveformPlot.Refresh();
        }

        private void UpdateProcessedWaveform()
        {
            if (_viewModel.ProcessedWaveformData == null || _viewModel.ProcessedWaveformData.Length == 0)
            {
                gainedWaveformPlot.Plot.Clear();
                gainedWaveformPlot.Refresh();
                return;
            }

            gainedWaveformPlot.Plot.Clear();
            gainedWaveformPlot.Plot.Add.Scatter(
                xs: Enumerable.Range(0, _viewModel.ProcessedWaveformData.Length).Select(i => (double)i).ToArray(),
                ys: _viewModel.ProcessedWaveformData);
            // X軸を常に [0, データサイズ-1] に固定
            gainedWaveformPlot.Plot.Axes.SetLimits(0, _viewModel.CurrentDataSize - 1, -1.2, 1.2);
            gainedWaveformPlot.Refresh();
        }
    }

    // ラジオボタンのバインディング用コンバーター
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
