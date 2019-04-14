using System;
using System.Collections.Generic;
using System.Text;

namespace DBLibrary.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public Role()
        {
            Users = new List<User>();
        }
    }
}
