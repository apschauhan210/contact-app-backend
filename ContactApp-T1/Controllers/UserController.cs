using ContactApp_T1.DTOs;
using ContactApp_T1.Models;
using ContactApp_T1.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;

namespace ContactApp_T1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController: ControllerBase
    {
        private readonly UserService userService;

        public UserController(UserService userService) 
        {
            this.userService = userService;
        }

        [HttpGet("all")]
        public ActionResult<IList<User>> GetAllUsers()
        {
            try
            {
                return Ok(userService.GetAllUsers());
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<UserWithoutPassword> GetUser(string id)
        {
            try
            {
                User user = userService.GetUser(id);
                if(user == null)
                {
                    return NotFound($"User with userId {id} not found");
                }
                return Ok(new UserWithoutPassword(user));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("e/{email}")]
        public ActionResult<User> GetUserByEmail(string email)
        {
            try
            {
                User user = userService.GetUserByEmail(email);
                if(user == null)
                {
                    return NotFound($"User with email {email} not found");
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost]
        public ActionResult<UserWithoutPassword> AddUser([FromBody] User user)
        {
            try
            {
                User createdUser = userService.AddUser(user);
                return CreatedAtAction(nameof(GetUser), new { id = createdUser.UserId }, new UserWithoutPassword(createdUser));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("login")]
        public ActionResult<UserWithoutPassword> LoginUser([FromBody] UserData user)
        {
            try
            {
                User returnedUser = userService.LoginUser(user.Email, user.Password);
                return Ok(new UserWithoutPassword(returnedUser));
            } catch (InvalidCredentialException ex) 
            {
                return Unauthorized(ex.Message);
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPut("{id}")]
        public ActionResult<User> UpdateUser(string id, [FromBody] UserData user)
        {
            try
            {
                return Ok(userService.UpdateUser(id, user));
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPut("add-contact/{userId}")]
        public ActionResult<User> AddContact(string userId, [FromBody] Contact contact)
        {
            try
            {
                return Ok(userService.AddContact(userId, contact));
            } catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPut("update-contact/{userId}")]
        public ActionResult<User> UpdateContact(string userId, [FromBody] Contact contact)
        {
            try
            {
                return Ok(userService.UpdateContact(userId, contact));
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpDelete("remove-contact/{userId}/{contactId}")]
        public ActionResult<User> DeleteContact(string userId, string contactId) 
        {
            try
            {
                return Ok(userService.DeleteContact(userId, contactId));
            } catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("contacts/{userId}")]
        public ActionResult<IEnumerable<Contact>> GetContacts(string userId)
        {
            try
            {
                return Ok(userService.GetContacts(userId));
            } catch (KeyNotFoundException e) 
            { 
                return NotFound(e.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("categories/{userId}")]
        public ActionResult<string[]> GetCategories(string userId)
        {
            try
            {
                return Ok(userService.GetCategories(userId));
            } 
            catch(KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("{userId}/{category}/all")]
        public ActionResult<IEnumerable<Contact>> GetContactsFromCategory(string userId, string category)
        {
            try
            {
                return Ok(userService.GetContactsFromCategory(userId, category));
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPut("add-category/{userId}/{category}")]
        public ActionResult<User> AddCategory(string userId, string category)
        {
            try
            {
                return Ok(userService.AddCategory(userId, category));
            }
            catch(KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch(ArgumentException e)
            {
                return BadRequest(e.Message); 
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPut("add-to-category/{userId}")]
        public ActionResult<Contact> AddContactToCategory(
            string userId, 
            [FromQuery(Name = "contactId")] string contactId,
            [FromQuery(Name = "category")] string category
        ) {
            try
            {
                return Ok(userService.AddContactToCategory(userId, category, contactId));
            } catch(KeyNotFoundException e)
            {
                return NotFound(e.Message);
            } catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPut("add-multiple-to-category/{userId}")]
        public ActionResult AddContactsToCategory(
            string userId,
            [FromBody] string[] contactIds,
            [FromQuery(Name = "category")] string category
        )
        {
            try
            {
                foreach(string  contactId in contactIds)
                {
                    userService.AddContactToCategory(userId, category, contactId);
                }
                return Ok();
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
