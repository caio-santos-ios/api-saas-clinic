using api_clinic.src.Infraestructure;
using api_clinic.src.Interfaces;
using api_clinic.src.Models;
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
        public async Task<List<dynamic>> GetAllAsync(List<BsonDocument> pipeline)
        {
            List<BsonDocument> results = await context.Plans.Aggregate<BsonDocument>(pipeline).ToListAsync();
            List<dynamic> entities = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
            return entities;
        }
        public async Task<dynamic?> GetByIdAggregateAsync(List<BsonDocument> pipeline)
        {
            BsonDocument? response = await context.Plans.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
            return result;
        }
        public async Task<Plan?> GetByIdAsync(string id)
        {
            Plan? entity = await context.Plans.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            return entity;
        }
        public async Task<int> GetCountDocumentsAsync(List<BsonDocument> pipeline)
        {
            List<BsonDocument> results = await context.Plans.Aggregate<BsonDocument>(pipeline).ToListAsync();
            return results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).Count();
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