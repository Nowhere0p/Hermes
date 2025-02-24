using System.Linq.Expressions;
using MongoDB.Driver;

namespace Hermes.DbCore;
public class MongoDbService<T> : IMongoDbService<T>
    where T : IMongoDbRecord
{
    private readonly IMongoCollection<T> _collection;

    public MongoDbService(IMongoClient mongoClient, string databaseName, string collectionName)
    {
        var database = mongoClient.GetDatabase(databaseName);
        _collection = database.GetCollection<T>(collectionName);
    }

    public async Task DeleteAsync(string id)
    {
        var result = await _collection.DeleteOneAsync(Builders<T>.Filter.Eq("_id", id));
    }

    public async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate)
    {
        var filter = Builders<T>.Filter.Where(predicate);
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task<T> GetItemAsync(string id)
    {
        return await _collection.Find(Builders<T>.Filter.Eq("_id", id)).FirstOrDefaultAsync();
    }

    public async Task<bool> SaveAsync(T record)
    {
        await _collection.InsertOneAsync(record);
        return true;
    }

    public async Task<bool> UpdateAsync(string id, T record)
    {
        var result = await _collection.ReplaceOneAsync(
            Builders<T>.Filter.Eq("_id", id),
            record
        );
        return result.ModifiedCount > 0;
    }
}