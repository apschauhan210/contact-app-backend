using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using ContactApp_T1.DTOs;

namespace ContactApp_T1.Models
{
    [BsonIgnoreExtraElements]
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; } = String.Empty;

        [BsonElement("email"), BsonRequired]
        public string Email { get; set; } = String.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = String.Empty;

        [BsonElement("password")]
        public string Password { get; set; } = String.Empty;

        [BsonElement("phone")]
        public string Phone { get; set; } = String.Empty;

        [BsonElement("contacts")]
        public IList<Contact> Contacts { get; set; } = new List<Contact>();

        [BsonElement("conatctCategories")]
        public string[] ContactCategories { get; set; } = Array.Empty<string>();

        public User EditUser(UserData userData)
        {
            this.Email = userData.Email;
            this.Name = userData.Name;
            this.Password = userData.Password; 
            this.Phone = userData.Phone;

            return this;
        }
    }
}
