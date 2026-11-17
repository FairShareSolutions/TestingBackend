using Common;
using FairShareSolutions_PYMT.Infrastructure.ResponseModel;
using FairShareSolutions_PYMT.Infrastructure.ViewModel;
using FairShareSolutions_PYMT.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FairShareSolutions_PYMT.API.Controllers
{
	[Authorize]
	[Route("api/")]
	[ApiController]
	public class GiftCardController(IGiftCardService giftCardService) : ControllerBase
	{
		private readonly IGiftCardService _giftCardService = giftCardService;

		[HttpPost("order/gift-card")]
		public async Task<IActionResult> OrderGiftCard(GiftCardOrderViewModel request)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(new ApiResponse
				{
					Success = false,
					Message = FairShareResources.InvalidInput,
					Errors = ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)).ToList()
				});
			}

			var result = await _giftCardService.OrderGiftCard(request);

			if (!result.Success)
			{
				return BadRequest(result);
			}

			return Ok(result);
		}

		[HttpPost("gift-card/balance")]
		public async Task<IActionResult> CheckGiftCardBalance(string cardId)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(new ApiResponse
				{
					Success = false,
					Message = FairShareResources.InvalidInput,
					Errors = ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)).ToList()
				});
			}

			var result = await _giftCardService.CheckCardBalance(cardId);

			if (!result.Success)
			{
				return BadRequest(result);
			}

			return Ok(result);
		}

		[AllowAnonymous]
		[HttpPost("akimbo/login")]
		public async Task<IActionResult> Login(GiftCardLoginVM model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(new ApiResponse
				{
					Success = false,
					Message = FairShareResources.InvalidInput,
					Errors = ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)).ToList()
				});
			}

			var result = await _giftCardService.Login(model);

			if (!result.Success)
			{
				return BadRequest(result);
			}

			return Ok(result);
		}

	
		//[ApiExplorerSettings(IgnoreApi = true)]
		//[HttpPost("download-pass-certificate")]
		//public async Task<IActionResult> DownloadPassCertificate([FromBody] WalletPassRequestVM request)
		//{
		//	if (string.IsNullOrWhiteSpace(request.CardId))
		//	{
		//		return BadRequest(new ApiResponse
		//		{
		//			Success = false,
		//			Message = FairShareResources.CardIdOrUsernameRequired
		//		});
		//	}

		//	var result = await _giftCardService.DownloadPassCertificate(request);

		//	if (!result.Success)
		//	{
		//		return BadRequest(result);
		//	}

		//	// iOS
		//	if (request.Platform.Equals("ios", StringComparison.OrdinalIgnoreCase))
		//	{
		//		return File(
		//			(byte[])result.Data,
		//			"application/vnd.apple.pkpass",
		//			$"giftcard-{request.CardId}.pkpass");
		//	}

		//	// Android
		//	return Ok(result);
		//}

		[HttpPost("card/provision/apple-wallet")]
		public async Task<IActionResult> AttachGiftCardInAppleWallet(WalletProvisionRequestVM request)
		{
			if (string.IsNullOrWhiteSpace(request.Username))
			{
				return BadRequest(new ApiResponse
				{
					Success = false,
					Message = FairShareResources.MissingUserName
				});
			}

			var result = await _giftCardService.ProvisionWalletAsync(request);

			if (!result.Success)
			{
				return BadRequest(result);
			}

			return Ok(result);
		}

		[HttpPost("wallet/attach/gift-card")]
		public async Task<IActionResult> AttachAndOrderGiftCard(AttachOrderGiftCardViewModel request)
		{
			if (string.IsNullOrWhiteSpace(request.UserName))
			{
				return BadRequest(new ApiResponse
				{
					Success = false,
					Message = FairShareResources.MissingUserName
				});
			}

			var result = await _giftCardService.AttachOrderGiftCard(request);

			if (!result.Success)
			{
				return BadRequest(result);
			}

			return Ok(result);
		}



		[HttpPost("wallet/passdata")]
		public async Task<IActionResult> WalletPassdata(AkimboPassRequest model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(new ApiResponse
				{
					Success = false,
					Message = FairShareResources.InvalidInput,
					Errors = ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)).ToList()
				});
			}

			var result = await _giftCardService.GetPassDataAsync(model);

			if (!result.Success)
			{
				return BadRequest(result);
			}

			return Ok(result);
		}

		[HttpGet("EncryptPassword")] 

		public async Task<IActionResult> EncryptPassword (string password, bool encrypt)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(new ApiResponse
				{
					Success = false,
					Message = FairShareResources.InvalidInput,
					Errors = ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)).ToList()
				});
			}

			var result = await _giftCardService.EncryptPassword(password,encrypt);

			if (result==null)
			{
				return BadRequest(result);
			}

			return Ok(result);
		}
	}
}
