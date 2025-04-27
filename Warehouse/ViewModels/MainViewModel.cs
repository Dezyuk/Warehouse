using System.Windows.Controls;
using System.Windows.Input;
using Warehouse.Helper;       // RelayCommand
using Warehouse.Views;        // UserControl-ы, которые будут переключаться
using Warehouse.ViewModels;   // Для ProductViewModel

namespace Warehouse.ViewModels
{
    /// <summary>
    /// ViewModel для главного окна.
    /// Содержит текущий отображаемый UserControl и команды для навигации.
    /// </summary>
    public class MainViewModel : BaseViewModel
    {
        private UserControl _currentView;
        /// <summary>
        /// Текущий отображаемый модуль.
        /// </summary>
        public UserControl CurrentView
        {
            get => _currentView;
            set { _currentView = value; OnPropertyChanged(); }
        }

        // Команды для переключения между модулями.
        public ICommand ShowProductsCommand { get; }
        public ICommand ShowInboundCommand { get; }
        public ICommand ShowOutboundCommand { get; }
        public ICommand ShowTopologyCommand { get; }
        public ICommand ShowHistoryCommand { get; }

        private readonly ProductViewModel _productViewModel;
        private readonly InboundInvoiceViewModel _inboundInvoiceViewModel;
        private readonly OutboundInvoiceViewModel _outboundInvoiceViewModel;
        private readonly InvoiceHistoryViewModel _invoceHistoryViewModel;
        private readonly TopologyViewModel _topologyView;
        public MainViewModel(ProductViewModel productViewModel,
            InboundInvoiceViewModel inboundInvoiceViewModel,
            OutboundInvoiceViewModel outboundInvoiceViewModel,
            InvoiceHistoryViewModel invoceHistoryViewModel,
            TopologyViewModel topologyView)
        {
            _inboundInvoiceViewModel = inboundInvoiceViewModel;
            _productViewModel = productViewModel;
            _outboundInvoiceViewModel = outboundInvoiceViewModel;
            _invoceHistoryViewModel = invoceHistoryViewModel;
            _topologyView = topologyView;
            // По умолчанию отображается окно товаров с передачей зависимостей
            CurrentView = new ProductView(_productViewModel);

            // Инициализация команд. При выполнении команды устанавливаем нужный UserControl.
            ShowProductsCommand = new RelayCommand(() => CurrentView = new ProductView(_productViewModel));
            // Остальные команды можно добавить, если соответствующие UserControl созданы:
            ShowInboundCommand = new RelayCommand(() => CurrentView = new InvoiceView(_inboundInvoiceViewModel));
            ShowOutboundCommand = new RelayCommand(() => CurrentView = new InvoiceView(_outboundInvoiceViewModel));
            ShowHistoryCommand = new RelayCommand(() => CurrentView = new InvoiceHistoryView(_invoceHistoryViewModel));

            ShowTopologyCommand = new RelayCommand(() => CurrentView = new WarehouseTopologyView(_topologyView));

        }
    }
}
