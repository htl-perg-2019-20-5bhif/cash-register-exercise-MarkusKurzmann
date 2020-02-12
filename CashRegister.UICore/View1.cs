using CashRegister.WebApi.Controllers;
using CashRegister.WebApi.Controllers.DTO;
using Polly;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CashRegister.UICore
{
    public class MainWindowViewModel : BindableBase
    {
        private ObservableCollection<Product> products;
        public ObservableCollection<Product> Products
        {
            get { return products; }
            set { SetProperty(ref products, value); }
        }

        private ObservableCollection<ReceiptLineViewModel> basket = new ObservableCollection<ReceiptLineViewModel>();
        public ObservableCollection<ReceiptLineViewModel> Basket
        {
            get { return basket; }
            set { SetProperty(ref basket, value); }
        }

        public decimal TotalSum => Basket.Sum(rl => rl.TotalPrice);

        public DelegateCommand<int?> AddToBasketCommand { get; }

        public DelegateCommand CheckoutCommand { get; }

        private HttpClient HttpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:44341"),
            Timeout = TimeSpan.FromSeconds(5)
        };

        public MainWindowViewModel()
        {
            AddToBasketCommand = new DelegateCommand<int?>(OnAddToBasket);
            CheckoutCommand = new DelegateCommand(async () => await OnCheckout(), () => Basket.Count > 0);
            Basket.CollectionChanged += (_, __) =>
            {
                CheckoutCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged(nameof(TotalSum));
            };

        }

        private readonly AsyncPolicy RetryPolicy = Policy.Handle<HttpRequestException>().RetryAsync(5);

        public async Task InitAsync()
        {
            var productsString = await RetryPolicy.ExecuteAndCaptureAsync(
                async () => await HttpClient.GetStringAsync("api/products"));

            var prods = await HttpClient.GetStringAsync("/api/products");

            Products = JsonSerializer.Deserialize<ObservableCollection<Product>>(prods);
        }

        private void OnAddToBasket(int? productID)
        {
            var product = Products.First(p => p.ID == productID);
            var basketItem = Basket.FirstOrDefault(p => p.ProductID == productID);
            if (basketItem != null)
            {
                basketItem.Amount++;
                basketItem.TotalPrice += product.UnitPrice;
                RaisePropertyChanged(nameof(TotalSum));
            }
            else
            {
                Basket.Add(new ReceiptLineViewModel
                {
                    ProductID = product.ID,
                    Amount = 1,
                    ProductName = product.ProductName,
                    TotalPrice = product.UnitPrice
                });
            }
        }

        private async Task OnCheckout()
        {
            var dto = Basket.Select(b => new ReceiptLineDto
            {
                ProductID = b.ProductID,
                Amount = b.Amount
            }).ToList();

            using (var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json"))
            {
                var response = await RetryPolicy.ExecuteAndCaptureAsync(async () => await HttpClient.PostAsync("/api/receipts2", content));
                response.Result.EnsureSuccessStatusCode();
            }

            Basket.Clear();
        }

    }

    public class ReceiptLineViewModel : BindableBase
    {
        private int productID;
        public int ProductID
        {
            get { return productID; }
            set { SetProperty(ref productID, value); }
        }

        private string productName;
        public string ProductName
        {
            get { return productName; }
            set { SetProperty(ref productName, value); }
        }

        private int amount;
        public int Amount
        {
            get { return amount; }
            set { SetProperty(ref amount, value); }
        }

        private decimal totalPrice;
        public decimal TotalPrice
        {
            get { return totalPrice; }
            set { SetProperty(ref totalPrice, value); }
        }
    }
}