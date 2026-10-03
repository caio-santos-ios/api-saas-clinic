using System.Security.Claims;
using api_clinic.src.Interfaces;
using api_clinic.src.Models;
using api_clinic.src.Models.Base;
using api_clinic.src.Requests.Signature;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_clinic.src.Controllers
{
    [Route("api/signatures")]
    [ApiController]
    public class SignatureController(ISignatureService service) : ControllerBase
    {
        private string UserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            ResponseApi<PaginationApi<List<dynamic>>> response = await service.GetAllAsync(new(Request.Query));
            return StatusCode(response.StatusCode, response.Result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            ResponseApi<dynamic?> response = await service.GetByIdAggregateAsync(id);
            return StatusCode(response.StatusCode, response.Result);
        }

        [Authorize]
        [HttpGet("clinic/{clinicId}")]
        public async Task<IActionResult> GetByClinicIdAsync(string clinicId)
        {
            ResponseApi<dynamic?> response = await service.GetByClinicIdAsync(clinicId);
            return StatusCode(response.StatusCode, response.Result);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSignatureRequest request)
        {
            if (request == null) return BadRequest("Dados inválidos.");
            request.CreatedBy = UserId;
            ResponseApi<Signature?> response = await service.CreateAsync(request);
            return StatusCode(response.StatusCode, response.Result);
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> WebHook([FromBody] AsaasWebhookRequest request)
        {
            var token = Request.Headers["asaas-access-token"].ToString();
            if (token != Environment.GetEnvironmentVariable("ASAAS_WEBHOOK_TOKEN")) return Unauthorized();

            if (request == null) return BadRequest("Dados inválidos.");
            ResponseApi<dynamic?> response = await service.ProcessWebhookAsync(request);
            return StatusCode(response.StatusCode, response.Result);
        }

        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe([FromBody] SubscribePlanRequest request)
        {
            if (request == null) return BadRequest("Dados inválidos.");
            ResponseApi<dynamic?> response = await service.SubscribeAsync(request);
            return StatusCode(response.StatusCode, response.Result);
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateSignatureRequest request)
        {
            if (request == null) return BadRequest("Dados inválidos.");
            request.UpdatedBy = UserId;
            ResponseApi<Signature?> response = await service.UpdateAsync(request);
            return StatusCode(response.StatusCode, response.Result);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            ResponseApi<Signature> response = await service.DeleteAsync(new() { Id = id, DeletedBy = UserId });
            return StatusCode(response.StatusCode, response.Result);
        }
    }
}
