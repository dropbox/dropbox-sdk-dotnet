using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleBlogDemo.Models
{
    public class UserProfile
    {
        public int ID { get; set; }

        [Required, StringLength(255, MinimumLength = 3)]
        public string UserName { get; set; }

        public string DropboxAccessToken { get; set; }

        [RegularExpression("^[A-Za-z][A-Za-z'-']*[A-Za-z]$")]
        public string BlogName { get; set; }

        [StringLength(32)]
        public string ConnectState { get; set; }
    }

    public class UsersStore : DbContext
    {
        public DbSet<UserProfile> Users { get; set; }
    }
}
