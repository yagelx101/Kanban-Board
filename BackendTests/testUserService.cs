using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.ServiceLayer;
using IntroSE.Kanban.Backend.ServiceLayers;

namespace BackendTests
{
    public class testUserService
    {

        private ServiceFactory factory;
        private UserService service;
        public testUserService()
        {
            factory = new ServiceFactory();
            service = factory.userS;
        }
        /// <summary>
        /// This function tests Requirement 2.2.1 - User Registration
        /// </summary>
        public void Register_InvalidPassword_TooShort()
        {
            var email = "testuser@example.com";
            var password = "Ab1";
            var expected = "Password must be 6-20 characters long, contain at least one uppercase letter, one lowercase letter, and one number";

            var result = service.Register(email, password);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            var errorMessage = resultJson.GetProperty("ErrorMessage").GetString();


            if (errorMessage == expected)
            {
                Console.WriteLine(" Passed: Received the expected error message.");
            }
            else
            {
                Console.WriteLine($"Failed: Expected '{expected}', but got '{errorMessage}'.");
            }
        }
        /// <summary>
        /// This function tests Requirement 2.2.1 - User Registration
        /// </summary>
        public void Register_InvalidEmail()
        {
            
            var email = "invalidEmail";
            var password = "Abc12345";
            var expected = "Invalid email format";

            var result = service.Register(email, password);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            var errorMessage = resultJson.GetProperty("ErrorMessage").GetString();

            if (errorMessage == expected)
                Console.WriteLine("Passed: Invalid email detected.");
            else
                Console.WriteLine($"Failed: Expected '{expected}', got '{errorMessage}'");
        }
        public void Register_DuplicateEmail()
        {
           
            var email = "duplicate@example.com";
            var password = "Abc12345";
            var expected = "Email already registered";

            service.Register(email, password);
            var result = service.Register(email, password);

            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            var errorMessage = resultJson.GetProperty("ErrorMessage").GetString();

            if (errorMessage == expected)
                Console.WriteLine("Passed: Duplicate email rejected.");
            else
                Console.WriteLine($"Failed: Expected '{expected}', got '{errorMessage}'");
        }
        public void Login_UserDoesNotExist()
        {
            
            var email = "nouser@example.com";
            var password = "Abc12345";
            var expected = "User does not exist";

            var result = service.Login(email, password);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            var errorMessage = resultJson.GetProperty("ErrorMessage").GetString();

            if (errorMessage == expected)
                Console.WriteLine("Passed: Non-existent user rejected.");
            else
                Console.WriteLine($"Failed: Expected '{expected}', got '{errorMessage}'");
        }
        public void Login_InvalidPassword()
        {
            
            var email = "user@example.com";
            var password = "Aabc12345";
            service.Register(email, password);
            service.Logout(email);
            
            var expected = "Incorrect password";

            var result = service.Login(email, "abc");
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            var errorMessage = resultJson.GetProperty("ErrorMessage").GetString();

            if (errorMessage == expected)
                Console.WriteLine("Passed: Invalid password rejected.");
            else
                Console.WriteLine($"Failed: Expected '{expected}', got '{errorMessage}'");
        }
        public void Login_NonExistingUser_ShouldFail()
        {
            
            var email = "notfound@example.com";
            var password = "Password1";

            var result = service.Login(email, password);
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            var errorMessage = resultJson.GetProperty("ErrorMessage").GetString();
            var expected = "User does not exist";

            if (errorMessage == expected)
                Console.WriteLine("Passed: Non-existent user correctly rejected.");
            else
                Console.WriteLine($"Failed: Expected '{expected}', got '{errorMessage}'.");
        }
        public void Logout_WithoutLogin_ShouldFail()
        {
           
            var result = service.Logout("not@loggedin.com");
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            var error = resultJson.GetProperty("ErrorMessage").GetString();
            var expected = "User is not logged in";

            if (error == expected)
                Console.WriteLine("Passed: Logout without login correctly failed.");
            else
                Console.WriteLine($"Failed: Expected '{expected}', got '{error}'.");
        }
        public void Login_ValidCredentials_ShouldSucceed()
        {

            
            service.Register("valid2@user.com", "Secure123");

            service.Logout("valid2@user.com");
            var result = service.Login("valid2@user.com", "Secure123");
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            var error = resultJson.GetProperty("ErrorMessage").GetString();

            if (string.IsNullOrEmpty(error))
                Console.WriteLine("Passed: Login with valid credentials succeeded.");
            else
                Console.WriteLine($"Failed: Expected success, got error '{error}'.");
        }
        public void Register_ValidUser_ShouldSucceed()
        {
            
            var result = service.Register("new@user.com", "Valid123");
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            var error = resultJson.GetProperty("ErrorMessage").GetString();

            if (string.IsNullOrEmpty(error))
                Console.WriteLine("Passed: Register with valid data succeeded.");
            else
                Console.WriteLine($"Failed: Expected success, got error '{error}'.");
        }
        public void Logout_EmptyEmail_ShouldFail()
        {
            
            var result = service.Logout("");
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            var error = resultJson.GetProperty("ErrorMessage").GetString();
            var expected = "User is not logged in";

            if (error == expected)
                Console.WriteLine("Passed: Empty email correctly rejected.");
            else
                Console.WriteLine($"Failed: Expected '{expected}', got '{error}'.");
        }
        public void Login_WrongPassword_ShouldFail()
        {
            
            service.Register("user@pass.com", "Correct123");
            var result = service.Login("user@pass.com", "WrongPassword");
            var resultJson = JsonSerializer.Deserialize<JsonElement>(result);
            var error = resultJson.GetProperty("ErrorMessage").GetString();
            var expected = "Incorrect password";

            if (error == expected)
                Console.WriteLine("Passed: Wrong password correctly rejected.");
            else
                Console.WriteLine($"Failed: Expected '{expected}', got '{error}'.");
        }
        
    }
}
