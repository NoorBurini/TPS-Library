using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace DataAccessLayer.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
            public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }
            public DbSet<Shelf> Shelfs => Set<Shelf>();
            public DbSet<Book> Books => Set<Book>();
            public DbSet<Category> Categories => Set<Category>();
        public DbSet<Rating> Ratings => Set<Rating>();
        // public DbSet<AppUser> AppUsers => Set<AppUser>();


    }
}
