using Common;
using FairShareSolutions_PYMT.Infrastructure.ResponseModel;
using FairShareSolutions_PYMT.Infrastructure.ViewModel;
using FairShareSolutions_PYMT.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace FairShareSolutions_PYMT.API.Controllers
{
	[Authorize]
	[Route("api/")]
	[ApiController]
	//[AllowAnonymous]
	public class UsioPaymentController : ControllerBase
	{
		private readonly IUsioService _usioService;
		public UsioPaymentController(IUsioService usioService)
		{
			_usioService = usioService;
		}

		[HttpPost("submitPermanentToken/makePayment")]
		public async Task<IActionResult> SubmitPayment([FromBody] RequestPaymentViewModel model)
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

			var result = await _usioService.ExecuteUsioPayment(model);

			if (!result.Success)
			{
				return BadRequest(result);
			}

			return Ok(result);
		}

		[HttpGet("payment-methods")]
		public async Task<IActionResult> GetPaymentMethods(string userId)
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

			var result = await _usioService.GetSavedPaymentMethodsAsync(userId);

			if (!result.Success)
			{
				return BadRequest(result);
			}

			return Ok(result);
		}

		[HttpPost("submitToken/makePayment")]
		public async Task<IActionResult> SubmitPayment([FromBody] SubmitTokenPaymentViewModel model)
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

			var result = await _usioService.SubmitTokenPayment(model);

			if (!result.Success)
			{
				return BadRequest(result);
			}

			return Ok(result);
		}

		[AllowAnonymous]
		[HttpPost("payment-methods/remove")]
		public async Task<IActionResult> RemovePaymentMethod(string paymentMethodId)
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
			var userIdClaim = User.FindFirst(ClaimTypes.Sid)?.Value;
			if (!Guid.TryParse(userIdClaim, out var userId))
			{
				return Unauthorized(new ApiResponse
				{
					Success = false,
					Message = "Invalid user identity."
				});
			}

			var result = await _usioService.RemovePaymentMethods(paymentMethodId, userId.ToString());

			if (!result.Success)
			{
				return BadRequest(result);
			}

			return Ok(result);
		}

		[HttpGet("transaction/list")]
		//public async Task<IActionResult> TransactionHistory(string userId, int pageNumber, string sorting, DateTime? fromDate = null, DateTime? toDate = null)
		public async Task<IActionResult> TransactionHistory(string userId, int pageNumber)
		{
			string sorting = "all";
			DateTime? fromDate = null,
			 toDate = fromDate;
			if (!ModelState.IsValid)
			{
				return BadRequest(new ApiResponse
				{
					Success = ConstantBal.False,
					Message = FairShareResources.InvalidInput,
					Errors = ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)).ToList()
				});
			}

			var result = await _usioService.GetTransactionHistoryAsync(userId, pageNumber, sorting, fromDate, toDate);

			if (!result.Success)
			{
				return BadRequest(result);
			}

			return Ok(result);
		}

		[HttpPost("payment-method/roulette")]
		public async Task<IActionResult> SaveRoulettePaymentMethod([FromBody] SaveRoulettePaymentMethodViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(new ApiResponse
				{
					Success = ConstantBal.False,
					Message = FairShareResources.InvalidInput,
					Errors = ModelState.Values
						.SelectMany(v => v.Errors.Select(b => b.ErrorMessage))
						.ToList()
				});
			}

			var result = await _usioService.SaveRoulettePaymentMethodAsync(model);

			if (!result.Success)
			{
				if (result.StatusCode == ((int)HttpStatusCode.NotFound).ToString())
				{
					return NotFound(result);
				}

				return BadRequest(result);
			}

			return Ok(result);
		}
	}
}
