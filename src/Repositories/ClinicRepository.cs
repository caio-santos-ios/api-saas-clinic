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
    public class ClinicRepository(AppDbContext context) : IClinicRepository
    {
        #region CREATE
        public async Task<Clinic?> CreateAsync(Clinic entity)
        {
            await context.Clinics.InsertOneAsync(entity);
            return entity;
        }
        #endregion

        #region READ
        public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Clinic> pagination)
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
                    new("$lookup", new BsonDocument
                    {
                        {"from", "signatures"},
                        {"let", new BsonDocument("clinicIdStr", "$id")},
                        {"pipeline", new BsonArray
                        {
                            new BsonDocument("$match", new BsonDocument("$expr", new BsonDocument("$and", new BsonArray
                            {
                                new BsonDocument("$eq", new BsonArray{"$clinicId", "$$clinicIdStr"}),
                                new BsonDocument("$eq", new BsonArray{"$deleted", false})
                            }))),
                            new BsonDocument("$sort", new BsonDocument("createdAt", -1)),
                            new BsonDocument("$limit", 1),
                            new BsonDocument("$lookup", new BsonDocument
                            {
                                {"from", "plans"},
                                {"let", new BsonDocument("pId", "$planId")},
                                {"pipeline", new BsonArray
                                {
                                    new BsonDocument("$match", new BsonDocument("$expr", new BsonDocument("$and", new BsonArray
                                    {
                                        new BsonDocument("$eq", new BsonArray{new BsonDocument("$toString", "$_id"), "$$pId"}),
                                        new BsonDocument("$eq", new BsonArray{"$deleted", false})
                                    }))),
                                    new BsonDocument("$project", new BsonDocument{{"name", 1}, {"price", 1}})
                                }},
                                {"as", "plan"}
                            }),
                            new BsonDocument("$unwind", new BsonDocument{{"path", "$plan"}, {"preserveNullAndEmptyArrays", true}})
                        }},
                        {"as", "signature"}
                    }),
                    new("$unwind", new BsonDocument{{"path", "$signature"}, {"preserveNullAndEmptyArrays", true}}),
                    new("$project", new BsonDocument
                    {
                        {"_id", 0}
                    }),
                    new("$sort", pagination.PipelineSort)
                };

                List<BsonDocument> results = await context.Clinics.Aggregate<BsonDocument>(pipeline).ToListAsync();
                List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
                return new(list);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }

        public async Task<ResponseApi<List<dynamic>>> GetSelectAsync(PaginationUtil<Clinic> pagination)
        {
            try
            {
                List<BsonDocument> pipeline = new()
                {
                    new("$match", pagination.PipelineFilter),
                    new("$addFields", new BsonDocument
                    {
                        {"id", new BsonDocument("$toString", "$_id")}
                    }),
                    new("$project", new BsonDocument
                    {
                        {"_id", 0},
                        {"id", 1},
                        {"tradeName", 1},
                        {"corporateName", 1},
                        {"cnpj", 1},
                        {"active", 1}
                    }),
                    new("$sort", new BsonDocument("tradeName", 1))
                };

                List<BsonDocument> results = await context.Clinics.Aggregate<BsonDocument>(pipeline).ToListAsync();
                List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
                return new(list);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }

        public async Task<int> GetCountDocumentsAsync(PaginationUtil<Clinic> pagination)
        {
            List<BsonDocument> pipeline = new()
            {
                new("$match", pagination.PipelineFilter)
            };

            List<BsonDocument> results = await context.Clinics.Aggregate<BsonDocument>(pipeline).ToListAsync();
            return results.Count;
        }

        public async Task<dynamic?> GetByIdAggregateAsync(string id)
        {
            try
            {
                if (!ObjectId.TryParse(id, out ObjectId objectId))
                    return null;

                List<BsonDocument> pipeline = new()
                {
                    new("$match", new BsonDocument
                    {
                        {"_id", objectId},
                        {"deleted", false}
                    }),
                    new("$addFields", new BsonDocument
                    {
                        {"id", new BsonDocument("$toString", "$_id")}
                    }),
                    new("$lookup", new BsonDocument
                    {
                        {"from", "signatures"},
                        {"let", new BsonDocument("clinicIdStr", "$id")},
                        {"pipeline", new BsonArray
                        {
                            new BsonDocument("$match", new BsonDocument("$expr", new BsonDocument("$and", new BsonArray
                            {
                                new BsonDocument("$eq", new BsonArray{"$clinicId", "$$clinicIdStr"}),
                                new BsonDocument("$eq", new BsonArray{"$deleted", false})
                            }))),
                            new BsonDocument("$sort", new BsonDocument("createdAt", -1)),
                            new BsonDocument("$limit", 1),
                            new BsonDocument("$lookup", new BsonDocument
                            {
                                {"from", "plans"},
                                {"let", new BsonDocument("pId", "$planId")},
                                {"pipeline", new BsonArray
                                {
                                    new BsonDocument("$match", new BsonDocument("$expr", new BsonDocument("$and", new BsonArray
                                    {
                                        new BsonDocument("$eq", new BsonArray{new BsonDocument("$toString", "$_id"), "$$pId"}),
                                        new BsonDocument("$eq", new BsonArray{"$deleted", false})
                                    }))),
                                    new BsonDocument("$project", new BsonDocument{{"name", 1}, {"price", 1}, {"limits", 1}, {"features", 1}})
                                }},
                                {"as", "plan"}
                            }),
                            new BsonDocument("$unwind", new BsonDocument{{"path", "$plan"}, {"preserveNullAndEmptyArrays", true}})
                        }},
                        {"as", "signature"}
                    }),
                    new("$unwind", new BsonDocument{{"path", "$signature"}, {"preserveNullAndEmptyArrays", true}}),
                    new("$project", new BsonDocument
                    {
                        {"_id", 0}
                    })
                };

                BsonDocument? response = await context.Clinics.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
                return response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<dynamic>> GetAllAsync(List<BsonDocument> pipeline)
        {
            List<BsonDocument> results = await context.Clinics.Aggregate<BsonDocument>(pipeline).ToListAsync();
            List<dynamic> entities = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
            return entities;
        }

        public async Task<dynamic?> GetByIdAggregateAsync(List<BsonDocument> pipeline)
        {
            BsonDocument? response = await context.Clinics.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
            return result;
        }

        public async Task<Clinic?> GetByIdAsync(string id)
        {
            Clinic? entity = await context.Clinics.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            return entity;
        }

        public async Task<Clinic?> GetByCNPJAsync(string cnpj)
        {
            Clinic? entity = await context.Clinics.Find(x => x.Cnpj == cnpj && !x.Deleted).FirstOrDefaultAsync();
            return entity;
        }

        public async Task<int> GetCountDocumentsAsync(List<BsonDocument> pipeline)
        {
            List<BsonDocument> results = await context.Clinics.Aggregate<BsonDocument>(pipeline).ToListAsync();
            return results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).Count();
        }
        #endregion

        #region UPDATE
        public async Task<Clinic?> UpdateAsync(Clinic entity)
        {
            await context.Clinics.ReplaceOneAsync(x => x.Id == entity.Id, entity);
            return entity;
        }
        #endregion

        #region DELETE
        public async Task<Clinic> DeleteAsync(Clinic entity)
        {
            await context.Clinics.ReplaceOneAsync(x => x.Id == entity.Id, entity);
            return entity;
        }
        #endregion
    }
}