using System.Security.Claims;
using api_clinic.src.Interfaces;
using api_clinic.src.Models.Base;
using api_clinic.src.Requests.Appointment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_clinic.src.Controllers
{
    [Route("api/appointments")]
    [ApiController]
    public class AppointmentController(IAppointmentService service) : ControllerBase
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
        [HttpGet("my-appointments")]
        public async Task<IActionResult> GetMyAppointments()
        {
            var query = Request.Query.ToDictionary(q => q.Key, q => q.Value.ToString());
            query["doctorId"] = UserId;
            ResponseApi<PaginationApi<List<dynamic>>> response = await service.GetAllAsync(new(query), ClinicId);
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
        public async Task<IActionResult> Create([FromBody] CreateAppointmentRequest request)
        {
            if (request == null) return BadRequest("Dados inválidos.");
            request.CreatedBy = UserId;
            if (string.IsNullOrEmpty(request.ClinicId)) request.ClinicId = ClinicId;

            ResponseApi<dynamic?> response = await service.CreateAsync(request);
            return StatusCode(response.StatusCode, response.Result);
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateAppointmentRequest request)
        {
            if (request == null) return BadRequest("Dados inválidos.");
            request.UpdatedBy = UserId;

            ResponseApi<dynamic?> response = await service.UpdateAsync(request);
            return StatusCode(response.StatusCode, response.Result);
        }

        [Authorize]
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(string id, [FromBody] UpdateAppointmentStatusRequest request)
        {
            if (request == null) return BadRequest("Dados inválidos.");
            request.Id = id;
            request.UpdatedBy = UserId;

            ResponseApi<dynamic?> response = await service.UpdateStatusAsync(request);
            return StatusCode(response.StatusCode, response.Result);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            ResponseApi<dynamic> response = await service.DeleteAsync(new() { Id = id, DeletedBy = UserId });
            return StatusCode(response.StatusCode, response.Result);
        }
    }
}
