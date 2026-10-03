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
    public class PlanRepository(AppDbContext context) : IPlanRepository
    {
        #region CREATE
        public async Task<Plan?> CreateAsync(Plan entity)
        {
            await context.Plans.InsertOneAsync(entity);
            return entity;
        }
        #endregion

        #region READ
        public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Plan> pagination)
        {
            try
            {
                List<BsonDocument> pipeline = new()
                {
                    new("$match", pagination.PipelineFilter),
                    new("$sort", pagination.PipelineSort),
                    new("$skip", pagination.Skip),
                    new("$limit", pagination.Limit),
                    new("$addFields", new BsonDocument
                    {
                        {"id", new BsonDocument("$toString", "$_id")}
                    }),
                    new("$project", new BsonDocument
                    {
                        {"_id", 0}
                    }),
                    new("$sort", pagination.PipelineSort)
                };

                List<BsonDocument> results = await context.Plans.Aggregate<BsonDocument>(pipeline).ToListAsync();
                List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
                return new(list);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }

        public async Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id)
        {
            try
            {
                if (!ObjectId.TryParse(id, out ObjectId objectId))
                    return new(null, 400, "Id inválido");

                List<BsonDocument> pipeline = new()
                {
                    new("$match", new BsonDocument
                    {
                        { "_id", objectId },
                        { "deleted", false }
                    }),
                    new("$addFields", new BsonDocument
                    {
                        {"id", new BsonDocument("$toString", "$_id")}
                    }),
                    new("$project", new BsonDocument
                    {
                        {"_id", 0}
                    })
                };

                BsonDocument? response = await context.Plans.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
                dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
                return new(result, 200, "Plano encontrado");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }

        public async Task<Plan?> GetByIdAsync(string id)
        {
            Plan? entity = await context.Plans.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            return entity;
        }

        public async Task<int> GetCountDocumentsAsync(PaginationUtil<Plan> pagination)
        {
            List<BsonDocument> pipeline = new()
            {
                new("$match", pagination.PipelineFilter)
            };

            List<BsonDocument> results = await context.Plans.Aggregate<BsonDocument>(pipeline).ToListAsync();
            return results.Count;
        }
        #endregion

        #region UPDATE
        public async Task<Plan?> UpdateAsync(Plan entity)
        {
            await context.Plans.ReplaceOneAsync(x => x.Id == entity.Id, entity);
            return entity;
        }
        #endregion

        #region DELETE
        public async Task<Plan> DeleteAsync(Plan entity)
        {
            await context.Plans.ReplaceOneAsync(x => x.Id == entity.Id, entity);
            return entity;
        }
        #endregion
    }
}