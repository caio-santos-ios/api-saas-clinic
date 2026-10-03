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
    public class SignatureRepository(AppDbContext context) : ISignatureRepository
    {
        #region CREATE
        public async Task<Signature?> CreateAsync(Signature entity)
        {
            await context.Signatures.InsertOneAsync(entity);
            return entity;
        }
        #endregion

        #region READ
        public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Signature> pagination)
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
                        {"id", new BsonDocument("$toString", "$_id")},
                        {"clinicObjId", new BsonDocument("$toObjectId", "$clinicId")},
                        {"planObjId", new BsonDocument("$toObjectId", "$planId")}
                    }),

                    MongoUtil.Lookup("clinics", ["$clinicObjId"], ["$_id"], "_clinics", [["deleted", false]], 1),
                    MongoUtil.Lookup("plans", ["$planObjId"], ["$_id"], "_plans", [["deleted", false]], 1),

                    new("$addFields", new BsonDocument
                    {
                        {"clinic", MongoUtil.First("_clinics")},
                        {"plan", MongoUtil.First("_plans")}
                    }),

                    new("$project", new BsonDocument
                    {
                        {"_id", 0},
                        {"_clinics", 0},
                        {"_plans", 0},
                        {"clinicObjId", 0},
                        {"planObjId", 0}
                    }),
                    new("$sort", pagination.PipelineSort)
                };

                List<BsonDocument> results = await context.Signatures.Aggregate<BsonDocument>(pipeline).ToListAsync();
                List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
                return new(list);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }

        public async Task<dynamic?> GetByIdAggregateAsync(List<BsonDocument> pipeline)
        {
            BsonDocument? response = await context.Signatures.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            dynamic? entity = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
            return entity;
        }

        public async Task<Signature?> GetByIdAsync(string id)
        {
            Signature? entity = await context.Signatures.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            return entity;
        }

        public async Task<Signature?> GetByClinicIdAsync(string clinicId)
        {
            Signature? entity = await context.Signatures.Find(x => x.ClinicId == clinicId && !x.Deleted).SortByDescending(x => x.CreatedAt).FirstOrDefaultAsync();
            return entity;
        }

        public async Task<Signature?> GetByAsaasSubscriptionIdAsync(string asaasSubscriptionId)
        {
            Signature? entity = await context.Signatures.Find(x => x.AsaasSubscriptionId == asaasSubscriptionId && !x.Deleted).FirstOrDefaultAsync();
            return entity;
        }

        public async Task<Signature?> GetByAsaasCustomerIdAsync(string asaasCustomerId)
        {
            Signature? entity = await context.Signatures.Find(x => x.AsaasCustomerId == asaasCustomerId && !x.Deleted).SortByDescending(x => x.CreatedAt).FirstOrDefaultAsync();
            return entity;
        }

        public async Task<int> GetCountDocumentsAsync(PaginationUtil<Signature> pagination)
        {
            List<BsonDocument> pipeline = new()
            {
                new("$match", pagination.PipelineFilter)
            };

            List<BsonDocument> results = await context.Signatures.Aggregate<BsonDocument>(pipeline).ToListAsync();
            return results.Count;
        }
        #endregion

        #region UPDATE
        public async Task<Signature?> UpdateAsync(Signature entity)
        {
            await context.Signatures.ReplaceOneAsync(x => x.Id == entity.Id, entity);
            return entity;
        }
        #endregion

        #region DELETE
        public async Task<Signature> DeleteAsync(Signature entity)
        {
            await context.Signatures.ReplaceOneAsync(x => x.Id == entity.Id, entity);
            return entity;
        }
        #endregion
    }
}
