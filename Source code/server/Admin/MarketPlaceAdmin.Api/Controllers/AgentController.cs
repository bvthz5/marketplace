using MarketPlaceAdmin.Bussiness.Dto.Forms;
using MarketPlaceAdmin.Bussiness.Dto.Forms.Validations;
using MarketPlaceAdmin.Bussiness.Helper;
using MarketPlaceAdmin.Bussiness.Interfaces;
using MarketPlaceAdmin.Bussiness.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlaceAdmin.Api.Controllers
{
    /// <summary>
    /// API controller for managing agent account related tasks.
    /// </summary>
    [ApiController]
    [Route("api/agent")]
    public class AgentController : ControllerBase
    {
        private readonly IAgentService _agentService;
        private readonly ISecurityUtil _securityUtil;

        /// <summary>
        /// Constructor for AgentController.
        /// </summary>
        /// <param name="agentService">The service for managing agent account related tasks.</param>
        /// <param name="securityUtil">The service for managing agent account related tasks.</param>
        /// 
        public AgentController(IAgentService agentService, ISecurityUtil securityUtil)
        {
            _agentService = agentService;
            _securityUtil = securityUtil;
        }

        /// <summary>
        /// Adds a new Agent account by admin to the system
        /// </summary>
        /// <param name="form">Agent registration form data</param>
        /// <returns>ServiceResult object containing the operation status and any relevant data</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost(Name = "post-agent")]
        public async Task<IActionResult> Add([FromBody] AgentRegistrationForm form)
        {
            ServiceResult result = await _agentService.AddAgentAsync(form);
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Retrieves a paginated list of Agent based on the provided search and sort parameters.
        /// </summary>
        /// <param name="form">A AgentPaginationParams object containing pagination, search, and sorting parameters.</param>
        /// <returns>A ServiceResult containing the paginated list of agents.</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> PaginatedAgentList([FromQuery] AgentPaginationParams form)
        {
            ServiceResult result = await _agentService.AgentListAsync(form);
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Action method for updating the current Agents's information.
        /// Requires an authenticated agent.
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="form">The updated agent information.</param>
        /// <returns>The updated agent's information.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("{agentId:int:min(1)}")]
        public async Task<IActionResult> EditAgent(int agentId, [FromBody] EditAgentForm form)
        {
            ServiceResult result = await _agentService.EditAgentAsync(agentId, form);
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [Authorize(Roles = "Agent")]
        [HttpPut("profile")]
        public async Task<IActionResult> EditAgent([FromBody] EditAgentForm form)
        {
            ServiceResult result = await _agentService.EditAgentAsync(_securityUtil.GetCurrentUserId(), form);
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Changes a Agent's status.
        /// </summary>
        /// <param name="agentId">The ID of the agent to update.</param>
        /// <param name="status">The new status to set for the agent.</param>
        [Authorize(Roles = "Admin")]
        [HttpPut("status/{agentId:int:min(1)}")]
        public async Task<IActionResult> ChangeAgentStatus(int agentId, [FromBody] byte status)
        {
            ServiceResult result = await _agentService.ChangeAgentStatusAsync(agentId, status);
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Action method for changing the agents's password.
        /// Requires an authenticated agent.
        /// </summary>
        /// <param name="form">The form data containing the password change information.</param>
        /// <returns>The status of the password change operation.</returns>
        [Authorize(Roles = "Agent")]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordForm form)
        {
            ServiceResult result = await _agentService.ChangePassword(form, _securityUtil.GetCurrentUserId());
            return StatusCode((int)result.ServiceStatus, result);

        }

        /// <summary>
        /// Generates a reset password token using the registered email address.
        /// </summary>
        /// <param name="email">The email address of the user requesting a password reset.</param>
        [AllowAnonymous]
        [HttpPut("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody][Email] string email)
        {
            ServiceResult result = await _agentService.ForgotPassword(email);
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// Resets the password using the received token via email.
        /// </summary>
        /// <param name="form">The form containing the token and new password.</param>
        [AllowAnonymous]
        [HttpPut("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ForgotPasswordForm form)
        {
            ServiceResult result = await _agentService.ResetPassword(form);
            return StatusCode((int)result.ServiceStatus, result);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Agent")]
        [HttpGet("profile")]
        public async Task<IActionResult> GetAgentProfile()
        {
            ServiceResult result = await _agentService.GetAgent(_securityUtil.GetCurrentUserId());
            return StatusCode((int)result.ServiceStatus, result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Agent")]
        [HttpPut("profile-pic")]
        public async Task<IActionResult> PutAgentProfile([FromForm] ImageForm form)
        {
            ServiceResult result = await _agentService.SetProfilePic(_securityUtil.GetCurrentUserId(), form);
            return StatusCode((int)result.ServiceStatus, result);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpGet("profile-pic/{fileName}")]
        public async Task<IActionResult> GetAgentProfilePic(string fileName)
        {
            FileStream? fileStream = await _agentService.GetAgentProfilePic(fileName);
            if (fileStream is null)
                return NotFound();
            return File(fileStream, "image/jpeg");
        }


    }
}

