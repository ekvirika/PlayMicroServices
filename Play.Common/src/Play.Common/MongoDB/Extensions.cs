using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Play.Common.Settings;


namespace Play.Common.MongoDB
{
    public static class Extensions
    {
        public static IServiceCollection AddMongo(this IServiceCollection services)
        {
            /* MongoDb Settings for date and Guid */
            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

            /* Settings and database connection config. */
            services.AddSingleton(serviceProvider =>
            {
                var configuration = serviceProvider.GetService<IConfiguration>();
                var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
                MongoDbSettings mongoDbSettings = configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
                MongoClient mongoClient = new MongoClient(mongoDbSettings.ConnectionString);
                return mongoClient.GetDatabase(serviceSettings.ServiceName);
            });

            return services;
        }

        public static IServiceCollection AddMongoRepository<T>(this IServiceCollection services, string collectionName) where T : IEntity
        {
            services.AddSingleton<IRepository<T>>(serviceProvider =>
            {
                var database = serviceProvider.GetService<IMongoDatabase>();
                return new MongoRepository<T>(database, collectionName);
            });
            return services;
        }
    }
}