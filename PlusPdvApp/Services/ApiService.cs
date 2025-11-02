using Newtonsoft.Json;
using PlusPdvApp.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace PlusPdvApp.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://165.227.177.3:50390/")
        };

        private string _jwtToken;

        public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
        {
            string jsonContent = JsonConvert.SerializeObject(loginRequest);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync("api/Login", content);

            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            LoginResponse loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseBody);

            _jwtToken = loginResponse.Token;
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _jwtToken);

            return loginResponse;
        }

        public async Task<List<Product>> SearchProductsAsync(string productName, string selectedOrder)
        {
            if (string.IsNullOrEmpty(_jwtToken))
            {
                throw new InvalidOperationException("Não autorizado. Faça login novamente.");
            }

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

            HttpResponseMessage response = await _httpClient.GetAsync(requestUri);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _jwtToken = null;
                throw new InvalidOperationException("Não autorizado. Sua sessão pode ter expirado.");
            }

            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Product>>(responseBody);
        }

        public void ClearToken()
        {
            _jwtToken = null;
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}