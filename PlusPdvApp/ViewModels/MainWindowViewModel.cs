using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlusPdvApp.Models;
using PlusPdvApp.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Linq;
using System;
using System.Collections.Generic;

namespace PlusPdvApp.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly ApiService _apiService;

        [ObservableProperty]
        private string _storeId = "1";

        [ObservableProperty]
        private string _username = "1";

        [ObservableProperty]
        private string _password = "admg2";

        [ObservableProperty]
        private string _loginStatusMessage;

        [ObservableProperty]
        private Visibility _productSearchVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private string _productName;

        public List<string> SortOrderOptions { get; } = new List<string>
        {
            "name", "id","manufacturer",
            "section", "subsection"
        };

        [ObservableProperty]
        private string _selectedSortOrder = "name";

        [ObservableProperty]
        private string _productStatusMessage;

        [ObservableProperty]
        private ObservableCollection<Product> _products;

        [ObservableProperty]
        private bool _isLoginEnabled = true;

        [ObservableProperty]
        private bool _isSearching;

        public MainWindowViewModel(ApiService apiService)
        {
            _apiService = apiService;
            _products = new ObservableCollection<Product>();
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            if (string.IsNullOrEmpty(StoreId) || string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                LoginStatusMessage = "Por favor, preencha todos os campos (Loja, Usuário e Senha).";
                ProductSearchVisibility = Visibility.Collapsed;
                return;
            }

            LoginStatusMessage = "Tentando fazer login...";
            IsLoginEnabled = false;

            var loginRequest = new LoginRequest
            {
                App_id = "PlusPdvWeb",
                Store_id = StoreId,
                Login = Username,
                Password = Password
            };

            try
            {
                var loginResponse = await _apiService.LoginAsync(loginRequest);
                LoginStatusMessage = "Login bem-sucedido! Token recuperado.";
                ProductSearchVisibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                LoginStatusMessage = $"Falha no login: {ex.Message}";
                ProductSearchVisibility = Visibility.Collapsed;
                IsLoginEnabled = true;
            }
        }

        [RelayCommand]
        private async Task SearchProductsAsync()
        {
            ProductStatusMessage = "Buscando produtos...";
            IsSearching = true;

            try
            {
                var productList = await _apiService.SearchProductsAsync(ProductName, SelectedSortOrder);

                if (productList != null && productList.Any())
                {
                    Products = new ObservableCollection<Product>(productList);
                    ProductStatusMessage = $"Encontrados {productList.Count} produtos.";
                }
                else
                {
                    Products.Clear();
                    ProductStatusMessage = "Nenhum produto encontrado com seus critérios.";
                }
            }
            catch (InvalidOperationException ex)
            {
                ProductStatusMessage = ex.Message;
                _apiService.ClearToken();
                ProductSearchVisibility = Visibility.Collapsed;
                LoginStatusMessage = "Sua sessão expirou. Faça login novamente.";
                IsLoginEnabled = true;
            }
            catch (Exception ex)
            {
                ProductStatusMessage = $"Falha na busca de produtos: {ex.Message}";
            }
            finally
            {
                IsSearching = false; 
            }
        }

        [RelayCommand]
        private void Logout()
        {
            _apiService.ClearToken();
            Products.Clear();
            ProductName = string.Empty;
            ProductStatusMessage = string.Empty;
            LoginStatusMessage = "Você saiu.";

            ProductSearchVisibility = Visibility.Collapsed;

            IsLoginEnabled = true;
        }
    }
}