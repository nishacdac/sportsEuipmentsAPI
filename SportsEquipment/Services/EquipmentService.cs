
using Dapper;
using MySql.Data.MySqlClient;
using SportsEquipment.Interfaces;
using SportsEquipment.Model;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SportsEquipment.Services
{
    public class EquipmentService : IEquipmentService
    {
        private readonly IConfiguration _configuration;

        public EquipmentService(IConfiguration configuration)
        {
            _configuration = configuration; 
        }

        



        //nisha ka code hai registraton wala 
        public async Task<bool> RegisterCustomerEquipment(Equipment equipment)
        {    
            // Hash the password before saving
            equipment.Password = HashPassword(equipment.Password);

            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using var connection = new MySqlConnection(connectionString);

            try
            {
                await connection.OpenAsync(); // Open the connection explicitly

                using var transaction = await connection.BeginTransactionAsync(); // Begin the transaction

                // Insert into registration table
                var registrationSql = @"
     INSERT INTO registration 
     (FirstName, LastName, Email, Gender, MobileNumber, Password, RoleId) 
     VALUES 
     (@FirstName, @LastName, @Email, @Gender, @MobileNumber, @Password, @RoleId);
     SELECT LAST_INSERT_ID();"; // Retrieve the inserted Id

                var newRegistrationId = await connection.ExecuteScalarAsync<int>(
                    registrationSql,
                    equipment,
                    transaction);

                // If RoleId is 2, insert data into customer table
                if (equipment.RoleId == 2)
                {
                    var customerSql = @"
         INSERT INTO customers 
         (Id, Gender, MobileNumber,Address) 
         VALUES 
         (@Id, @Gender, @MobileNumber,@Address);";

                    await connection.ExecuteAsync(
                        customerSql,
                        new
                        {
                            Id = newRegistrationId,
                            Gender = equipment.Gender,
                            MobileNumber = equipment.MobileNumber,
                            address = equipment.Address
                        },
                        transaction);
                }

                await transaction.CommitAsync();
                return true;
            }
            catch
            {

                throw; // Rethrow the exception to handle it outside
            }
            finally
            {
                await connection.CloseAsync(); // Ensure the connection is closed
            }
        }

         


        public async Task<bool> AddCustomerAsync(Equipment equipment)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (var connection = new MySqlConnection(connectionString))
            {
                // Open the connection to the database
                await connection.OpenAsync();

                // Start a transaction to ensure both inserts are done atomically
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        // Insert into registration table
                        var registrationSql = @"
                 INSERT INTO registration (FirstName, LastName, Email, Password, RoleId)
                 VALUES (@FirstName, @LastName, @Email, @Password, @RoleId);
             ";

                        await connection.ExecuteAsync(registrationSql, new
                        {
                            FirstName = equipment.FirstName,
                            LastName = equipment.LastName,
                            Email = equipment.Email,
                            Password = equipment.Password,
                            RoleId = equipment.RoleId
                        }, transaction);

                        // Get the generated registration_id
                        var registrationId = await connection.QuerySingleAsync<int>("SELECT LAST_INSERT_ID()", transaction);

                        // Insert into customer table with the generated registration_id
                        var customerSql = @" 
                 INSERT INTO customer (registration_id, phone_no, gender)
                 VALUES (@RegistrationId, @PhoneNo, @Gender);
             ";

                        var result = await connection.ExecuteAsync(customerSql, new
                        {
                            RegistrationId = registrationId,
                            PhoneNo = equipment.MobileNumber,
                            Gender = equipment.Gender
                        }, transaction);

                        // Commit the transaction if both inserts are successful
                        await transaction.CommitAsync();

                        return result > 0;
                    }
                    catch (Exception)
                    {
                        // Rollback the transaction if any error occurs
                        await transaction.RollbackAsync();
                        return false;
                    }
                }
            }
        }

        ///////////////////////seller profile/////////////////


        public async Task<bool> SellerEquipment(Equipment equipment)
        {
            // Hash the password before saving
            equipment.Password = HashPassword(equipment.Password);

            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using var connection = new MySqlConnection(connectionString);

            try
            {
                await connection.OpenAsync(); // Open the connection explicitly

                using var transaction = await connection.BeginTransactionAsync(); // Begin the transaction

                // Insert into registration table
                var registrationSql = @"
INSERT INTO registration 
(FirstName, LastName, Email, Gender, MobileNumber, Password, RoleId) 
VALUES 
(@FirstName, @LastName, @Email, @Gender, @MobileNumber, @Password, @RoleId); SELECT LAST_INSERT_ID();"; // Retrieve the inserted Id

                var newRegistrationId = await connection.ExecuteScalarAsync<int>(
                    registrationSql,
                    equipment,
                    transaction);

                // If RoleId is 3 (Assuming RoleId 3 is for Seller), insert data into seller table
                if (equipment.RoleId == 3)
                {
                    var sellerSql = @"
    INSERT INTO sellers 
    (userId,  Gender, Address, Image, MobileNumber) 
    VALUES 
    (@userId, @Gender, @Address, @Image, @MobileNumber );";

                    await connection.ExecuteAsync(
                        sellerSql,
                        new
                        {
                            userId = newRegistrationId,
                            Address = equipment.Address,
                            Image = equipment.Image,
                            Gender = equipment.Gender,
                            MobileNumber = equipment.MobileNumber
                        },
                        transaction);
                }

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                throw; 
            }
            finally
            {
                await connection.CloseAsync(); 
            }
        }

        public async Task<bool> AddSellerAsync(Equipment equipment)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (var connection = new MySqlConnection(connectionString))
            {
                // Open the connection to the database
                await connection.OpenAsync();

                // Start a transaction to ensure both inserts are done atomically
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        // Insert into registration table
                        var registrationSql = @"
            INSERT INTO registration (Email, Password, RoleId)
            VALUES ( @Email, @Password, @RoleId);
        ";

                        await connection.ExecuteAsync(registrationSql, new
                        {

                            Email = equipment.Email,
                            Password = equipment.Password,
                            RoleId = equipment.RoleId
                        }, transaction);

                        // Get the generated registration_id
                        var registrationId = await connection.QuerySingleAsync<int>("SELECT LAST_INSERT_ID()", transaction);

                        // Insert into seller table with the generated registration_id
                        var sellerSql = @" 
            INSERT INTO seller (registration_id, Address, Image, Gender, MobileNumber)
            VALUES (@RegistrationId, @Address, @Image, @Gender, @MobileNumber);
        ";

                        var result = await connection.ExecuteAsync(sellerSql, new
                        {
                            RegistrationId = registrationId,
                            Address = equipment.Address,
                            Image = equipment.Image,
                            Gender = equipment.Gender,
                            MobileNumber = equipment.MobileNumber
                        }, transaction);

                        // Commit the transaction if both inserts are successful
                        await transaction.CommitAsync();

                        return result > 0;
                    }
                    catch (Exception)
                    {
                        // Rollback the transaction if any error occurs
                        await transaction.RollbackAsync();
                        return false;
                    }
                }
            }
        }






        /// <summary>
        /// customer and seller login
        /// </summary>
        /// <param name="equipmentLogin"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// 

        public async Task<string?> LoginUser(EquipmentLogin equipmentLogin)
        {
            if (equipmentLogin == null)
            {
                throw new ArgumentNullException(nameof(equipmentLogin), "The equipmentLogin parameter cannot be null.");
            }

            if (string.IsNullOrEmpty(equipmentLogin.Email) || string.IsNullOrEmpty(equipmentLogin.Password))
            {
                throw new ArgumentException("Email and Password cannot be null or empty");
            }

            using var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var sql = "SELECT * FROM registration WHERE Email = @Email AND RoleId = @RoleId";
            var user = await connection.QueryFirstOrDefaultAsync<Equipment>(sql, new
            {
                Email = equipmentLogin.Email,
                RoleId = equipmentLogin.RoleId,
               

            });

            // Compare hashed passwords
            if (user != null && VerifyPasswordHash(equipmentLogin.Password, user.Password))
            {
                return GenerateJwtToken(user.Email, user.RoleId, user.Id, user.FirstName);
            }

            return null; // If login fails, return null
        }

        /// <summary>
        /// admin login
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>


        public async Task<string?> RegisterEquipment(AdminLogin adminLogin)
        {
            if (adminLogin == null)
            {
                throw new ArgumentNullException(nameof(adminLogin), "The equipmentLogin parameter cannot be null.");
            }

            if (string.IsNullOrEmpty(adminLogin.Email) || string.IsNullOrEmpty(adminLogin.Password))
            {
                throw new ArgumentException("Email and Password cannot be null or empty");
            }

            using var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var sql = "SELECT * FROM registration WHERE Email = @Email AND RoleId = @RoleId";
            var user = await connection.QueryFirstOrDefaultAsync<AdminLogin>(sql, new
            {
                Email = adminLogin.Email,
                RoleId = adminLogin.RoleId,
                FirstName = adminLogin.FirstName
            });

            // Compare hashed passwords
            if (user != null && VerifyPasswordHash(adminLogin.Password, user.Password))
            {
                return GenerateJwtToken(user.Email, user.RoleId, user.Id, user.FirstName);
            }

            return null; // If login fails, return null
        }

        private string GenerateJwtToken(string email, int roleId, int id, string firstname)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.Email, email),
            new Claim("RoleId", roleId.ToString()),
            new Claim("Id", id.ToString()),
            new Claim("FirstName", firstname )
        }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string HashPassword(string password)
        {
            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] bytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // Verify hashed password
        private bool VerifyPasswordHash(string enteredPassword, string storedHashedPassword)
        {
            string hashedEnteredPassword = HashPassword(enteredPassword);
            return hashedEnteredPassword == storedHashedPassword;
        }
    }
}













