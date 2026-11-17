using Common;
using FairShareSolutions_PYMT.API.Resource;
using FairShareSolutions_PYMT.API.SignalR;
using FairShareSolutions_PYMT.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace FairShareSolutions_PYMT.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class BillScanController : ControllerBase
	{
		private readonly IAwsTextractService _textractService;
		private readonly IHubContext<GetItems> _hubContext;

		public BillScanController(IAwsTextractService textractService, IHubContext<GetItems> hubContext)
		{
			_textractService = textractService;
			_hubContext = hubContext;
		}
		#region ScanBill 
		/// <summary>
		/// This method is used to Scan Bill
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		[AllowAnonymous]
		[HttpPost]
		[Route(ConstantBal.ScanBill)]
		public async Task<IActionResult> ScanBill(IFormFile file)
		{
			try
			{
				if (file == null || file.Length == 0)
				{ 
					return BadRequest(APIResource.NoFileUploaded);
				}
				using var stream = file.OpenReadStream();
				var result = await _textractService.ExtractTextFromImageAsync(stream);

				if (result.Status == true)
				{
					// var FileUpload = _textractService.UploadFileToS3(file);
				}
				return Ok(result);
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}
		#endregion
	}
}
