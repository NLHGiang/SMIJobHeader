using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using SMIJobXml.Entities.Interfaces;

namespace SMIJobXml.Entities
{
    [BsonIgnoreExtraElements]
    public class MessageLog : IBaseEntity<ObjectId>
    {

        [BsonId(IdGenerator = typeof(ObjectIdGenerator))]
        public ObjectId Id { get; set; }

        [BsonElement("functionName")]
        public string? FunctionName { get; set; }

        [BsonElement("masterKey")]
        public string? MasterKey { get; set; }

        [BsonElement("taxCode")]
        public string? TaxCode { get; set; }

        [BsonElement("errorCode")]
        public int ErrorCode { get; set; }

        [BsonElement("messsage")]
        public string? Messsage { get; set; }

        [BsonElement("bodyRequet")]
        public string? BodyRequet { get; set; }

        [BsonElement("botId")]
        public string? BotId { get; set; }

        [BsonElement("levelLog")]
        public int LevelLog { get; set; }

        [BsonElement("createdAt")]
        public DateTime? CreatedAt { get; set; }

    }
}
