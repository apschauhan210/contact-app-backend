using ContactApp_T1.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace ContactApp_T1.DTOs
{
    public class UserWithoutPassword
    {
        public string UserId { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;
        public string Name { get; set; } = String.Empty;
        public string Phone { get; set; } = String.Empty;
        public IList<Contact> Contacts { get; set; } = new List<Contact>();
        public string[] ContactCategories { get; set; } = Array.Empty<string>();

        public UserWithoutPassword(User user)
        {
            this.UserId = user.UserId; 
            this.Email = user.Email; 
            this.Name = user.Name;
            this.Phone = user.Phone;
            this.Contacts = user.Contacts;
            this.ContactCategories = user.ContactCategories;
        }
    }
}
