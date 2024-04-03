using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Common.DbEventStore.Settings.MongoDb;
using Common.DbEventStore.Settings.Service;
using Microsoft.Extensions.Logging;
using Common.DbEventStore.Helpers;

namespace Common.DbEventStore.MongoDB;

public static class Extensions
{
    private const string ServiceSettingsSectionName = nameof(ServiceSettings);
    private const string MongoDbSettingsSectionName = nameof(MongoDbSettings);

    public static IServiceCollection AddMongo(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var serviceSettings = configuration.GetConfiguration<ServiceSettings>(ServiceSettingsSectionName);
        var mongoDbSettings = configuration.GetConfiguration<MongoDbSettings>(MongoDbSettingsSectionName);

        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
        BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

        services.AddSingleton(serviceProvider =>
        {
            var mongoClient = new MongoClient(mongoDbSettings.ConnectionString);
            return mongoClient.GetDatabase(serviceSettings.ServiceName);
        });

        return services;
    }

    public static IServiceCollection AddMongoRepository<T>(
        this IServiceCollection services,
        string collectionName
    ) where T : IEntity
    {
        services.AddSingleton<IRepository<T>>(serviceProvider =>
        {
            var database = serviceProvider.GetRequiredService<IMongoDatabase>();
            var logger = serviceProvider.GetRequiredService<ILogger<MongoRepository<T>>>();
            return new MongoRepository<T>(database, collectionName, logger);
        });

        return services;
    }
}
