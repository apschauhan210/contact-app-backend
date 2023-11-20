using ContactApp_T1.DTOs;
using ContactApp_T1.Models;
using MongoDB.Driver;
using System;
using System.Security.Authentication;

namespace ContactApp_T1.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        [Obsolete]
        public UserService(IDatabaseSettings settings, IMongoClient mongoClient)
        {
            IMongoDatabase database = mongoClient.GetDatabase(settings.DatabaseName);
            _users = database.GetCollection<User>(settings.UsersCollectionName);
            CreateIndexOptions options = new() { Unique = true };
            var field = new StringFieldDefinition<User>("Email");
            var indexDefinition = new IndexKeysDefinitionBuilder<User>().Ascending(field);
            _users.Indexes.CreateOneAsync(indexDefinition, options);
        }

        public User AddContact(string userId, Contact contact)
        {
            User user = _users.Find(user => user.UserId == userId).FirstOrDefault() ?? throw new KeyNotFoundException($"User with userId {userId} not found");
            contact.ContactId = System.Guid.NewGuid().ToString();

            user.Contacts.Add(contact);
            _users.ReplaceOne(user => user.UserId == userId, user);

            return user;
        }

        public User UpdateContact(string userId, Contact updatedContact)
        {
            User user = _users.Find(user => user.UserId == userId).FirstOrDefault() ?? throw new KeyNotFoundException($"User with userId {userId} not found");
            bool found = false;

            foreach (Contact contact in user.Contacts)
            {
                if (contact.ContactId == updatedContact.ContactId)
                {
                    contact.UpdateDetails(updatedContact);
                    found = true;
                    break;
                }
            }

            if(!found)
            {
                throw new KeyNotFoundException($"Contact with contactId {updatedContact.ContactId} for user with userId {userId} do not exist");
            }

            _users.ReplaceOne(user => user.UserId == userId, user);

            return user;
        }

        public User DeleteContact(string userId, string contactId) 
        {
            User user = _users.Find(user => user.UserId == userId).FirstOrDefault() ?? throw new KeyNotFoundException($"User with userId {userId} not found");
            bool found = false;

            foreach(Contact contact in user.Contacts)
            {
                if(contact.ContactId ==  contactId)
                {
                    user.Contacts.Remove(contact);
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                throw new KeyNotFoundException($"Contact with contactId {contactId} for user with userId {userId} do not exist");
            }

            _users.ReplaceOne(user => user.UserId == userId, user);

            return user;
        }

        public User AddUser(User user)
        {
            foreach(Contact contact in user.Contacts)
            {
                contact.ContactId = System.Guid.NewGuid().ToString();
            }

            user.ContactCategories = GetPredefinedCategories();

            _users.InsertOne(user);
            return _users.Find(dbUser => dbUser.Email == user.Email).FirstOrDefault();
        }

        public User LoginUser(string email, string password)
        {
            User user =  _users.Find(dbUser => dbUser.Email == email).FirstOrDefault() ?? throw new KeyNotFoundException($"{email} is not registered");

            if(user.Password == password)
            {
                return user;
            } else
            {
                throw new InvalidCredentialException("Invalid credentials. Please double check.");
            }
        }

        public User DeleteUser(string id)
        {
            return _users.FindOneAndDelete(user => user.UserId == id);
        }

        public IEnumerable<Contact> GetContacts(string userId)
        {
            User user = _users.Find(user => user.UserId == userId).FirstOrDefault() ?? throw new KeyNotFoundException($"User with id {userId} not found");
            return user.Contacts;
        }

        public User GetUser(string id)
        {
            return _users.Find(dbUser => dbUser.UserId == id).FirstOrDefault();
        }

        public User GetUserByEmail(string email)
        {
            return _users.Find(dbUser => dbUser.Email == email).FirstOrDefault();
        }

        public List<User> GetAllUsers()
        {
            return _users.Find(user => true).ToList();
        }

        public User UpdateUser(string id, UserData userData)
        {
            User dbUser = _users.Find(user => user.UserId == id).FirstOrDefault() ?? throw new KeyNotFoundException($"User with userId {id} not found");
            dbUser.EditUser(userData);
            _users.ReplaceOne(user => user.UserId == id, dbUser);
            
            return dbUser;
        }

        public string[] GetPredefinedCategories()
        {
            string[] arr = { "home", "work", "family", "friends" };
            return arr;
        }

        public string[] GetCategories(string userId)
        {
            User dbUser = _users.Find(user => user.UserId == userId).FirstOrDefault() ?? throw new KeyNotFoundException($"User with userId {userId} not found");
            return dbUser.ContactCategories;
        }

        public IEnumerable<Contact> GetContactsFromCategory(string userId, string category)
        {
            category = category.ToLower();
            User dbUser = _users.Find(user => user.UserId == userId).FirstOrDefault() ?? throw new KeyNotFoundException($"User with userId {userId} not found");

            if (!dbUser.ContactCategories.Contains(category))
            {
                throw new KeyNotFoundException($"Category {category} does not exists");
            }

            IList<Contact> contacts = new List<Contact>();

            foreach (Contact contact in dbUser.Contacts) 
            {
                if(contact.Categories != null && contact.Categories.Contains(category))
                {
                    contacts.Add( contact );
                }
            }

            return contacts;
        }

        public User AddCategory(string userId, string category)
        {
            category = category.ToLower();
            User dbUser = _users.Find(user => user.UserId == userId).FirstOrDefault() ?? throw new KeyNotFoundException($"User with userId {userId} not found");
            ISet<string> categorySet = new HashSet<string>(dbUser.ContactCategories);

            if(categorySet.Contains(category))
            {
                throw new ArgumentException($"Category {category} already exists");
            }

            categorySet.Add(category);

            dbUser.ContactCategories = categorySet.ToArray();

            _users.ReplaceOne(user => user.UserId == userId, dbUser);

            return dbUser;
        }

        public Contact AddContactToCategory(string userId, string category, string contactId)
        {
            category = category.ToLower();

            User user = _users.Find(user => user.UserId == userId).FirstOrDefault()
                ?? throw new KeyNotFoundException($"User with userId {userId} not found");

            ISet<string> categorySet = new HashSet<string>(user.ContactCategories);

            if(!categorySet.Contains(category))
            {
                throw new KeyNotFoundException($"Category {category} doesn't exist. Please create it first.");
            }

            bool found = false;
            Contact? resContact = null;

            foreach (Contact contact in user.Contacts)
            {
                if (contact.ContactId == contactId)
                {
                    contact.Categories ??= Array.Empty<string>();

                    ISet<string> contactCategories = new HashSet<string>(contact.Categories)
                    {
                        category
                    };

                    contact.Categories = contactCategories.ToArray();

                    resContact = contact;

                    found = true;
                    break;
                }
            }

            if (!found)
            {
                throw new KeyNotFoundException($"Contact with contactId {contactId} for user with userId {userId} do not exist");
            }

            _users.ReplaceOne(user => user.UserId == userId, user);

            return resContact;
        }
    }
}
