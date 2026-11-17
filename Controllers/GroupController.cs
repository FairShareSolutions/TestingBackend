using Common;
using FairShareSolutions_PYMT.API.Resource;
using FairShareSolutions_PYMT.Infrastructure.ResponseModel;
using FairShareSolutions_PYMT.Infrastructure.ViewModel;
using FairShareSolutions_PYMT.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using Serilog;
using System.Net;
using System.Security.Claims;
namespace FairShareSolutions_PYMT.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class GroupController : ControllerBase
	{
		private readonly IGroupDetailService _dashboardService;
		private readonly ILogger<GroupController> _logger;
		public GroupController(IGroupDetailService dashboardService, ILogger<GroupController> logger)
		{
			_dashboardService = dashboardService;
			_logger = logger;
		}

		#region Get GroupById
		/// <summary>
		///  This method is used to get a group by Id
		/// </summary>
		/// <param name="groupId"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		[HttpGet]
		[Route(ConstantBal.GetGroupById)]
		public async Task<IActionResult> GetGroupById(int groupId)
		{
			ApplicationResponse responseM = new ApplicationResponse();
			try
			{
				responseM = await _dashboardService.GetGroupById(groupId);
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

		#region GetAllGroup 
		/// <summary>
		/// This method is used to Get All Group List
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="PageNumber"></param>
		/// <param name="Sorting"></param>
		/// <returns></returns>
		[HttpGet]
		[Route(ConstantBal.GetAllGroupList)]
		public async Task<IActionResult> GetAllGroupList(string userId, int pageNumber, string sorting)
		{
			ApplicationResponse response = new ApplicationResponse();
			try
			{
				if (ModelState.IsValid)
				{
					response = await _dashboardService.GetAllGroupList(userId, pageNumber, sorting);
					if (response.StatusCode == (int)HttpStatusCode.OK)
					{
						return Ok(response);
					}
					else
					{
						return BadRequest(response);
					}
				}
				else
				{
					return BadRequest(ModelState);
				}
			}
			catch (Exception ex)
			{
				return StatusCode((int)HttpStatusCode.InternalServerError, new { message = ex.Message });
			}
		}
		#endregion

		#region Create Group
		/// <summary>
		/// This method is used to create a group
		/// </summary>
		/// <param name="group"></param>
		/// <returns></returns>
		[HttpPost]
		[Route(ConstantBal.CreateGroup)]
		//[ApiExplorerSettings(IgnoreApi = true)]
		public async Task<IActionResult> CreateGroup(CreateGroupViewModel group)
		{
			ApplicationResponse response = new ApplicationResponse();
			try
			{
				if (ModelState.IsValid)
				{
					_logger.LogInformation(APIResource.LogCreate, group);
					response = await _dashboardService.CreateGroup(group);
					if (response.StatusCode == (int)HttpStatusCode.OK)
					{
						_logger.LogInformation(APIResource.LogCreatedSuccess, response.Data);
						return Ok(response);
					}
					else
					{
						_logger.LogWarning(APIResource.LogCreateFaild, response);
						return BadRequest(response);
					}
				}
				else
				{
					_logger.LogWarning(APIResource.LogInvalidModel, ModelState);
					return BadRequest(ModelState);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, APIResource.LogCatch);
				return StatusCode((int)HttpStatusCode.InternalServerError, new { message = ex.Message });
			}
		}
		#endregion

		#region ReceiptImage
		/// <summary>
		/// This method is used to upload a receipt image
		/// </summary>
		/// <param name="receiptImageViewModel"></param>
		/// <returns></returns>
		//[ApiExplorerSettings(IgnoreApi = true)]
		[HttpPost]
		[Route(ConstantBal.ReceiptImage)]
		public async Task<IActionResult> UploadedReceiptImage(ReceiptImageViewModel receiptImageViewModel)
		{
			ApplicationResponse response = new ApplicationResponse();
			try
			{
				if (ModelState.IsValid)
				{
					response = await _dashboardService.UploadedReceiptImage(receiptImageViewModel);
					if (response.StatusCode == (int)HttpStatusCode.OK)
					{
						return Ok(response);
					}
					else
					{
						return BadRequest(response);
					}
				}
				else
				{
					return BadRequest(ModelState);
				}
			}
			catch (Exception ex)
			{
				return StatusCode((int)HttpStatusCode.InternalServerError, new { message = ex.Message });
			}
		}
		#endregion

		#region UpdateGroup
		/// <summary>
		/// This method is used to update a group
		/// </summary>
		/// <param name="group"></param>
		/// <returns></returns>
		[HttpPost]
		[Route(ConstantBal.UpdateGroup)]
		public async Task<IActionResult> UpdateGroup(UpdateGroupViewModel group)
		{
			ApplicationResponse response = new ApplicationResponse();
			try
			{
				if (ModelState.IsValid)
				{
					response = await _dashboardService.UpdateGroup(group);
					if (response.StatusCode == (int)HttpStatusCode.OK)
					{
						return Ok(response);
					}
					else
					{
						return BadRequest(response);
					}
				}
				else
				{
					return BadRequest(ModelState);
				}
			}
			catch (Exception ex)
			{
				return StatusCode((int)HttpStatusCode.InternalServerError, new { message = ex.Message });
			}
		}
		#endregion

		#region InviteFriends
		/// <summary>
		/// This method is used to Invite Friends
		/// </summary>
		/// <param name="friendsViewModel"></param>
		/// <returns></returns>
		[HttpPost]
		[Route(ConstantBal.InviteFriends)]
		public async Task<IActionResult> InviteFriends(InviteFriendListViewModel friendsViewModel)
		{
			ApplicationResponse response = new ApplicationResponse();
			try
			{
				if (ModelState.IsValid)
				{
					response = await _dashboardService.InviteFriends(friendsViewModel);
					if (response.StatusCode == (int)HttpStatusCode.OK)
					{
						return Ok(response);
					}
					else
					{
						return BadRequest(response);
					}
				}
				else
				{
					return BadRequest(ModelState);
				}
			}
			catch (Exception ex)
			{
				return StatusCode((int)HttpStatusCode.InternalServerError, new { message = ex.Message });
			}
		}
		#endregion

		#region JoinGroup
		/// <summary>
		/// This method is used to join a group Request
		/// </summary>
		/// <param name="groupId"></param>
		/// <param name="userId"></param>
		/// <param name="InviteRequestStatus"></param>
		/// <returns></returns>
		[HttpPost]
		[Route(ConstantBal.JoinGroup)]
		public async Task<IActionResult> JoinGroup(int groupId, string userId, string inviteRequestStatus)
		{
			ApplicationResponse response = new ApplicationResponse();
			try
			{
				if (ModelState.IsValid)
				{
					response = await _dashboardService.JoinGroup(groupId, userId, inviteRequestStatus);
					if (response.StatusCode == (int)HttpStatusCode.OK)
					{
						return Ok(response);
					}
					else
					{
						return BadRequest(response);
					}
				}
				else
				{
					return BadRequest(ModelState);
				}
			}
			catch (Exception ex)
			{
				return StatusCode((int)HttpStatusCode.InternalServerError, new { message = ex.Message });
			}
		}
		#endregion

		#region DeclineGroup
		/// <summary>
		/// This method is used to Decline a group!
		/// </summary>
		/// <param name="groupId"></param>
		/// <param name="userId"></param>
		/// <returns></returns>
		[ApiExplorerSettings(IgnoreApi = true)]
		[HttpPost]
		[Route(ConstantBal.DeclineGroup)]
		public async Task<IActionResult> DeclineGroup(int groupId, string userId)
		{
			ApplicationResponse response = new ApplicationResponse();
			try
			{
				if (ModelState.IsValid)
				{
					response = await _dashboardService.DeclineGroup(groupId, userId);
					if (response.StatusCode == (int)HttpStatusCode.OK)
					{
						return Ok(response);
					}
					else
					{
						return BadRequest(response);
					}
				}
				else
				{
					return BadRequest(ModelState);
				}
			}
			catch (Exception ex)
			{
				return StatusCode((int)HttpStatusCode.InternalServerError, new { message = ex.Message });
			}
		}
		#endregion

		#region Equalsplit
		/// <summary>
		/// This method is used to payment Equal a split
		/// </summary>
		/// <param name="splitViewModel"></param>
		/// <returns></returns>
		[HttpPost]
		[Route(ConstantBal.EqualSplit)]
		public async Task<IActionResult> EqualSplit(EqualSplitViewModel splitViewModel)
		{
			ApplicationResponse response = new ApplicationResponse();
			try
			{
				if (ModelState.IsValid)
				{
					response = await _dashboardService.EqualSplit(splitViewModel);
					if (response.StatusCode == (int)HttpStatusCode.OK)
					{
						return Ok(response);
					}
					else
					{
						return BadRequest(response);
					}
				}
				else
				{
					return BadRequest(ModelState);
				}
			}
			catch (Exception ex)
			{
				return StatusCode((int)HttpStatusCode.InternalServerError, new { message = ex.Message });
			}
		}
		#endregion

		#region Select Item for Member 
		/// <summary>
		/// This method is used to create a bill of selected items!
		/// </summary>
		/// <param name="selectItemsSplitViewModel"></param>
		/// <returns></returns>
		[HttpPost]
		[Route(ConstantBal.SelectItemSplit)]
		[ApiExplorerSettings(IgnoreApi = true)]
		public async Task<IActionResult> SelectItemSplit(ItemSelectSplitViewModel selectItemsSplitViewModel)
		{
			ApplicationResponse response = new ApplicationResponse();
			try
			{
				if (ModelState.IsValid)
				{
					response = await _dashboardService.SelectItemSplit(selectItemsSplitViewModel);
					if (response.StatusCode == (int)HttpStatusCode.OK)
					{
						return Ok(response);
					}
					else
					{
						return BadRequest(response);
					}
				}
				else
				{
					return BadRequest(ModelState);
				}
			}
			catch (Exception ex)
			{
				return StatusCode((int)HttpStatusCode.InternalServerError, new { message = ex.Message });
			}
		}
		#endregion

		#region Create Bill ItemSelect Add
		/// <summary>
		/// This method is used to create a bill of selected items!
		/// </summary>
		/// <param name="equalSplitViewModel"></param>
		/// <returns></returns>
		// [ApiExplorerSettings(IgnoreApi = true)]
		[HttpPost]
		[Route(ConstantBal.ItemsAdd)]
		public async Task<IActionResult> CreateBill(AddItemsDetails equalSplitViewModel)
		{
			ApplicationResponse response = new ApplicationResponse();
			try
			{
				if (ModelState.IsValid)
				{
					response = await _dashboardService.CreateBill(equalSplitViewModel);
					if (response.StatusCode == (int)HttpStatusCode.OK)
					{
						return Ok(response);
					}
					else
					{
						return BadRequest(response);
					}
				}
				else
				{
					return BadRequest(ModelState);
				}
			}
			catch (Exception ex)
			{
				return StatusCode((int)HttpStatusCode.InternalServerError, new { message = ex.Message });
			}
		}
		#endregion

		#region Get EqualSplitData ByGroupId
		/// <summary>
		/// This method is used to get a group by Id
		/// </summary>
		/// <param name="groupId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route(ConstantBal.GetAllSplitData)]
		public async Task<IActionResult> GetEqualSplitData(int groupId)
		{
			ApplicationResponse responseM = new ApplicationResponse();
			try
			{
				responseM = await _dashboardService.GetEqualSplitData(groupId);
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

		#region Select Item for Member 
		/// <summary>
		/// This method is used to create a bill of selected items!
		/// </summary>
		/// <param name="selectItemsSplitViewModel"></param>
		/// <returns></returns>
		//[ApiExplorerSettings(IgnoreApi = true)]
		[HttpPost]
		[Route(ConstantBal.SelectItem)]
		public async Task<IActionResult> SelectItems(SelectItemsSplitViewModel selectItemsSplitViewModel)
		{
			ApplicationResponse response = new ApplicationResponse();
			try
			{
				if (ModelState.IsValid)
				{
					response = await _dashboardService.SelectItems(selectItemsSplitViewModel);
					if (response.StatusCode == (int)HttpStatusCode.OK)
					{
						return Ok(response);
					}
					else
					{
						return BadRequest(response);
					}
				}
				else
				{
					return BadRequest(ModelState);
				}
			}
			catch (Exception ex)
			{
				return StatusCode((int)HttpStatusCode.InternalServerError, new { message = ex.Message });
			}
		}
		#endregion

		#region PercentageSplit for Member
		/// <summary>
		/// This method is used to member amount split percentage wise!
		/// </summary>
		/// <param name="percentageSplitViewModel"></param>
		/// <returns></returns>
		[HttpPost]
		[Route(ConstantBal.PercentageSplit)]
		public async Task<IActionResult> PercentageSplit(PercentageSplitViewModel percentageSplitViewModel)
		{
			ApplicationResponse response = new ApplicationResponse();
			try
			{
				if (ModelState.IsValid)
				{
					response = await _dashboardService.PercentageSplit(percentageSplitViewModel);
					if (response.StatusCode == (int)HttpStatusCode.OK)
					{
						return Ok(response);
					}
					else
					{
						return BadRequest(response);
					}
				}
				else
				{
					return BadRequest(ModelState);
				}
			}
			catch (Exception ex)
			{
				return StatusCode((int)HttpStatusCode.InternalServerError, new { message = ex.Message });
			}
		}
		#endregion

		#region Notification for Invited Member
		/// <summary>
		/// This method is used to member notification for the admin!
		/// </summary>
		/// <param name="MemberId"></param>
		/// <param name="NotificationId"></param>
		/// <param name="Action"></param>
		/// <returns></returns>
		[HttpPost]
		[Route(ConstantBal.InviteNotificationUpdate)]
		public async Task<IActionResult> InviteNotificationUpdate(string memberId, int notificationId, bool Action)
		{
			ApplicationResponse response = new ApplicationResponse();
			try
			{
				if (ModelState.IsValid)
				{
					response = await _dashboardService.InviteNotification(memberId, notificationId, Action);
					if (response.StatusCode == (int)HttpStatusCode.OK)
					{
						return Ok(response);
					}
					else
					{
						return BadRequest(response);
					}
				}
				else
				{
					return BadRequest(ModelState);
				}
			}
			catch (Exception ex)
			{
				return StatusCode((int)HttpStatusCode.InternalServerError, new { message = ex.Message });
			}
		}
		#endregion

		#region Get all member items detail by GroupId and MemberId 
		/// <summary>
		/// This method is used to get a group by Id
		/// </summary>
		/// <param name="groupId"></param>
		/// <param name="MemberId"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		[HttpGet]
		[Route(ConstantBal.GetAllMemberItems)]
		public async Task<IActionResult> GetAllMemberItems(int groupId, string memberId)
		{
			ApplicationResponse responseM = new ApplicationResponse();
			try
			{
				responseM = await _dashboardService.GetAllMemberItems(groupId, memberId);
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

		#region Get all Notification detail by UserId 
		/// <summary>
		/// This method is used to get a group by Id
		/// </summary>
		/// <param name="UserId"></param>
		/// <param name="PageNumber"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		[HttpGet]
		[Route(ConstantBal.GetAllNotification)]
		public async Task<IActionResult> GetAllNotification(string userId, int pageNumber)
		{
			ApplicationResponse responseM = new ApplicationResponse();
			try
			{
				responseM = await _dashboardService.GetAllNotification(userId, pageNumber);
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

		#region GetAdminMemberDetails 
		/// <summary>
		/// This method is used to Get All Group List
		/// </summary>
		/// <param name="AdminId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route(ConstantBal.GetAdminMemberDetails)]
		public async Task<IActionResult> GetAdminMemberDetails(string adminId)
		{
			ApplicationResponse response = new ApplicationResponse();
			try
			{
				if (ModelState.IsValid)
				{
					response = await _dashboardService.GetAdminMemberDetails(adminId);
					if (response.StatusCode == (int)HttpStatusCode.OK)
					{
						return Ok(response);
					}
					else
					{
						return BadRequest(response);
					}
				}
				else
				{
					return BadRequest(ModelState);
				}
			}
			catch (Exception ex)
			{
				return StatusCode((int)HttpStatusCode.InternalServerError, new { message = ex.Message });
			}
		}
		#endregion

		#region GetItemSelectionCount 
		/// <summary>
		/// This method is used to Get Item Selection Count
		/// </summary>
		/// <param name="GroupID"></param>
		/// <param name="MemberId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route(ConstantBal.GetItemSelectionCount)]
		public async Task<IActionResult> GetItemSelectionCount(int groupID, string memberId)
		{
			ApplicationResponse response = new ApplicationResponse();
			try
			{
				if (ModelState.IsValid)
				{
					response = await _dashboardService.GetItemSelectionCount(groupID, memberId);
					if (response.StatusCode == (int)HttpStatusCode.OK)
					{
						return Ok(response);
					}
					else
					{
						return BadRequest(response);
					}
				}
				else
				{
					return BadRequest(ModelState);
				}
			}
			catch (Exception ex)
			{
				return StatusCode((int)HttpStatusCode.InternalServerError, new { message = ex.Message });
			}
		}
		#endregion

		#region SendNotification
		[HttpGet("send Notification")]
		public async Task<IActionResult> SendNotification(int NotificationTypeFlag, string Title, string Body, string Token, string Data, string UserId, string RequestUserName)
		{
			var data = _dashboardService.SendNotificationAsync(108, Title, Body, Token, Data, UserId, RequestUserName);
			return Ok(data);
		}
		#endregion

		#region Roulette Bill
		/// <summary>
		/// Creates a Credit Card Roulette bill for a group.
		/// </summary>
		/// <param name="request">Roulette bill details.</param>
		/// <returns>Bill creation response.</returns>
		[HttpPost]
		[Route(ConstantBal.RouletteBillAdd)]
		public async Task<IActionResult> CreateRouletteBill([FromBody] CreateRouletteBillRequest request)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					Log.Information("CreateRouletteBill: Invalid request received.- Model is not Valid");

					return BadRequest(ModelState);
				}

				string? userId = User.FindFirstValue(
					ClaimTypes.NameIdentifier);

				if (string.IsNullOrWhiteSpace(userId))
				{
					Log.Information("CreateRouletteBill: User ID not found.");

					return Unauthorized();
				}

				Log.Information("CreateRouletteBill started for GroupId: {GroupId}", request.GroupId);

				ApplicationResponse response = await _dashboardService.CreateRouletteBillAsync(request, userId);

				if (response.StatusCode == (int)HttpStatusCode.OK)
				{
					return Ok(response);
				}

				if (response.StatusCode == (int)HttpStatusCode.NotFound)
				{
					return NotFound(response);
				}

				if (response.StatusCode == (int)HttpStatusCode.Forbidden)
				{
					return StatusCode(
						(int)HttpStatusCode.Forbidden, response);
				}

				if (response.StatusCode == (int)HttpStatusCode.BadRequest)
				{
					return BadRequest(response);
				}

				int statusCode = response.StatusCode.HasValue && response.StatusCode.Value != 0 ? response.StatusCode.Value : (int)HttpStatusCode.InternalServerError;
				return StatusCode(statusCode, response);
			}
			catch (Exception ex)
			{
				Log.Error(
					ex,
					"CreateRouletteBill controller failed.");

				return StatusCode(
					(int)HttpStatusCode.InternalServerError,
					new ApplicationResponse
					{
						Status = ConstantBal.False,
						StatusCode = (int)HttpStatusCode.InternalServerError,
						Message = "Unable to create roulette bill.",
						Data = null
					});
			}
		}
		#endregion

	}
}