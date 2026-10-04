using api_clinic.src.Infraestructure;
using api_clinic.src.Interfaces;
using api_clinic.src.Models;
using api_clinic.src.Models.Base;
using api_clinic.src.Shared.Utils;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace api_clinic.src.Repository
{
    public class ProfilePatientRepository(AppDbContext context) : IProfilePatientRepository
    {
        public async Task<ProfilePatient?> CreateAsync(ProfilePatient entity)
        {
            await context.ProfilePatients.InsertOneAsync(entity);
            return entity;
        }

        public async Task<ProfilePatient?> UpdateAsync(ProfilePatient entity)
        {
            await context.ProfilePatients.ReplaceOneAsync(x => x.Id == entity.Id, entity);
            return entity;
        }

        public async Task<ProfilePatient> DeleteAsync(ProfilePatient entity)
        {
            await context.ProfilePatients.ReplaceOneAsync(x => x.Id == entity.Id, entity);
            return entity;
        }

        public async Task<ProfilePatient?> GetByIdAsync(string id)
        {
            return await context.ProfilePatients.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
        }

        public async Task<ProfilePatient?> GetByUserIdAsync(string userId)
        {
            return await context.ProfilePatients.Find(x => x.UserId == userId && !x.Deleted).FirstOrDefaultAsync();
        }

        public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<User> pagination, string clinicId)
        {
            try
            {
                BsonDocument matchDoc = new()
                {
                    { "accessProfile", "patient" },
                    { "deleted", false }
                };

                if (!string.IsNullOrEmpty(clinicId))
                {
                    matchDoc.Add("clinicId", clinicId);
                }

                BsonArray andArray = [matchDoc];
                if (pagination.PipelineFilter != null && pagination.PipelineFilter.ElementCount > 0)
                {
                    andArray.Add(pagination.PipelineFilter);
                }

                List<BsonDocument> pipeline =
                [
                    new("$match", new BsonDocument("$and", andArray)),
                    new("$sort", pagination.PipelineSort),
                    new("$skip", pagination.Skip),
                    new("$limit", pagination.Limit),
                    new("$addFields", new BsonDocument
                    {
                        { "id", new BsonDocument("$toString", "$_id") }
                    }),
                    MongoUtil.Lookup("profile_patients", ["$_id"], ["$userId"], "_profilePatient", [["deleted", false]], 1),
                    new("$addFields", new BsonDocument
                    {
                        { "profile", MongoUtil.First("_profilePatient") }
                    }),
                    new("$project", new BsonDocument
                    {
                        { "_id", 0 },
                        { "id", "$id" },
                        { "name", "$name" },
                        { "email", "$email" },
                        { "phone", "$phone" },
                        { "photo", "$photo" },
                        { "blocked", "$blocked" },
                        { "clinicId", "$clinicId" },
                        { "createdAt", "$createdAt" },
                        { "cpf", MongoUtil.ValidateNull("profile.cpf", "") },
                        { "rg", MongoUtil.ValidateNull("profile.rg", "") },
                        { "birthDate", "$profile.birthDate" },
                        { "gender", MongoUtil.ValidateNull("profile.gender", "") },
                        { "bloodType", MongoUtil.ValidateNull("profile.bloodType", "") },
                        { "allergies", MongoUtil.ValidateNull("profile.allergies", new BsonArray()) },
                        { "emergencyContactName", MongoUtil.ValidateNull("profile.emergencyContactName", "") },
                        { "emergencyContactPhone", MongoUtil.ValidateNull("profile.emergencyContactPhone", "") },
                        { "emergencyContactRelationship", MongoUtil.ValidateNull("profile.emergencyContactRelationship", "") },
                        { "address", "$profile.address" },
                        { "notes", MongoUtil.ValidateNull("profile.notes", "") },
                        { "profilePatientId", MongoUtil.ToString("$profile._id") }
                    })
                ];

                List<BsonDocument> results = await context.Users.Aggregate<BsonDocument>(pipeline).ToListAsync();
                List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
                return new(list);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }

        public async Task<ResponseApi<List<dynamic>>> GetSelectAsync(string clinicId)
        {
            try
            {
                BsonDocument matchDoc = new()
                {
                    { "accessProfile", "patient" },
                    { "deleted", false },
                    { "blocked", false }
                };

                if (!string.IsNullOrEmpty(clinicId))
                {
                    matchDoc.Add("clinicId", clinicId);
                }

                List<BsonDocument> pipeline =
                [
                    new("$match", matchDoc),
                    new("$addFields", new BsonDocument
                    {
                        { "id", new BsonDocument("$toString", "$_id") }
                    }),
                    MongoUtil.Lookup("profile_patients", ["$_id"], ["$userId"], "_profilePatient", [["deleted", false]], 1),
                    new("$addFields", new BsonDocument
                    {
                        { "profile", MongoUtil.First("_profilePatient") }
                    }),
                    new("$project", new BsonDocument
                    {
                        { "_id", 0 },
                        { "id", "$id" },
                        { "name", "$name" },
                        { "email", "$email" },
                        { "phone", "$phone" },
                        { "cpf", MongoUtil.ValidateNull("profile.cpf", "") }
                    }),
                    new("$sort", new BsonDocument("name", 1))
                ];

                List<BsonDocument> results = await context.Users.Aggregate<BsonDocument>(pipeline).ToListAsync();
                List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
                return new(list);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }

        public async Task<int> GetCountDocumentsAsync(PaginationUtil<User> pagination, string clinicId)
        {
            BsonDocument matchDoc = new()
            {
                { "accessProfile", "patient" },
                { "deleted", false }
            };

            if (!string.IsNullOrEmpty(clinicId))
            {
                matchDoc.Add("clinicId", clinicId);
            }

            BsonArray andArray = [matchDoc];
            if (pagination.PipelineFilter != null && pagination.PipelineFilter.ElementCount > 0)
            {
                andArray.Add(pagination.PipelineFilter);
            }

            List<BsonDocument> pipeline =
            [
                new("$match", new BsonDocument("$and", andArray))
            ];

            List<BsonDocument> results = await context.Users.Aggregate<BsonDocument>(pipeline).ToListAsync();
            return results.Count;
        }

        public async Task<dynamic?> GetByIdAggregateAsync(string id)
        {
            if (!ObjectId.TryParse(id, out ObjectId objectId)) return null;

            List<BsonDocument> pipeline =
            [
                new("$match", new BsonDocument
                {
                    { "_id", objectId },
                    { "deleted", false }
                }),
                new("$addFields", new BsonDocument
                {
                    { "id", new BsonDocument("$toString", "$_id") }
                }),
                MongoUtil.Lookup("profile_patients", ["$_id"], ["$userId"], "_profilePatient", [["deleted", false]], 1),
                new("$addFields", new BsonDocument
                {
                    { "profile", MongoUtil.First("_profilePatient") }
                }),
                new("$project", new BsonDocument
                {
                    { "_id", 0 },
                    { "id", "$id" },
                    { "name", "$name" },
                    { "email", "$email" },
                    { "phone", "$phone" },
                    { "photo", "$photo" },
                    { "blocked", "$blocked" },
                    { "clinicId", "$clinicId" },
                    { "createdAt", "$createdAt" },
                    { "cpf", MongoUtil.ValidateNull("profile.cpf", "") },
                    { "rg", MongoUtil.ValidateNull("profile.rg", "") },
                    { "birthDate", "$profile.birthDate" },
                    { "gender", MongoUtil.ValidateNull("profile.gender", "") },
                    { "bloodType", MongoUtil.ValidateNull("profile.bloodType", "") },
                    { "allergies", MongoUtil.ValidateNull("profile.allergies", new BsonArray()) },
                    { "emergencyContactName", MongoUtil.ValidateNull("profile.emergencyContactName", "") },
                    { "emergencyContactPhone", MongoUtil.ValidateNull("profile.emergencyContactPhone", "") },
                    { "emergencyContactRelationship", MongoUtil.ValidateNull("profile.emergencyContactRelationship", "") },
                    { "address", "$profile.address" },
                    { "notes", MongoUtil.ValidateNull("profile.notes", "") },
                    { "profilePatientId", MongoUtil.ToString("$profile._id") }
                })
            ];

            BsonDocument? response = await context.Users.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            return response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
        }
    }
}
