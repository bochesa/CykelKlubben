using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CykelKlubben.Models
{
    public class IdentityDataContext : IdentityDbContext<AppUser>
    {
        public IdentityDataContext(DbContextOptions<IdentityDataContext> options) : base(options)
        {
            //Database.EnsureCreated();
        }
    }
}
