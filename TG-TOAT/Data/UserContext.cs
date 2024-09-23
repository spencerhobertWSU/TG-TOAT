using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGTOAT.Models;

namespace TGTOAT.Data
{
    public class UserContext : DbContext
    {
        public UserContext (DbContextOptions<UserContext> options)
            : base(options)
        {
        }
        // User entity
        public DbSet<TGTOAT.Models.User> User { get; set; } = default!;
    }
}
