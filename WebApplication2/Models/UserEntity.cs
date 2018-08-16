using Microsoft.AspNetCore.Identity;
using System;


namespace WebApplication2.Models
{
    public class UserEntity : IdentityUser<Guid>
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}
