using api_clinic.src.Requests;

namespace api_clinic.src.Shared.DTOs
{
    public class DeleteDTO : Request
    {
        public string Id {get;set;} = string.Empty;
    }
}