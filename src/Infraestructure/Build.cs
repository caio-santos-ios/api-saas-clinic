using api_clinic.src.Interfaces;
using api_clinic.src.Repository;
using api_clinic.src.Services;
using api_clinic.src.Infraestructure;
using api_clinic.src.Helpers;
using api_clinic.src.Handlers;

namespace api_clinic.src.Configuration
{
    public static class Build
    {
        public static void AddBuilderConfiguration(this WebApplicationBuilder builder)
        {
            AppDbContext.ConnectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? "";
            AppDbContext.DatabaseName = Environment.GetEnvironmentVariable("DATABASE_NAME") ?? "";
            AppDbContext.IsSSL = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("IS_SSL"))
                && Convert.ToBoolean(Environment.GetEnvironmentVariable("IS_SSL"));
        }

        public static void AddContext(this WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<AppDbContext>();
        }

        public static void AddBuilderHelpers(this WebApplicationBuilder builder)
        {
            builder.Services.AddHttpClient<MailHelper>();
            builder.Services.AddTransient<UploadHelper>();
        }

        public static void AddBuilderServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddTransient<IAuthService, AuthService>();
            
            builder.Services.AddTransient<IUserService, UserService>();
            builder.Services.AddTransient<IUserRepository, UserRepository>();
            
            builder.Services.AddTransient<IDashboardService, DashboardService>();
            builder.Services.AddTransient<IDashboardRepository, DashboardRepository>();
            
            builder.Services.AddTransient<IAttachmentService, AttachmentService>();
            builder.Services.AddTransient<IAttachmentRepository, AttachmentRepository>();
            
            builder.Services.AddTransient<IPlanService, PlanService>();
            builder.Services.AddTransient<IPlanRepository, PlanRepository>();

            builder.Services.AddTransient<IClinicService, ClinicService>();
            builder.Services.AddTransient<IClinicRepository, ClinicRepository>();

            builder.Services.AddTransient<ISignatureService, SignatureService>();
            builder.Services.AddTransient<ISignatureRepository, SignatureRepository>();

            builder.Services.AddTransient<IProfileDoctorService, ProfileDoctorService>();
            builder.Services.AddTransient<IProfileDoctorRepository, ProfileDoctorRepository>();

            builder.Services.AddTransient<IProcedureService, ProcedureService>();
            builder.Services.AddTransient<IProcedureRepository, ProcedureRepository>();

            builder.Services.AddTransient<IProfileEmployeeService, ProfileEmployeeService>();
            builder.Services.AddTransient<IProfileEmployeeRepository, ProfileEmployeeRepository>();

            builder.Services.AddTransient<IProfilePatientService, ProfilePatientService>();
            builder.Services.AddTransient<IProfilePatientRepository, ProfilePatientRepository>();

            builder.Services.AddSingleton<AsaasHandler>();
            builder.Services.AddHttpClient<UploadHelper>();
        }
    }
}