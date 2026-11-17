using Common;
using FairShareSolutions_PYMT.Infrastructure.ResponseModel;
using FairShareSolutions_PYMT.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FairShareSolutions_PYMT.API.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	[ApiController]
	public class DashboardController : ControllerBase
	{
		private readonly IAnalyticsService _analytics;
		public DashboardController(IAnalyticsService analytics)
		{
			_analytics = analytics;
		}


		#region Analytics
		[HttpPost]
		[Route(ConstantBal.Analytics)]
		public async Task<IActionResult> Analytics(string userId)
		{
			ApplicationResponse responseM = new ApplicationResponse();
			try
			{
				if (ModelState.IsValid)
				{
					responseM = await _analytics.GetUserAnalytics(userId);
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
	}
}

