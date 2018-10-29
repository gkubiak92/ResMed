using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ResMed.Models;

namespace ResMed.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Doctors> Doctors { get; set; }
        public DbSet<Patients> Patients { get; set; }
        public DbSet<VisitTypes> VisitTypes { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<Specializations> Specializations { get; set; }

    }
}
