using Dto.Request.Dto.Request;
using Dto.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.UWO;
using Service.Interface;
using System.Security.Claims;

namespace API_UsePrevention.Controllers
{
    namespace API_UsePrevention.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class ResultsController : ControllerBase
        {
            private readonly IResultService _resultService;
            private readonly IUnitOfWork _unitOfWork;

            public ResultsController(IResultService resultService, IUnitOfWork unitOfWork)
            {
                _resultService = resultService;
                _unitOfWork = unitOfWork;
            }

            [HttpGet("{resultId}")]
            [Authorize]
            public async Task<ActionResult<ResultResponseDto>> GetResult(int resultId)
            {
                try
                {
                    var emailClaim = User.FindFirst(ClaimTypes.Email)?.Value;
                    if (string.IsNullOrEmpty(emailClaim))
                        return Unauthorized("Email claim is missing in the JWT token");

                    var users = _unitOfWork.User.Find(u => u.Email == emailClaim);
                    var user = users.FirstOrDefault();
                    if (user == null)
                        return Unauthorized("User not found");

                    var result = await _resultService.GetResultByIdAsync(resultId);
                    if (result.UserId != user.Id && user.RoleId != 5) // Chỉ user hoặc consultant được xem
                        return Unauthorized("Not authorized to view this result");

                    return Ok(result);
                }
                catch (KeyNotFoundException ex)
                {
                    return NotFound(ex.Message);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"An error occurred: {ex.Message}");
                }
            }

            [HttpGet("user")]
            [Authorize]
            public async Task<ActionResult<IEnumerable<ResultResponseDto>>> GetUserResults()
            {
                try
                {
                    var emailClaim = User.FindFirst(ClaimTypes.Email)?.Value;
                    if (string.IsNullOrEmpty(emailClaim))
                        return Unauthorized("Email claim is missing in the JWT token");

                    var users = _unitOfWork.User.Find(u => u.Email == emailClaim);
                    var user = users.FirstOrDefault();
                    if (user == null)
                        return Unauthorized("User not found");

                    var results = await _resultService.GetResultsByUserIdAsync(user.Id);
                    return Ok(results);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"An error occurred: {ex.Message}");
                }
            }

            [HttpPut("{resultId}")]
            [Authorize]
            public async Task<ActionResult> UpdateResult(int resultId, [FromBody] UpdateResultRequestDto request)
            {
                try
                {
                    var emailClaim = User.FindFirst(ClaimTypes.Email)?.Value;
                    if (string.IsNullOrEmpty(emailClaim))
                        return Unauthorized("Email claim is missing in the JWT token");

                    var users = _unitOfWork.User.Find(u => u.Email == emailClaim);
                    var user = users.FirstOrDefault();
                    if (user == null)
                        return Unauthorized("User not found");

                    if (user.RoleId != 5) // Chỉ consultant được cập nhật
                        return Unauthorized("Only consultants (RoleId = 5) can update results");

                    await _resultService.UpdateResultAsync(resultId, request);
                    return NoContent();
                }
                catch (KeyNotFoundException ex)
                {
                    return NotFound(ex.Message);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"An error occurred: {ex.Message}");
                }
            }

            [HttpDelete("{resultId}")]
            [Authorize]
            public async Task<ActionResult> DeleteResult(int resultId)
            {
                try
                {
                    var emailClaim = User.FindFirst(ClaimTypes.Email)?.Value;
                    if (string.IsNullOrEmpty(emailClaim))
                        return Unauthorized("Email claim is missing in the JWT token");

                    var users = _unitOfWork.User.Find(u => u.Email == emailClaim);
                    var user = users.FirstOrDefault();
                    if (user == null)
                        return Unauthorized("User not found");

                    if (user.RoleId != 5) // Chỉ consultant được xóa
                        return Unauthorized("Only consultants (RoleId = 5) can delete results");

                    await _resultService.DeleteResultAsync(resultId);
                    return NoContent();
                }
                catch (KeyNotFoundException ex)
                {
                    return NotFound(ex.Message);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"An error occurred: {ex.Message}");
                }
            }
        }
    }
}
