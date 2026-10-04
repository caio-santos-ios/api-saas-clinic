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
    public class AppointmentRepository(AppDbContext context) : IAppointmentRepository
    {
        public async Task<Appointment?> CreateAsync(Appointment entity)
        {
            await context.Appointments.InsertOneAsync(entity);
            return entity;
        }

        public async Task<Appointment?> UpdateAsync(Appointment entity)
        {
            await context.Appointments.ReplaceOneAsync(x => x.Id == entity.Id, entity);
            return entity;
        }

        public async Task<Appointment> DeleteAsync(Appointment entity)
        {
            await context.Appointments.ReplaceOneAsync(x => x.Id == entity.Id, entity);
            return entity;
        }

        public async Task<Appointment?> GetByIdAsync(string id)
        {
            return await context.Appointments.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
        }

        public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Appointment> pagination, string clinicId)
        {
            try
            {
                BsonDocument matchDoc = new()
                {
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
                    MongoUtil.Lookup("users", ["$patientId"], ["$_id"], "_patient", [["deleted", false]], 1),
                    new("$addFields", new BsonDocument
                    {
                        { "patient", MongoUtil.First("_patient") }
                    }),
                    MongoUtil.Lookup("users", ["$doctorId"], ["$_id"], "_doctor", [["deleted", false]], 1),
                    new("$addFields", new BsonDocument
                    {
                        { "doctor", MongoUtil.First("_doctor") }
                    }),
                    MongoUtil.Lookup("procedures", ["$procedureId"], ["$_id"], "_procedure", [["deleted", false]], 1),
                    new("$addFields", new BsonDocument
                    {
                        { "procedure", MongoUtil.First("_procedure") }
                    }),
                    new("$project", new BsonDocument
                    {
                        { "_id", 0 },
                        { "id", "$id" },
                        { "patientId", "$patientId" },
                        { "patientName", MongoUtil.ValidateNull("patient.name", "") },
                        { "patientPhone", MongoUtil.ValidateNull("patient.phone", "") },
                        { "patientEmail", MongoUtil.ValidateNull("patient.email", "") },
                        { "doctorId", "$doctorId" },
                        { "doctorName", MongoUtil.ValidateNull("doctor.name", "") },
                        { "procedureId", "$procedureId" },
                        { "procedureName", MongoUtil.ValidateNull("procedure.name", "") },
                        { "date", "$date" },
                        { "startTime", "$startTime" },
                        { "endTime", "$endTime" },
                        { "status", "$status" },
                        { "notes", MongoUtil.ValidateNull("notes", "") },
                        { "price", MongoUtil.ValidateNull("price", 0) },
                        { "clinicId", "$clinicId" },
                        { "createdAt", "$createdAt" }
                    })
                ];

                List<BsonDocument> results = await context.Appointments.Aggregate<BsonDocument>(pipeline).ToListAsync();
                List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
                return new(list);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }

        public async Task<int> GetCountDocumentsAsync(PaginationUtil<Appointment> pagination, string clinicId)
        {
            BsonDocument matchDoc = new()
            {
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

            List<BsonDocument> results = await context.Appointments.Aggregate<BsonDocument>(pipeline).ToListAsync();
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
                MongoUtil.Lookup("users", ["$patientId"], ["$_id"], "_patient", [["deleted", false]], 1),
                new("$addFields", new BsonDocument
                {
                    { "patient", MongoUtil.First("_patient") }
                }),
                MongoUtil.Lookup("users", ["$doctorId"], ["$_id"], "_doctor", [["deleted", false]], 1),
                new("$addFields", new BsonDocument
                {
                    { "doctor", MongoUtil.First("_doctor") }
                }),
                MongoUtil.Lookup("procedures", ["$procedureId"], ["$_id"], "_procedure", [["deleted", false]], 1),
                new("$addFields", new BsonDocument
                {
                    { "procedure", MongoUtil.First("_procedure") }
                }),
                new("$project", new BsonDocument
                {
                    { "_id", 0 },
                    { "id", "$id" },
                    { "patientId", "$patientId" },
                    { "patientName", MongoUtil.ValidateNull("patient.name", "") },
                    { "patientPhone", MongoUtil.ValidateNull("patient.phone", "") },
                    { "patientEmail", MongoUtil.ValidateNull("patient.email", "") },
                    { "doctorId", "$doctorId" },
                    { "doctorName", MongoUtil.ValidateNull("doctor.name", "") },
                    { "procedureId", "$procedureId" },
                    { "procedureName", MongoUtil.ValidateNull("procedure.name", "") },
                    { "date", "$date" },
                    { "startTime", "$startTime" },
                    { "endTime", "$endTime" },
                    { "status", "$status" },
                    { "notes", MongoUtil.ValidateNull("notes", "") },
                    { "price", MongoUtil.ValidateNull("price", 0) },
                    { "clinicId", "$clinicId" },
                    { "createdAt", "$createdAt" }
                })
            ];

            BsonDocument? response = await context.Appointments.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            return response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
        }
    }
}
