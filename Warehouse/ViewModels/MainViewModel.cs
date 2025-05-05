using System.Windows.Controls;
using System.Windows.Input;
using Warehouse.Helper;       
using Warehouse.Views;        
using Warehouse.ViewModels;   

namespace Warehouse.ViewModels
{
    
 
    public class MainViewModel : BaseViewModel
    {
        private UserControl _currentView;
        public UserControl CurrentView
        {
            get => _currentView;
            set { _currentView = value; OnPropertyChanged(); }
        }

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
            CurrentView = new ProductView(_productViewModel);


            ShowProductsCommand = new RelayCommand(() => CurrentView = new ProductView(_productViewModel));
            ShowInboundCommand = new RelayCommand(() => CurrentView = new InvoiceView(_inboundInvoiceViewModel));
            ShowOutboundCommand = new RelayCommand(() => CurrentView = new InvoiceView(_outboundInvoiceViewModel));
            ShowHistoryCommand = new RelayCommand(() => CurrentView = new InvoiceHistoryView(_invoceHistoryViewModel));
            ShowTopologyCommand = new RelayCommand(() => CurrentView = new WarehouseTopologyView(_topologyView));

        }
    }
}
