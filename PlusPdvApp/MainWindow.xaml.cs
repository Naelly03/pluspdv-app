using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using PlusPdvApp.Models;

namespace PlusPdvApp
{
    public partial class MainWindow : Window
    {
        private static readonly HttpClient _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://165.227.177.3:50390/")
        };

        private string _jwtToken;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            string storeId = StoreTextBox.Text.Trim();
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordTextBox.Password.Trim(); 

            if (string.IsNullOrEmpty(storeId) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                LoginStatusTextBlock.Text = "Por favor, preencha todos os campos (Loja, Usuário e Senha).";
                ProductSearchGrid.Visibility = Visibility.Collapsed; 
                return; 
            }

            LoginStatusTextBlock.Text = "Tentando fazer login...";

            var loginRequest = new LoginRequest
            {
                App_id = "PlusPdvWeb", 
                Store_id = storeId,   
                Login = username,     
                Password = password   
            };

            try
            {
                string jsonContent = JsonConvert.SerializeObject(loginRequest);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync("api/Login", content);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    LoginResponse loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseBody);

                    _jwtToken = loginResponse.Token;
                    LoginStatusTextBlock.Text = "Login bem-sucedido! Token recuperado.";

                    ProductSearchGrid.Visibility = Visibility.Visible;
                    (sender as Button).IsEnabled = false;

                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", _jwtToken);
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    LoginStatusTextBlock.Text = $"Login falhou. Código de Status: {response.StatusCode}. Detalhes: {errorContent}";

                    ProductSearchGrid.Visibility = Visibility.Collapsed;
                    (sender as Button).IsEnabled = true;
                }
            }
            catch (HttpRequestException httpEx)
            {
                LoginStatusTextBlock.Text = $"Erro de rede durante o login: {httpEx.Message}. Verifique sua conexão ou a URL da API.";
            }
            catch (JsonException jsonEx)
            {
                LoginStatusTextBlock.Text = $"Erro ao interpretar resposta do login: {jsonEx.Message}. Verifique se a classe LoginResponse está correta para o JSON recebido.";
            }
            catch (Exception ex)
            {
                LoginStatusTextBlock.Text = $"Ocorreu um erro inesperado: {ex.Message}";
            }
        }

        private async void SearchProducts_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_jwtToken))
            {
                ProductStatusTextBlock.Text = "Por favor, faça login primeiro para buscar produtos.";
                return;
            }

            ProductStatusTextBlock.Text = "Buscando produtos...";

            string productName = ProductNameTextBox.Text.Trim();
            string selectedOrder = (SortOrderComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            var queryParameters = new List<string>();

            if (!string.IsNullOrEmpty(productName))
            {
                queryParameters.Add($"name={Uri.EscapeDataString(productName)}");
            }

            if (!string.IsNullOrEmpty(selectedOrder))
            {
                queryParameters.Add($"order={Uri.EscapeDataString(selectedOrder)}");
            }

            queryParameters.Add("limit=50");
            queryParameters.Add("offset=0");

            string queryString = string.Join("&", queryParameters);
            string requestUri = $"api/Produto?{queryString}";

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(requestUri);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    List<Product> products = JsonConvert.DeserializeObject<List<Product>>(responseBody);

                    if (products != null && products.Any())
                    {
                        ProductDataGrid.ItemsSource = products;
                        ProductStatusTextBlock.Text = $"Encontrados {products.Count} produtos.";
                    }
                    else
                    {
                        ProductDataGrid.ItemsSource = null;
                        ProductStatusTextBlock.Text = "Nenhum produto encontrado com seus critérios.";
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    ProductStatusTextBlock.Text = "Não autorizado. Sua sessão pode ter expirado. Por favor, faça login novamente.";
                    _jwtToken = null;
                    ProductSearchGrid.Visibility = Visibility.Collapsed;
                    (LoginStatusTextBlock.Parent as StackPanel)?.Children.OfType<Button>().FirstOrDefault()?.Dispatcher.Invoke(() =>
                    {
                        (LoginStatusTextBlock.Parent as StackPanel)?.Children.OfType<Button>().FirstOrDefault()?.IsEnabled = true;
                    });
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    ProductStatusTextBlock.Text = $"Falha na busca de produtos. Código de Status: {response.StatusCode}. Detalhes: {errorContent}";
                }
            }
            catch (HttpRequestException httpEx)
            {
                ProductStatusTextBlock.Text = $"Erro de rede durante a busca de produtos: {httpEx.Message}";
            }
            catch (JsonException jsonEx)
            {
                ProductStatusTextBlock.Text = $"Erro ao interpretar resposta da busca de produtos: {jsonEx.Message}. Verifique o formato da resposta da API.";
            }
            catch (Exception ex)
            {
                ProductStatusTextBlock.Text = $"Ocorreu um erro inesperado durante a busca: {ex.Message}";
            }
        }
    }
}
