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
    public class ProcedureRepository(AppDbContext context) : IProcedureRepository
    {
        public async Task<Procedure?> CreateAsync(Procedure entity)
        {
            await context.Procedures.InsertOneAsync(entity);
            return entity;
        }

        public async Task<Procedure?> UpdateAsync(Procedure entity)
        {
            await context.Procedures.ReplaceOneAsync(x => x.Id == entity.Id, entity);
            return entity;
        }

        public async Task<Procedure> DeleteAsync(Procedure entity)
        {
            await context.Procedures.ReplaceOneAsync(x => x.Id == entity.Id, entity);
            return entity;
        }

        public async Task<Procedure?> GetByIdAsync(string id)
        {
            return await context.Procedures.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
        }

        public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Procedure> pagination, string clinicId)
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
                    new("$project", new BsonDocument
                    {
                        { "_id", 0 },
                        { "id", 1 },
                        { "name", 1 },
                        { "description", 1 },
                        { "code", 1 },
                        { "durationMinutes", 1 },
                        { "price", 1 },
                        { "active", 1 },
                        { "clinicId", 1 },
                        { "createdAt", 1 }
                    })
                ];

                List<BsonDocument> results = await context.Procedures.Aggregate<BsonDocument>(pipeline).ToListAsync();
                List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
                return new(list);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }

        public async Task<ResponseApi<List<dynamic>>> GetSelectAsync(PaginationUtil<Procedure> pagination, string clinicId)
        {
            try
            {
                BsonDocument matchDoc = new()
                {
                    { "deleted", false },
                    { "active", true }
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
                    new("$project", new BsonDocument
                    {
                        { "_id", 0 },
                        { "id", 1 },
                        { "name", 1 },
                        { "code", 1 },
                        { "durationMinutes", 1 },
                        { "price", 1 }
                    }),
                    new("$sort", new BsonDocument("name", 1))
                ];

                List<BsonDocument> results = await context.Procedures.Aggregate<BsonDocument>(pipeline).ToListAsync();
                List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
                return new(list);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }

        public async Task<int> GetCountDocumentsAsync(PaginationUtil<Procedure> pagination, string clinicId)
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

            List<BsonDocument> results = await context.Procedures.Aggregate<BsonDocument>(pipeline).ToListAsync();
            return results.Count;
        }
    }
}
