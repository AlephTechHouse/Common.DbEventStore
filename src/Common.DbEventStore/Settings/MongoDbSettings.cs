namespace Common.DbEventStore.Settings.MongoDb
{
    public class MongoDbSettings
    {
        public string? Host { get; set; }
        public int Port { get; init; }
        public string ConnectionString
        {
            get
            {
                return $"mongodb://{Host}:{Port}";
            }
        }
    }
}