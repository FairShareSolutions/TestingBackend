//using Common;
//using FairShareSolutions_PYMT.API.Resource;
//using FairShareSolutions_PYMT.Infrastructure.PaymentProcessorViewModel;
//using FairShareSolutions_PYMT.Infrastructure.ResponseModel;
//using FairShareSolutions_PYMT.Infrastructure.ViewModel;
//using FairShareSolutions_PYMT.Services.IServices;
//using Microsoft.AspNetCore.Mvc;
//using Serilog;
//using System.Net;

//namespace FairShareSolutions_PYMT.API.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    //[Authorize]
//    public class PaymentWalletController : ControllerBase
//    {

//        private readonly IPaymentWalletService _paymentWalletService;
//        private readonly IHttpContextAccessor _httpContextAccessor;

//        public PaymentWalletController(IPaymentWalletService paymentWalletService, IHttpContextAccessor httpContextAccessor)
//        {
//            _paymentWalletService = paymentWalletService;
//            _httpContextAccessor = httpContextAccessor;
//        }
//        /// <summary>
//        /// This is method used to the apple wallet setup for payment
//        /// </summary>
//        /// <param name="walletModel"></param>
//        /// <returns></returns>
//        /// <exception cref="Exception"></exception>
//        [HttpPost]
//        [Route(ConstantBal.AppleWalletSetup)]
//        public async Task<IActionResult> AppleWalletSetup(WalletModel walletModel)
//        {
//            try
//            {
//                ApplicationResponse applicationResponse = new ApplicationResponse();
//                if (ModelState.IsValid)
//                {
//                    var applicationResponses = await _paymentWalletService.AppleWalletSetup(walletModel);

//                    if (applicationResponse != null)
//                    {
//                        var pass = File(applicationResponses, ConstantBal.PassType, ConstantBal.PkPassFileDownlodName);
//                        return File(applicationResponses, ConstantBal.PassType, ConstantBal.PkPassFileDownlodName);
//                    }
//                    else
//                    {
//                        return BadRequest(applicationResponse);
//                    }
//                }
//                else
//                {
//                    return BadRequest(ModelState);
//                }
//            }
//            catch (Exception ex)
//            {

//                throw new Exception(ex.Message);
//            }
//        }

//        [HttpPost("pkpass")]
//		public async Task<string> SavePkPassFile(WalletModel walletModel)
//		{
//			var pkpassBytes = await _paymentWalletService.AppleWalletSetup(walletModel);

//			var rootFolder = Directory.GetCurrentDirectory();
//			var tempPassFolder = Path.Combine(rootFolder, "wwwroot", "TempPass");
//			Directory.CreateDirectory(tempPassFolder);

//			var fileName = $"giftcard-{walletModel.CardNumber}.pkpass";
//			var filePath = Path.Combine(tempPassFolder, fileName);

//			await System.IO.File.WriteAllBytesAsync(filePath, pkpassBytes);
//			return $"/TempPass/{fileName}";
//		}


//		/// <summary>
//		/// This is method used to the create payees account for the payment processor
//		/// </summary>
//		/// <param name="createPayeesModel"></param>
//		/// <returns></returns>
//		/// <exception cref="Exception"></exception>
//		[ApiExplorerSettings(IgnoreApi = true)]
//        [HttpPost]
//        [Route(ConstantBal.CreatePayees)]
//        public async Task<IActionResult> CreatePayees(CreatePayeesViewModel createPayeesModel)
//        {
//            try
//            {
//                ApplicationResponse applicationResponse = new ApplicationResponse();
//                if (ModelState.IsValid)
//                {
//                    var applicationResponses = await _paymentWalletService.CreatePayees(createPayeesModel);

//                    if (applicationResponse != null)
//                    {
//                        return Ok(applicationResponses);
//                    }
//                    else
//                    {
//                        return BadRequest(applicationResponse);
//                    }
//                }
//                else
//                {
//                    return BadRequest(ModelState);
//                }
//            }
//            catch (Exception ex)
//            {

//                throw new Exception(ex.Message);
//            }
//        }

//        /// <summary>
//        /// This is method used to the create payees bank account for the payment processor
//        /// </summary>
//        /// <param name="createPayeeBankAccountViewModel"></param>
//        /// <returns></returns>
//        /// <exception cref="Exception"></exception>
//        [ApiExplorerSettings(IgnoreApi = true)]
//        [HttpPost]
//        [Route(ConstantBal.CreatePayeeBankAccount)]
//        public async Task<IActionResult> CreatePayeeBankAccount(CreatePayeeBankAccountViewModel createPayeeBankAccountViewModel)
//        {
//            try
//            {
//                ApplicationResponse applicationResponse = new ApplicationResponse();
//                if (ModelState.IsValid)
//                {
//                    var applicationResponses = await _paymentWalletService.CreatePayeeBankAccount(createPayeeBankAccountViewModel);

//                    if (applicationResponse != null)
//                    {
//                        return Ok(applicationResponses);
//                    }
//                    else
//                    {
//                        return BadRequest(applicationResponse);
//                    }
//                }
//                else
//                {
//                    return BadRequest(ModelState);
//                }
//            }
//            catch (Exception ex)
//            {

//                throw new Exception(ex.Message);
//            }
//        }

//        /// <summary>
//        /// This method is used for call the send the ach payment API for the each user 
//        /// </summary>
//        /// <param name="sendACHPaymentViewModel"></param>
//        /// <returns></returns>
//        /// <exception cref="Exception"></exception>
//        [HttpPost]
//        [Route(ConstantBal.SendACHPayment)]
//        public async Task<IActionResult> SendACHPayment(SendACHPaymentViewModel sendACHPaymentViewModel)
//        {
//            try
//            {
//                ApplicationResponse applicationResponse = new ApplicationResponse();
//                if (ModelState.IsValid)
//                {
//                    var applicationResponses = await _paymentWalletService.SendACHPayment(sendACHPaymentViewModel);

//                    if (applicationResponse != null)
//                    {
//                        return Ok(applicationResponses);
//                    }
//                    else
//                    {
//                        return BadRequest(applicationResponse);
//                    }
//                }
//                else
//                {
//                    return BadRequest(ModelState);
//                }
//            }
//            catch (Exception ex)
//            {

//                throw new Exception(ex.Message);
//            }
//        }

//        /// <summary>
//        /// This method is used for call the create the virtual card payment API for the admin
//        /// </summary>
//        /// <param name="createVirtualCardViewModel"></param>
//        /// <returns></returns>
//        /// <exception cref="Exception"></exception>
//        [ApiExplorerSettings(IgnoreApi = true)]
//        [HttpPost]
//        [Route(ConstantBal.CreateVirtualCardPayment)]
//        public async Task<IActionResult> CreateVirtualCardPayment(CreateVirtualCardViewModel createVirtualCardViewModel)
//        {
//            try
//            {
//                ApplicationResponse applicationResponse = new ApplicationResponse();
//                if (ModelState.IsValid)
//                {
//                    var applicationResponses = await _paymentWalletService.CreateVirtualCardPayment(createVirtualCardViewModel);

//                    if (applicationResponse != null)
//                    {
//                        return Ok(applicationResponses);
//                    }
//                    else
//                    {
//                        return BadRequest(applicationResponse);
//                    }
//                }
//                else
//                {
//                    return BadRequest(ModelState);
//                }
//            }
//            catch (Exception ex)
//            {

//                throw new Exception(ex.Message);
//            }
//        }

//        /// <summary>
//        /// This method is used for call the get the virtual card payment API based on the admin
//        /// </summary>
//        /// <param name="AdminId"></param>
//        /// <returns></returns>
//        /// <exception cref="Exception"></exception>
//        [HttpPost]
//        [Route(ConstantBal.GetVirtualCardDetails)]
//        public async Task<IActionResult> GetVirtualCardDetails(string AdminId)
//        {
//            try
//            {
//                ApplicationResponse applicationResponse = new ApplicationResponse();
//                if (ModelState.IsValid)
//                {
//                    var applicationResponses = await _paymentWalletService.GetVirtualCardDetails(AdminId);

//                    if (applicationResponse != null)
//                    {
//                        return Ok(applicationResponses);
//                    }
//                    else
//                    {
//                        return BadRequest(applicationResponse);
//                    }
//                }
//                else
//                {
//                    return BadRequest(ModelState);
//                }
//            }
//            catch (Exception ex)
//            {

//                throw new Exception(ex.Message);
//            }
//        }

//        /// <summary>
//        /// This is method used to the google wallet setup for payment
//        /// </summary>
//        /// <param name="request"></param>
//        /// <returns></returns>
//        [ApiExplorerSettings(IgnoreApi = true)]
//        [HttpPost]
//        [Route(ConstantBal.GoogleWalletSetup)]
//        public async Task<IActionResult> GoogleWalletSetup([FromBody] GoogleWalletRequestModel request)
//        {
//            try
//            {
//                // Create Loyalty Object
//                var loyaltyObjectId = await _paymentWalletService.GoogleWalletSetup(request);

//                // Generate .pkpass file
//                var pkpassBytes = await _paymentWalletService.GeneratePkPassFile(loyaltyObjectId);

//                // Return .pkpass file for download
//                return File(pkpassBytes, ConstantBal.PassTypes, ConstantBal.GooglePassTypes);
//            }
//            catch (Exception ex)
//            {
//                return BadRequest($"{APIResource.ErrorCreatingLoyaltyPass} {ex.Message}");
//            }
//        }

//        /// <summary>
//        /// This is method used to the google wallet setup for payment
//        /// </summary>
//        /// <param name="googleWalletModel"></param>
//        /// <returns></returns>
//        [HttpPost]
//        [Route(ConstantBal.GoogleWalletSetups)]
//        public IActionResult GoogleWalletSetups(GoogleWalletModel googleWalletModel)
//        {
//            ApplicationResponse applicationResponse = new ApplicationResponse();
//            try
//            {
//                var loyaltyObjectId = _paymentWalletService.GoogleWalletSetups(googleWalletModel);

//                if (loyaltyObjectId != null)
//                {
//                    applicationResponse.StatusCode = (int)HttpStatusCode.OK;
//                    applicationResponse.Message = APIResource.RetriveGoogleWallet;
//                    applicationResponse.Status = true;
//                    applicationResponse.Data = loyaltyObjectId;

//                }
//                return Ok(applicationResponse);
//            }
//            catch (Exception ex)
//            {
//                return BadRequest($"{APIResource.ErrorCreatingLoyaltyPass} {ex.Message}");
//            }
//        }

//        /// <summary>
//        ///  This is method used to the get the transaction list for the user
//        /// </summary>
//        /// <param name="UserId"></param>
//        /// <returns></returns>
//        /// <exception cref="Exception"></exception>
//        [HttpPost]
//        [Route(ConstantBal.TransactionList)]
//        public async Task<IActionResult> TransactionList(string UserId)
//        {
//            try
//            {
//                ApplicationResponse applicationResponse = new ApplicationResponse();
//                if (ModelState.IsValid)
//                {
//                    applicationResponse = await _paymentWalletService.TransactionList(UserId);

//                    if (applicationResponse != null)
//                    {
//                        return Ok(applicationResponse);
//                    }
//                    else
//                    {
//                        return BadRequest(applicationResponse);
//                    }
//                }
//                else
//                {
//                    return BadRequest(ModelState);
//                }
//            }
//            catch (Exception ex)
//            {

//                throw new Exception(ex.Message);
//            }
//        }

//        /// <summary>
//        /// This is method used to the get the transaction list for the user
//        /// </summary>
//        /// <param name="UsersId"></param>
//        /// <returns></returns>
//        /// <exception cref="Exception"></exception>
//        [HttpPost]
//        [Route(ConstantBal.GetPayeeAccountDetails)]
//        public async Task<IActionResult> GetPayeeAccountDetails(string UsersId)
//        {
//            try
//            {
//                ApplicationResponse applicationResponse = new ApplicationResponse();
//                if (ModelState.IsValid)
//                {
//                    applicationResponse = await _paymentWalletService.GetPayeeAccountDetails(UsersId);

//                    if (applicationResponse != null)
//                    {
//                        return Ok(applicationResponse);
//                    }
//                    else
//                    {
//                        return BadRequest(applicationResponse);
//                    }
//                }
//                else
//                {
//                    return BadRequest(ModelState);
//                }
//            }
//            catch (Exception ex)
//            {

//                throw new Exception(ex.Message);
//            }
//        }

//        /// <summary>
//        /// This is method used to the get the spending transaction list for the user(Weekly, Monthly, Annually)
//        /// </summary>
//        /// <param name="UsersId"></param>
//        /// <returns></returns>
//        /// <exception cref="Exception"></exception>
//        [HttpPost]
//        [Route(ConstantBal.SpendingTransactionDetails)]
//        public async Task<IActionResult> SpendingTransactionDetails(string UsersId)
//        {
//            try
//            {
//                ApplicationResponse applicationResponse = new ApplicationResponse();
//                if (ModelState.IsValid)
//                {
//                    applicationResponse = await _paymentWalletService.SpendingTransactionDetails(UsersId);

//                    if (applicationResponse != null)
//                    {
//                        return Ok(applicationResponse);
//                    }
//                    else
//                    {
//                        return BadRequest(applicationResponse);
//                    }
//                }
//                else
//                {
//                    return BadRequest(ModelState);
//                }
//            }
//            catch (Exception ex)
//            {

//                throw new Exception(ex.Message);
//            }
//        }

//        /// <summary>
//        /// This method is used to create a public exchange token for getting transaction data
//        /// </summary>
//        /// <param name="request"></param>
//        /// <returns></returns>
//        /// <exception cref="Exception"></exception>

//        [HttpPost]
//        [Route(ConstantBal.ExchangePublicToken)]
//        public async Task<IActionResult> ExchangePublicToken(PublicTokenExchangeRequest request)
//        {
//            try
//            {
//                Log.Information(APIResource.ExchangePublicTokenApiStart + ConstantBal.Space + request);
//                ApplicationResponse applicationResponse = new ApplicationResponse();
//                if (ModelState.IsValid)
//                {
//                    applicationResponse = await _paymentWalletService.ExchangePublicToken(request);
//                    if (applicationResponse.StatusCode == (int)HttpStatusCode.OK)
//                    {
//                        return Ok(applicationResponse);
//                    }
//                    else
//                    {
//                        return BadRequest(applicationResponse);
//                    }
//                }
//                else
//                {
//                    return BadRequest(ModelState);
//                }
//            }
//            catch (Exception ex)
//            {
//                Log.Information(APIResource.AnyErrorForExchangePublicToken + ex.Message);
//                Log.Error(APIResource.ExceptionInExchangePublicTokenAPI + ConstantBal.Space + ex.Message);
//                throw new Exception(ex.Message);
//            }
//        }

//    }

//}
