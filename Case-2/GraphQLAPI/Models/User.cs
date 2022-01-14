using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GraphQLAPI.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public string Token { get; set; }
        public bool isLocked { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }
        public ICollection<Twittor> Twittors { get; set; }
    }
}