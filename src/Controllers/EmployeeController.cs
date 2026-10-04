using System.Security.Claims;
using api_clinic.src.Interfaces;
using api_clinic.src.Models.Base;
using api_clinic.src.Requests.ProfileEmployee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_clinic.src.Controllers
{
    [Route("api/employees")]
    [Route("api/profile-employees")]
    [ApiController]
    public class EmployeeController(IProfileEmployeeService service) : ControllerBase
    {
        private string UserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
        private string ClinicId => User.FindFirst("clinicId")?.Value ?? "";

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            ResponseApi<PaginationApi<List<dynamic>>> response = await service.GetAllAsync(new(Request.Query), ClinicId);
            return StatusCode(response.StatusCode, response.Result);
        }

        [Authorize]
        [HttpGet("select")]
        public async Task<IActionResult> GetSelect()
        {
            ResponseApi<List<dynamic>> response = await service.GetSelectAsync(ClinicId);
            return StatusCode(response.StatusCode, response.Result);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            ResponseApi<dynamic?> response = await service.GetByIdAggregateAsync(id);
            return StatusCode(response.StatusCode, response.Result);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProfileEmployeeRequest request)
        {
            if (request == null) return BadRequest("Dados inválidos.");
            request.CreatedBy = UserId;
            if (string.IsNullOrEmpty(request.ClinicId)) request.ClinicId = ClinicId;

            ResponseApi<dynamic?> response = await service.CreateAsync(request);
            return StatusCode(response.StatusCode, response.Result);
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateProfileEmployeeRequest request)
        {
            if (request == null) return BadRequest("Dados inválidos.");
            request.UpdatedBy = UserId;

            ResponseApi<dynamic?> response = await service.UpdateAsync(request);
            return StatusCode(response.StatusCode, response.Result);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            ResponseApi<dynamic> response = await service.DeleteAsync(new() { Id = id, DeletedBy = UserId });
            return StatusCode(response.StatusCode, response.Result);
        }

        [Authorize]
        [HttpPatch("{id}/toggle-block")]
        public async Task<IActionResult> ToggleBlock(string id)
        {
            ResponseApi<dynamic?> response = await service.ToggleBlockAsync(id, UserId);
            return StatusCode(response.StatusCode, response.Result);
        }
    }
}
