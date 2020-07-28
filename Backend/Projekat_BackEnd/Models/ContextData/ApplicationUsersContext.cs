using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Projekat_BackEnd.Models.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Projekat_BackEnd.Models.ContextData
{
    public class ApplicationUsersContext : IdentityDbContext<User, Role, string>
    {
        public ApplicationUsersContext(DbContextOptions<ApplicationUsersContext> options) : base(options)
        {

        }

        public DbSet<User> ApplicationUsers { get; set; }
        public DbSet<RentACar> RentACarCompanies { get; set; }
        public DbSet<Airline> AirlineCompanies { get; set; }
    }
}
