using api_clinic.src.Models;

namespace api_clinic.src.Helpers
{
    public class MailHelper(HttpClient http)
    {

        private readonly string _apiKey = Environment.GetEnvironmentVariable("RESEND_API_KEY") ?? "";
        private readonly string _fromEmail = Environment.GetEnvironmentVariable("RESEND_EMAIL") ?? "";

        public async Task<string> SendMail(string recipient, string subject, string body)
        {
            try
            {
                var payload = new
                {
                    from = _fromEmail,
                    to = new[] { recipient },
                    subject,
                    html = body
                };

                var req = new HttpRequestMessage(HttpMethod.Post, "https://api.resend.com/emails")
                {
                    Content = JsonContent.Create(payload)
                };
                req.Headers.Authorization = new("Bearer", _apiKey);

                var res = await http.SendAsync(req);
                if (!res.IsSuccessStatusCode)
                    return await res.Content.ReadAsStringAsync();

                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> SendAccountConfirmationMail(string recipient, string name, string code, Clinic? clinic = null)
        {
            string nameClinic = clinic is not null && !string.IsNullOrWhiteSpace(clinic.TradeName) ? clinic.TradeName : "ClinicSaaS";
            string subject = $"Confirmação de Cadastro — {nameClinic}";
            string body = EmailTemplates.GetAccountConfirmationTemplate(name, code, clinic);
            return await SendMail(recipient, subject, body);
        }

        public async Task<string> SendPasswordResetMail(string recipient, string name, string code, Clinic? clinic = null)
        {
            string nameClinic = clinic is not null && !string.IsNullOrWhiteSpace(clinic.TradeName) ? clinic.TradeName : "ClinicSaaS";
            string subject = $"Redefinição de Senha — {nameClinic}";
            string body = EmailTemplates.GetPasswordResetTemplate(name, code, clinic);
            return await SendMail(recipient, subject, body);
        }
    }
}