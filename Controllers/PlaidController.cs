//using Common;
//using FairShareSolutions_PYMT.Services.IServices;
//using Microsoft.AspNetCore.Mvc;
//using System.Net.Http.Json;

//namespace FairShareSolutions_PYMT.API.Controllers
//{
//    public class PlaidController : Controller
//    {
//        private readonly IWebHostEnvironment webHostEnvironment;
//        private readonly IPaymentWalletService _paymentWalletService;
//        private readonly IConfiguration _iconfiguration;

//        public PlaidController(IWebHostEnvironment webHostEnvironment, IPaymentWalletService paymentWalletService, IConfiguration iconfiguration)
//        {
//            this.webHostEnvironment = webHostEnvironment;
//            _paymentWalletService = paymentWalletService;
//            _iconfiguration = iconfiguration;
//        }
//        [HttpGet]
//        public async Task<IActionResult> GetLinkToken()
//        {
//            var client = new HttpClient();
//            var requestBody = new
//            {
//                client_id = _iconfiguration.GetSection(ConstantBal.PlaidClientId).Value,
//                secret = _iconfiguration.GetSection(ConstantBal.PlaidSecretS).Value,
//                user = new { client_user_id = Guid.NewGuid() },
//                client_name = ConstantBal.AppName,
//                products = new[] { ConstantBal.Auth, ConstantBal.AppTransactions },
//                country_codes = new[] { ConstantBal.AllowedMerchantCountryUS },
//                language = ConstantBal.AllowedLanguage
//            };
//            var response = await client.PostAsJsonAsync(_iconfiguration.GetSection(ConstantBal.BaseURLS).Value, requestBody);
//            var content = await response.Content.ReadFromJsonAsync<LinkTokenResponse>();
//            return Ok(new { link_token = content!.Link_token });
//        }
//        [HttpGet]
//        public void CallBack(string public_token)
//        {

//        }
//    }
//    public class LinkTokenResponse
//    {
//        public string? Expiration { get; set; }
//        public string? Link_token { get; set; }
//        public string? Request_id { get; set; }
//    }


//}
