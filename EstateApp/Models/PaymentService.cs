using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EstateApp.Models;

namespace EstateApp.Services
{
    public class PaymentService
    {
        private readonly IHttpClientFactory _clientFactory;

        public PaymentService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<PaystackResponse> InitiatePayment(PaymentDTO paymentDTO)
        {
            var client = _clientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.paystack.co/transaction/initialize");

            // Add headers
            request.Headers.Add("Authorization", "Bearer sk_test_c202853900c12a46d6389dc3fe5c2caaff9f4a96"); // Replace YOUR_SECRET_KEY with your actual Paystack secret key

            // Create request body (amount conversion for API call only)
            var paymentData = new
            {
                email = paymentDTO.email,
                amount = (int)(paymentDTO.amountPaid * 100), // This conversion is required for Paystack
                callback_url = "http://127.0.0.1:5500/make_payments.html"
            };

            var json = JsonSerializer.Serialize(paymentData);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            // Send request
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var paystackResponse = JsonSerializer.Deserialize<PaystackResponse>(responseContent);
                return paystackResponse;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Paystack API returned an error: {errorContent}");
            }
        }
    }

    public class PaystackResponse
    {
        public bool status { get; set; }
        public string message { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public string authorization_url { get; set; }
        public string access_code { get; set; }
        public string reference { get; set; }
    }
}
