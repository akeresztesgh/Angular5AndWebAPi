using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace api.Models
{
    public class MyDbContext : IdentityDbContext<User>
    {
        public MyDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static MyDbContext Create()
        {
            return new MyDbContext();
        }
    }
}