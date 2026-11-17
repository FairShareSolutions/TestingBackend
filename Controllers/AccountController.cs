using Common;
using FairShareSolutions_PYMT.API.Resource;
using FairShareSolutions_PYMT.Infrastructure.DataModel;
using FairShareSolutions_PYMT.Infrastructure.ResponseModel;
using FairShareSolutions_PYMT.Infrastructure.ViewModel;
using FairShareSolutions_PYMT.Infrastructure.ViewModel.FairShareSolutions_PYMT.Infrastructure.ViewModel;
using FairShareSolutions_PYMT.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Net;
using System.Security.Claims;

namespace FairShareSolutions_PYMT.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AccountController(IAccountService accountService) : ControllerBase
	{
		private readonly IAccountService _accountService = accountService;

		#region Send OTP  method
		/// <summary>
		/// This is method used to send the otp for setting password
		/// </summary>
		/// <param name="otpData"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		[AllowAnonymous]
		[HttpPost]
		[Route(ConstantBal.ResendEmailVerificationCode)]
		public async Task<IActionResult> SendOtp(OtpViewModel otpData)
		{
			ApplicationResponse responseM = new ApplicationResponse();
			try
			{
				responseM = await _accountService.SendOtp(otpData);
				if (responseM.StatusCode == (int)HttpStatusCode.OK)
				{
					return Ok(responseM);
				}
				else
				{
					return BadRequest(responseM);
				}

			}
			catch (Exception ex)
			{

				throw new Exception(ex.Message);
			}

		}
		#endregion

		#region Send Phone OTP  method
		/// <summary>
		/// This is method used to send the phone otp for account register
		/// </summary>
		/// <param name="sendPhoneVerificationCodeModel"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		[AllowAnonymous]
		[HttpPost]
		[Route(ConstantBal.SendPhoneVerificationCode)]
		public async Task<IActionResult> SendPhoneVerificationCode(SendPhoneVerificationCodeModel sendPhoneVerificationCodeModel)
		{
			ApplicationResponse responseM = new ApplicationResponse();
			try
			{
				responseM = await _accountService.SendPhoneVerificationCode(sendPhoneVerificationCodeModel);
				if (responseM.StatusCode == (int)HttpStatusCode.OK)
				{
					return Ok(responseM);
				}
				else
				{
					return BadRequest(responseM);
				}

			}
			catch (Exception ex)
			{

				throw new Exception(ex.Message);
			}

		}
		#endregion

		#region Resend Phone OTP  method
		/// <summary>
		/// This is method used to re-send the phone otp for user login
		/// </summary>
		/// <param name="sendPhoneVerificationCodeModel"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		[AllowAnonymous]
		[HttpPost]
		[Route(ConstantBal.ResendPhoneVerificationCode)]
		public async Task<IActionResult> ResendPhoneVerificationCode(SendPhoneVerificationCodeModel sendPhoneVerificationCodeModel)
		{
			ApplicationResponse responseM = new ApplicationResponse();
			try
			{
				responseM = await _accountService.ResendPhoneVerificationCode(sendPhoneVerificationCodeModel);
				if (responseM.StatusCode == (int)HttpStatusCode.OK)
				{
					return Ok(responseM);
				}
				else
				{
					return BadRequest(responseM);
				}

			}
			catch (Exception ex)
			{

				throw new Exception(ex.Message);
			}

		}
		#endregion

		#region Validate OTP method
		/// <summary>
		/// 
		/// </summary>
		/// <param name="validateOtpViewModel"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		/// 
		[AllowAnonymous]
		[HttpPost]
		[Route(ConstantBal.ValidateOtp)]
		public async Task<IActionResult> ValidateOtp(ValidateOtpViewModel validateOtpViewModel)
		{
			ApplicationResponse responseM = new ApplicationResponse();
			try
			{
				responseM = await _accountService.ValidateOtp(validateOtpViewModel);
				if (responseM.StatusCode == (int)HttpStatusCode.OK)
				{
					return Ok(responseM);
				}
				else
				{
					return BadRequest(responseM);
				}
			}
			catch (Exception ex)
			{

				throw new Exception(ex.Message);
			}
		}
		#endregion

		#region Sign Up Method
		/// <summary>
		/// This is method used to the sign up 
		/// </summary>
		/// <param name="applicationUserVM"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		[AllowAnonymous]
		[HttpPost]
		[Route(ConstantBal.SignUp)]
		public async Task<IActionResult> SignUp(SignUpModel applicationUserVM)
		{
			ApplicationResponse responseM = new ApplicationResponse();
			try
			{
				if (ModelState.IsValid)
				{
					responseM = await _accountService.Signup(applicationUserVM);
					if (responseM.StatusCode == (int)HttpStatusCode.OK)
					{
						return Ok(responseM);
					}
					else
					{
						return BadRequest(responseM);
					}
				}
				else
				{
					return BadRequest(ModelState);
				}
			}
			catch (Exception ex)
			{

				throw new Exception(ex.Message);
			}

		}
		#endregion

		#region Login method
		/// <summary>
		/// This is the method for user Login
		/// </summary>
		/// <param name="loginViewModel"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		[AllowAnonymous]
		[HttpPost]
		[Route(ConstantBal.LogIn)]
		public async Task<IActionResult> Login(LoginViewModel loginViewModel)
		{
			Log.Information(APIResource.LoghinStart);
			ApplicationResponse responseM = new ApplicationResponse();
			try
			{
				responseM = await _accountService.Login(loginViewModel);
				if (responseM?.StatusCode == (int)HttpStatusCode.OK)
				{
					Log.Information(APIResource.LoginSuccess);
					return Ok(responseM);
				}
				else
				{
					Log.Error(APIResource.LoginFailed);
					return BadRequest(responseM);
				}
			}
			catch (Exception ex)
			{
				Log.Error(APIResource.LoginFailed + ex.Message);
				throw new Exception(ex.Message);
			}
		}
		#endregion

		#region Login By Password method
		/// <summary>
		/// This is the method for user Login
		/// </summary>
		/// <param name="loginViewModel"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		/// 
		[AllowAnonymous]
		[HttpPost]
		[Route(ConstantBal.LogInByPassword)]
		public async Task<IActionResult> LoginByPassword(LoginViewModel loginViewModel)
		{
			Log.Information(APIResource.LoghinStart);
			ApplicationResponse responseM = new ApplicationResponse();
			try
			{
				responseM = await _accountService.Login(loginViewModel);
				if (responseM?.StatusCode == (int)HttpStatusCode.OK)
				{
					Log.Information(APIResource.LoginSuccess);
					return Ok(responseM);
				}
				else
				{
					Log.Error(APIResource.LoginFailed);
					return BadRequest(responseM);
				}
			}
			catch (Exception ex)
			{
				Log.Error(APIResource.LoginFailed + ex.Message);
				throw new Exception(ex.Message);
			}
		}
		#endregion

		#region Forgot Password method
		/// <summary>
		/// 
		/// </summary>
		/// <param name="forgotPasswordVM"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		[AllowAnonymous]
		[HttpPost]
		[Route(ConstantBal.ForgotPassword)]
		public async Task<IActionResult> ForgotPassword(ForgetPasswordViewModel forgotPasswordVM)
		{
			ApplicationResponse responseM = new ApplicationResponse();
			try
			{
				responseM = await _accountService.ForgotPassword(forgotPasswordVM);
				if (responseM.StatusCode == (int)HttpStatusCode.OK)
				{
					return Ok(responseM);
				}
				else
				{
					return BadRequest(responseM);
				}
			}
			catch (Exception ex)
			{

				throw new Exception(ex.Message);
			}
		}
		#endregion

		#region Reset Password method
		/// <summary>
		/// This is the method used to Reset Password
		/// </summary>
		/// <param name="resetPasswordViewModel"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		[AllowAnonymous]
		[HttpPost]
		[Route(ConstantBal.ResetPassword)]
		public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
		{
			ApplicationResponse responseM = new ApplicationResponse();
			try
			{
				responseM = await _accountService.ResetPassword(resetPasswordViewModel);
				if (responseM.StatusCode == (int)HttpStatusCode.OK)
				{
					return Ok(responseM);
				}
				else
				{
					return BadRequest(responseM);
				}

			}
			catch (Exception ex)
			{

				throw new Exception(ex.Message);
			}

		}
		#endregion

		#region Update Profile Method
		/// <summary>
		/// This is the method used to update the profile
		/// </summary>
		/// <param name="updateProfileViewModel"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		[HttpPost]
		[Route(ConstantBal.UpdateProfile)]
		public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileViewModel updateProfileViewModel)
		{
			ApplicationResponse responseM = new ApplicationResponse();
			try
			{
				responseM = await _accountService.UpdateProfile(updateProfileViewModel);
				if (responseM.StatusCode == (int)HttpStatusCode.OK)
				{
					return Ok(responseM);
				}
				else
				{
					return BadRequest(responseM);
				}

			}
			catch (Exception ex)
			{

				throw new Exception(ex.Message);
			}

		}
		#endregion

		#region SearchAll Method
		/// <summary>
		/// This is the method used to search the all string value
		/// </summary>
		/// <param name="searchModel"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		[HttpPost]
		[Route(ConstantBal.SearchAll)]
		public async Task<IActionResult> SearchAll(SearchModel searchModel)
		{
			ApplicationResponse responseM = new ApplicationResponse();
			try
			{
				responseM = await _accountService.SearchAll(searchModel);
				if (responseM.StatusCode == (int)HttpStatusCode.OK)
				{
					return Ok(responseM);
				}
				else
				{
					return BadRequest(responseM);
				}
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}

		}
		#endregion

		#region RemoveUser
		[HttpPost]
		[Route(ConstantBal.RemoveUser)]
		public async Task<IActionResult> RemoveUser(string userId)
		{
			ApplicationResponse responseM = new ApplicationResponse();
			try
			{
				if (ModelState.IsValid)
				{
					responseM = await _accountService.RemoveUser(userId);
					if (responseM.StatusCode == (int)HttpStatusCode.OK)
					{
						return Ok(responseM);
					}
					else
					{
						return BadRequest(responseM);
					}
				}
				else
				{
					return BadRequest(ModelState);
				}
			}
			catch (Exception ex)
			{

				throw new Exception(ex.Message);
			}

		}
		#endregion

		#region Logout Method
		/// <summary>
		/// This method is used for user logout
		/// </summary>
		/// <returns></returns>
		[AllowAnonymous]
		[HttpPost]
		[Route(ConstantBal.Logout)]
		public async Task<IActionResult> Logout()
		{
			Log.Information("Logout started");

			ApplicationResponse responseM = new ApplicationResponse();

			try
			{
				responseM = await _accountService.Logout();

				if (responseM.StatusCode == (int)HttpStatusCode.OK)
				{
					Log.Information("Logout success");
					return Ok(responseM);
				}
				else
				{
					Log.Error("Logout failed");
					return BadRequest(responseM);
				}
			}
			catch (Exception ex)
			{
				Log.Error("Logout failed : " + ex.Message);
				throw new Exception(ex.Message);
			}
		}

		#endregion

		#region Get Secret Credentials
		/// <summary>
		/// This method is used to get secret credentials
		/// </summary>
		/// <returns></returns>
		//[AllowAnonymous]
		[HttpGet]
		[Route(ConstantBal.GetSecretCredentials)]
		public async Task<IActionResult> GetSecretCredentials(string userId)
		{
			Log.Information("Get secret credentials started");

			ApplicationResponse responseM = new ApplicationResponse();

			try
			{
				responseM = await _accountService.GetSecretCredentials();

				if (responseM.StatusCode == (int)HttpStatusCode.OK)
				{
					Log.Information("Get secret credentials success");
					return Ok(responseM);
				}
				else
				{
					Log.Error("Get secret credentials failed");
					return BadRequest(responseM);
				}
			}
			catch (Exception ex)
			{
				Log.Error("Get secret credentials failed : " + ex.Message);
				throw new Exception(ex.Message);
			}
		}

		#endregion

		#region Change Password

		/// <summary>
		/// Changes the password for the currently authenticated user.
		/// </summary>
		[Authorize]
		[HttpPost]
		[Route(ConstantBal.ChangePassword)]
		public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordViewModel changePasswordViewModel)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					Log.Warning("Model state is not valid.");

					return BadRequest(new ApplicationResponse
					{
						Message = "Invalid password details.",
						Status = ConstantBal.False,
						StatusCode = (int)HttpStatusCode.BadRequest,
						Data = ModelState
							.Where(x => x.Value != null && x.Value.Errors.Count > 0)
							.ToDictionary(
								x => x.Key,
								x => x.Value!.Errors
									.Select(e => e.ErrorMessage)
									.ToArray())
					});
				}

				// Get the user ID from the authenticated user's claims.
				var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

				if (string.IsNullOrWhiteSpace(userId))
				{
					Log.Warning("Change password rejected because the authenticated user ID was missing.");

					return Unauthorized(new ApplicationResponse
					{
						Message = FairShareResources.UnAuthenticatedUser,
						Status = ConstantBal.False,
						StatusCode = (int)HttpStatusCode.Unauthorized,
						Data = null
					});
				}

				var response = await _accountService.ChangePasswordAsync(changePasswordViewModel, userId);

				return StatusCode(response.StatusCode ?? (int)HttpStatusCode.InternalServerError, response);
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Unexpected error in ChangePassword endpoint.");

				return StatusCode((int)HttpStatusCode.InternalServerError,
					new ApplicationResponse
					{
						Message = "An unexpected error occurred while changing the password.",
						Status = ConstantBal.False,
						StatusCode = (int)HttpStatusCode.InternalServerError,
						Data = null
					});
			}
		}
		#endregion



		[ApiExplorerSettings(IgnoreApi = true)]
		[AllowAnonymous]
		[HttpPost]
		[Route(ConstantBal.SuccessStatus)]
		public async Task<IActionResult> Login2(LoginViewModel loginViewModel)
		{
			// Thread where the request starts (sync part, before any await)
			Log.Information("{Message} | ThreadId: {ThreadId}",
				APIResource.LoghinStart, Environment.CurrentManagedThreadId);

			ApplicationResponse responseM = new ApplicationResponse();
			try
			{
				Log.Information("Calling _accountService.Login | ThreadId (before await): {ThreadId}",
					Environment.CurrentManagedThreadId);

				responseM = await _accountService.Login(loginViewModel);

				// After await, the continuation may run on a different thread
				Log.Information("_accountService.Login returned StatusCode: {StatusCode} | ThreadId (after await): {ThreadId}",
					responseM?.StatusCode, Environment.CurrentManagedThreadId);
				var d = SynchronizationContext.Current;
				if (responseM?.StatusCode == (int)HttpStatusCode.OK)
				{
					Log.Information("{Message} | ThreadId: {ThreadId}",
						APIResource.LoginSuccess, Environment.CurrentManagedThreadId, d);
					return Ok(responseM);
				}
				else
				{
					Log.Error("{Message} | StatusCode: {StatusCode} | ThreadId: {ThreadId}",
						APIResource.LoginFailed, responseM?.StatusCode, Environment.CurrentManagedThreadId);
					return BadRequest(responseM);
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex, "{Message}{Error} | ThreadId: {ThreadId}",
					APIResource.LoginFailed, ex.Message, Environment.CurrentManagedThreadId);
				throw new Exception(ex.Message);
			}
		}
	}
}
