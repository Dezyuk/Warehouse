using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using Warehouse.Data.Repositories;
using Warehouse.Data;
using Warehouse.Services;
using Warehouse.ViewModels;
using Warehouse.Views;

namespace Warehouse
{
    public partial class App : Application
    {
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
            services.AddScoped<IWarehouseZoneRepository, WarehouseZoneRepository>();
            services.AddScoped<ICellRepository, CellRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();

            // Регистрация сервисов
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IWarehouseZoneService, WarehouseZoneService>();
            services.AddScoped<ICellService, CellService>();
            services.AddScoped<IOrderService, OrderService>();

            // Регистрация ViewModel
            services.AddTransient<WarehouseViewModel>();
            services.AddTransient<ProductViewModel>();
            services.AddTransient<OrderViewModel>();

            // Регистрация окон
            services.AddTransient<MainWindow>();
            services.AddTransient<OrdersWindow>();
            services.AddTransient<ProductWindow>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = _serviceProvider.GetService<MainWindow>();
            mainWindow?.Show();
        }
    }
}
