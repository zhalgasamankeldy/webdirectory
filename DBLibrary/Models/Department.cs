using System;
using System.Collections.Generic;
using System.Text;

namespace DBLibrary.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string DepartmentName { get; set; }
        public int SequenceNumber { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public Department()
        {
            Users = new List<User>();
        }
    }
}
