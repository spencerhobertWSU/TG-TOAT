using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MvcMovie.Models;

namespace MvcMovie.Data
{
    public class MvcMovieContext : DbContext
    {
        public MvcMovieContext (DbContextOptions<MvcMovieContext> options)
            : base(options)
        {
        }

        // Movie entity
        public DbSet<MvcMovie.Models.Movie> Movie { get; set; } = default!;

        // User entity
        public DbSet<MvcMovie.Models.User> User { get; set; } = default!;
    }
}
