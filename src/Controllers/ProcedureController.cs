using System.Security.Claims;
using api_clinic.src.Interfaces;
using api_clinic.src.Models;
using api_clinic.src.Models.Base;
using api_clinic.src.Requests.Procedure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_clinic.src.Controllers
{
    [Route("api/procedures")]
    [ApiController]
    public class ProcedureController(IProcedureService service) : ControllerBase
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
            ResponseApi<List<dynamic>> response = await service.GetSelectAsync(new(Request.Query), ClinicId);
            return StatusCode(response.StatusCode, response.Result);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            ResponseApi<Procedure?> response = await service.GetByIdAsync(id);
            return StatusCode(response.StatusCode, response.Result);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProcedureRequest request)
        {
            if (request == null) return BadRequest("Dados inválidos.");
            request.CreatedBy = UserId;
            if (string.IsNullOrEmpty(request.ClinicId)) request.ClinicId = ClinicId;

            ResponseApi<Procedure?> response = await service.CreateAsync(request);
            return StatusCode(response.StatusCode, response.Result);
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateProcedureRequest request)
        {
            if (request == null) return BadRequest("Dados inválidos.");
            request.UpdatedBy = UserId;

            ResponseApi<Procedure?> response = await service.UpdateAsync(request);
            return StatusCode(response.StatusCode, response.Result);
        }

        [Authorize]
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> ToggleStatus(string id)
        {
            ResponseApi<Procedure?> response = await service.ToggleActiveAsync(id, UserId);
            return StatusCode(response.StatusCode, response.Result);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            ResponseApi<Procedure> response = await service.DeleteAsync(new() { Id = id, DeletedBy = UserId });
            return StatusCode(response.StatusCode, response.Result);
        }
    }
}
