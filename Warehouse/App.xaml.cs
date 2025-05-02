using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using Warehouse.Data.Repositories;
using Warehouse.Data;
using Warehouse.Services;
using Warehouse.ViewModels;
using Warehouse.Views;
using Microsoft.EntityFrameworkCore;

namespace Warehouse
{
    public partial class App : Application
    {
        public static ServiceProvider ServiceProvider { get; private set; }

        private readonly ServiceProvider _serviceProvider;

        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<WarehouseContext>();

            // Регистрация репозиториев
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICellRepository, CellRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderProductRepository, OrderProductRepository>();

            // Регистрация сервисов
            services.AddSingleton<StorageZoneService>();
            services.AddScoped<AbcXyzService>();
            services.AddScoped<ILineDirectionService, LineDirectionService>();
            services.AddScoped<PlacementService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICellService, CellService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IOrderProductService, OrderProductService>();

            // Регистрация ViewModel
            services.AddSingleton<ProductViewModel>();
            //services.AddSingleton<OrderViewModel>();
            services.AddSingleton<OutboundInvoiceViewModel>();
            
            services.AddSingleton<InboundInvoiceViewModel>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<InvoiceHistoryViewModel>();
            services.AddSingleton<TopologyViewModel>();


            // Регистрация окон / UserControl-ов
            services.AddSingleton<MainWindow>();
            services.AddSingleton<ProductView>();
            services.AddSingleton<WarehouseTopologyView>();
            services.AddTransient<InvoiceHistoryView>();
            // приходная накладная
            //services.AddTransient<InvoiceView>(sp =>
            //    new InvoiceView(sp.GetRequiredService<InboundInvoiceViewModel>()));

            //// расходная накладная
            //services.AddTransient<InvoiceView>(sp =>
            //    new InvoiceView(sp.GetRequiredService<OutboundInvoiceViewModel>()));

            


        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = _serviceProvider.GetService<MainWindow>();
            mainWindow?.Show();
        }
    }
}
