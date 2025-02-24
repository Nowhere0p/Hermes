namespace Hermes.DbCore;

public interface IMongoDbRecord
{
    object GetPartitionKey();
}