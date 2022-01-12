using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AuthServices.Helpers;
using AuthServices.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AuthServices.Data
{
    public class DALUser : IUser
    {
        private ApplicationDbContext _db;
        private IConfiguration _config;

        public DALUser(
            ApplicationDbContext db,
            IConfiguration configuration)
        {
            _db = db;
            _config = configuration;
        }
        public async Task<string> Authenticate(string username, string password)
        {
            var userFound = await (
                from user in _db.Users
                where user.Username == username && user.Password == password
                select user
            ).SingleOrDefaultAsync();

            if (userFound == null) throw new System.Exception("Invalid Credentials");

            var roles = await GetRoles(userFound.Id);

            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, userFound.Username));

            if (userFound.StudentId != null)
            {
                claims.Add(new Claim("Id", userFound.StudentId.ToString()));
            }

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["AppSettings:Secret"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),

                Expires = DateTime.UtcNow.AddHours(1),

                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature),
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            //userFound.Token = tokenHandler.WriteToken(token);

            //Console.WriteLine(newUser.Token);

            return tokenHandler.WriteToken(token);
        }

        public Task<User> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<User> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Role>> GetRoles(int userId)
        {
            var roles = await (
                from user in _db.Users
                join userRole in _db.UserRoles on user.Id equals userRole.UserId
                join role in _db.Roles on userRole.RoleId equals role.Id
                where user.Id == userId
                select role
            ).ToListAsync();

            //Console.WriteLine(roles[0]);

            return roles;
        }

        public Task<User> Insert(User obj)
        {
            throw new NotImplementedException();
        }

        public Task<User> Update(int id, User obj)
        {
            throw new NotImplementedException();
        }
    }
}