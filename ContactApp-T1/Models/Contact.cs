using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace ContactApp_T1.Models
{
    public partial class Contact
    {
        [BsonElement("contactId")]
        public string ContactId { get; set; } = String.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = String.Empty;

        [BsonElement("email")]
        public string? Email { get; set; }

        [BsonElement("phone")]
        public string Phone { get; set; } = String.Empty;

        [BsonElement("company")]
        public string? Company { get; set; }

        [BsonElement("position")]
        public string? Position { get; set; }

        [BsonElement("categories")]
        public string[]? Categories { get; set; }

        //public anytype documents { get; set; } // To store documents

        public Contact UpdateDetails(Contact newContact)
        {
            this.Name = newContact.Name;
            this.Email = newContact.Email;
            this.Phone = newContact.Phone;
            this.Company = newContact.Company;
            this.Position = newContact.Position;

            return this;
        }
    }
}
