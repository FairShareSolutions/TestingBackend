using Common;
using FairShareSolutions_PYMT.Infrastructure.DataModel;
using FairShareSolutions_PYMT.Infrastructure.ResponseModel;
using FairShareSolutions_PYMT.Infrastructure.ViewModel;
using FairShareSolutions_PYMT.Services.IServices;
using FairShareSolutions_PYMT.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Net;
using System.Runtime;
using System.Text.Json;

namespace FairShareSolutions_PYMT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AnonymousGroupController : ControllerBase
    {
        private readonly IAnonymousGroupServices _anonymousGroupServices;
        public AnonymousGroupController(IAnonymousGroupServices anonymousGroupServices)
        {
            _anonymousGroupServices = anonymousGroupServices;
        }

        #region AnonymousGroupInviteFriend 
        /// <summary>
        /// This method is used to Anonymous Group InviteFriend
        /// </summary>
        /// <param name="anonymousGroupViewModel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPost]
        [Route(ConstantBal.AnonymousGroupInviteFriend)]
        public async Task<IActionResult> AnonymousGroupInviteFriend(AnonymousGroupViewModel anonymousGroupViewModel)
        {
            ApplicationResponse applicationResponse = new ApplicationResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    applicationResponse = await _anonymousGroupServices.AnonymousGroupInviteFriend(anonymousGroupViewModel);
                    if (applicationResponse.StatusCode == (int)HttpStatusCode.OK)
                    {
                        return Ok(applicationResponse);
                    }
                    else
                    {
                        return BadRequest(applicationResponse);
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

        #region GetAllGroup 
        /// <summary>
        /// This method is used to Get All Group List
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(ConstantBal.GetAllGroupListUserId)]
        public async Task<IActionResult> GetAllGroupListUserId(string userId)
        {
            ApplicationResponse response = new ApplicationResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    response = await _anonymousGroupServices.GetAllGroupListUserId(userId);
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

        #region SendingNotificationAdmin 
        /// <summary>
        /// This method is used to Sending Notification Admin
        /// </summary>
        /// <param name="editItemsList"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(ConstantBal.SendingNotificationAdmin)]
        public async Task<IActionResult> EditItemsList(EditItemsList editItemsList)
        {
            ApplicationResponse response = new ApplicationResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    response = await _anonymousGroupServices.EditItemsList(editItemsList);
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

        #region UpdateAddItems 
        /// <summary>
        /// This method is used to Update Add Items Bill
        /// </summary>
        /// <param name="updateBillViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(ConstantBal.UpdateAddItems)]
        public async Task<IActionResult> EditEqualSplitBill(UpdateBillViewModel updateBillViewModel)
        {
            ApplicationResponse response = new ApplicationResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    response = await _anonymousGroupServices.EditEqualSplitBill(updateBillViewModel);
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

        #region UpdateEqualSplitBills 
        /// <summary>
        /// This method is used to Update Equal Split Bill
        /// </summary>
        /// <param name="updateEqualSplitsViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(ConstantBal.UpdateEqualSplitBills)]
        public async Task<IActionResult> UpdateEqualSplitBills(UpdateEqualSplitsViewModel updateEqualSplitsViewModel)
        {
            ApplicationResponse response = new ApplicationResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    response = await _anonymousGroupServices.UpdateEqualSplitBills(updateEqualSplitsViewModel);
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

        #region UpdateSelectItemsSplitBill 
        /// <summary>
        /// This method is used to Update Equal Split Bill
        /// </summary>
        /// <param name="updateSelectItemsViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(ConstantBal.UpdateSelectItemsSplitBill)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> EditSelectItemsSplitBill(UpdateSelectItemsViewModel updateSelectItemsViewModel)
        {
            ApplicationResponse response = new ApplicationResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    response = await _anonymousGroupServices.EditSelectItemsSplitBill(updateSelectItemsViewModel);
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

        #region UpdatePercentageSplitBill 
        /// <summary>
        /// This method is used to Update Percentage Split Bill
        /// </summary>
        /// <param name="updatePercentageViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(ConstantBal.UpdatePercentageSplitBill)]
        public async Task<IActionResult> EditPercentageSplitBill(UpdatePercentageViewModel updatePercentageViewModel)
        {
            ApplicationResponse response = new ApplicationResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    response = await _anonymousGroupServices.EditPercentageSplitBill(updatePercentageViewModel);
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
                Log.Information("Exception occurs in UpdatePercentageSplitBill api end point" , ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { message = ex.Message });
            }
        }
        #endregion

        #region DeleteAllBill 
        /// <summary>
        /// This method is used to Delete All Split Bill
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="AdminId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(ConstantBal.DeleteAllSplitBill)]
        public async Task<IActionResult> DeleteAllSplitBill(int groupId, string AdminId)
        {
            ApplicationResponse response = new ApplicationResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    response = await _anonymousGroupServices.DeleteAllSplitBill(groupId, AdminId);
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

        #region DeleteGroup 
        /// <summary>
        /// This method is used to DeleteGroup 
        /// </summary>
        /// <param name="groupDeleteViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(ConstantBal.DeleteGroup)]
        public async Task<IActionResult> DeleteGroup(GroupDeleteViewModel groupDeleteViewModel)
        {
            ApplicationResponse response = new ApplicationResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    response = await _anonymousGroupServices.DeleteGroup(groupDeleteViewModel);
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

        [HttpPost]
        [Route(ConstantBal.DeleteGroupData)]
        public async Task<IActionResult> DeleteAllGroupType(GroupDeleteViewModel groupDeleteViewModel)
        {
            ApplicationResponse response = new ApplicationResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    response = await _anonymousGroupServices.DeleteAllGroupType(groupDeleteViewModel);
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

        #region ClearAllPastGroup 
        /// <summary>
        /// This method is used to clear the all past group based on the user Id 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(ConstantBal.ClearAllPastGroup)]
        public async Task<IActionResult> ClearAllPastGroup(string userId)
        {
            ApplicationResponse response = new ApplicationResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    response = await _anonymousGroupServices.ClearAllPastGroup(userId);
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

        #region UpdateGroupDetails
        [HttpPost("annonymous-group/update")]
		public async Task<IActionResult> UpdateGroupDetails(UpdateGroupInfoViewModel model)
		{
			Log.Information("Preparing the Udpdate Group Functionality Request");

			if (!ModelState.IsValid)
			{
				return BadRequest(new ApiResponse
				{
					Success = false,
					Message = FairShareResources.InvalidInput,
					Errors = ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)).ToList()
				});
			}

			var result = await _anonymousGroupServices.UpdateGroup(model);

			if (!result.Status)
			{
				return BadRequest(result);
			}

			return Ok(result);
		}
		#endregion

	}
}
