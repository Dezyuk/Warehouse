using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using Warehouse.Helper;
using Warehouse.Models;
using Warehouse.Services;
using Warehouse.Views; // Здесь находится реализация RelayCommand

namespace Warehouse.ViewModels
{
    /// <summary>
    /// ViewModel для управления списком товаров.
    /// Обеспечивает загрузку данных, выбор продукта и выполнение операций CRUD через команды.
    /// </summary>
    public class ProductViewModel : BaseViewModel
    {
        // Сервис для работы с данными продуктов (через репозиторий)
        private readonly IProductService _productService;

        // Коллекция товаров, которая автоматически обновляет UI при изменении
        public ObservableCollection<Product> Products { get; set; } = new ObservableCollection<Product>();

        // Выбранный товар (например, для редактирования или удаления)
        private Product? _selectedProduct;
        public Product? SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                _selectedProduct = value;
                OnPropertyChanged(); // Уведомляем UI об изменении
            }
        }

        // Команды для операций над товарами
        public ICommand AddCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }

        /// <summary>
        /// Конструктор ProductViewModel получает IProductService через DI.
        /// Инициализируются команды и загружается список товаров.
        /// </summary>
        public ProductViewModel(IProductService productService)
        {
            _productService = productService;

            // Инициализация команд с использованием RelayCommand.
            // Update и Delete команды активны, только если выбран товар.
            AddCommand = new RelayCommand(AddProduct);
            UpdateCommand = new RelayCommand(UpdateProduct, () => SelectedProduct != null);
            DeleteCommand = new RelayCommand(DeleteProduct, () => SelectedProduct != null);

            // Загружаем список товаров при инициализации ViewModel
            LoadProducts();
        }

        /// <summary>
        /// Загружает все товары из сервиса в ObservableCollection.
        /// </summary>
        private void LoadProducts()
        {
            Products.Clear();
            foreach (var product in _productService.GetAllProducts())
            {
                Products.Add(product);
            }
        }

        /// <summary>
        /// Добавляет новый товар.
        /// Открывается модальное окно (ProductEditWindow), где пользователь вводит данные.
        /// После подтверждения новый товар сохраняется через сервис, и список обновляется.
        /// </summary>
        private void AddProduct()
        {
            // Создаем пустой объект товара
            var newProduct = new Product();

            // Открываем окно редактирования товара
            var window = new ProductEditWindow(newProduct);
            if (window.ShowDialog() == true)
            {
                _productService.AddProduct(newProduct);
                LoadProducts();
            }
        }

        /// <summary>
        /// Редактирует выбранный товар.
        /// Создается копия выбранного товара для редактирования, чтобы не менять данные напрямую.
        /// Открывается модальное окно для редактирования, после чего изменения сохраняются.
        /// </summary>
        private void UpdateProduct()
        {
            if (SelectedProduct == null)
                return;

            // Создаем копию данных выбранного товара
            var editedProduct = new Product
            {
                Id = SelectedProduct.Id,
                Name = SelectedProduct.Name,
                Article = SelectedProduct.Article,
                Quantity = SelectedProduct.Quantity,
                Price = SelectedProduct.Price
            };

            var window = new ProductEditWindow(editedProduct);
            if (window.ShowDialog() == true)
            {
                _productService.UpdateProduct(editedProduct);
                LoadProducts();
            }
        }

        /// <summary>
        /// Удаляет выбранный товар.
        /// Вызывает удаление через сервис и обновляет список.
        /// </summary>
        private void DeleteProduct()
        {
            if (SelectedProduct == null)
                return;

            _productService.DeleteProduct(SelectedProduct.Id);
            LoadProducts();
        }
    }
}
