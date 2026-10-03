using MongoDB.Driver;
using api_clinic.src.Models;

namespace api_clinic.src.Infraestructure
{
    public class AppDbContext
    {
        public static string? ConnectionString { get; set; }
        public static string? DatabaseName { get; set; }
        public static bool IsSSL { get; set; }
        private IMongoDatabase Database { get; }

        public AppDbContext()
        {
            try
            {
                MongoClientSettings mongoClientSettings = MongoClientSettings.FromUrl(new MongoUrl(ConnectionString));
                if (IsSSL)
                {
                    mongoClientSettings.SslSettings = new SslSettings
                    {
                        EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12
                    };
                }

                var mongoClient = new MongoClient(mongoClientSettings);
                Database = mongoClient.GetDatabase(DatabaseName);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to connect to database. Error: {ex.Message}");
            }
        }

        public IMongoCollection<User> Users => Database.GetCollection<User>("users");
        public IMongoCollection<Attachment> Attachments => Database.GetCollection<Attachment>("attachments");
        public IMongoCollection<Plan> Plans => Database.GetCollection<Plan>("plans");
        public IMongoCollection<Clinic> Clinics => Database.GetCollection<Clinic>("clinics");
        public IMongoCollection<Signature> Signatures => Database.GetCollection<Signature>("signatures");
    }
}

