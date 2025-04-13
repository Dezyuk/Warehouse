using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Warehouse.Models;
using Warehouse.Services;

namespace Warehouse.Views
{
    /// <summary>
    /// Окно выбора товара. Загружает список товаров через IProductService и фильтрует те,
    /// которые уже выбраны (на основе списка идентификаторов), чтобы не допустить повторного выбора.
    /// </summary>
    public partial class ProductSelectionWindow : Window
    {
        // Коллекция товаров для отображения
        public ObservableCollection<Product> Products { get; set; } = new ObservableCollection<Product>();

        private readonly IProductService _productService;
        // Список уже выбранных товаров (по Id), чтобы их исключить из отображения
        private readonly System.Collections.Generic.List<int> _excludedProductIds;

        public Product? SelectedProduct { get; private set; }

        /// <summary>
        /// Конструктор окна.
        /// </summary>
        /// <param name="productService">Сервис для работы с товарами.</param>
        /// <param name="excludedProductIds">Список идентификаторов товаров, которые уже выбраны.</param>
        public ProductSelectionWindow(IProductService productService, System.Collections.Generic.List<int> excludedProductIds = null)
        {
            InitializeComponent();
            _productService = productService;
            _excludedProductIds = excludedProductIds ?? new System.Collections.Generic.List<int>();
            LoadProducts();
            DataContext = this;
        }

        /// <summary>
        /// Загрузить список товаров, исключая те, что уже выбраны.
        /// </summary>
        private void LoadProducts()
        {
            Products.Clear();
            var allProducts = _productService.GetAllProducts();
            // Фильтруем товары, если их Id содержится в _excludedProductIds
            var filtered = allProducts.Where(p => !_excludedProductIds.Contains(p.Id));
            foreach (var product in filtered)
            {
                Products.Add(product);
            }
        }

        // При двойном клике выбираем товар
        private void ProductsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ProductsList.SelectedItem is Product product)
            {
                SelectedProduct = product;
                DialogResult = true;
                Close();
            }
        }

        // При нажатии OK также выбираем товар, если выбран один из списка
        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsList.SelectedItem is Product product)
            {
                SelectedProduct = product;
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Выберите товар.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
