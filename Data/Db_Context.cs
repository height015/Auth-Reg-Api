using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RegAuthApiDemo.Domain;

namespace RegAuthApiDemo.Data
{
	public class Db_Context : IdentityDbContext<User>
	{
        public Db_Context(DbContextOptions<Db_Context> options): base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
           

        }

        //public DbSet<User> Users { get; set; }


    }
}

