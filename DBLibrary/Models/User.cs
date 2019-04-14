using System;
using System.Collections.Generic;
using System.Text;

namespace DBLibrary.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Position { get; set; }
        public string CityNumber { get; set; }
        public string LocalNumber { get; set; }
        public string MobileNumber { get; set; }
        public string CabinetNumber { get; set; }
        public int SequenceNumber { get; set; }

        public int? RoleId { get; set; }
        public virtual Role Role { get; set; }
        public int? DepartmentId { get; set; }
        public virtual Department Department { get; set; }
        public virtual ICollection<Friend> AddedBys { get; set; }
        public virtual ICollection<Friend> AddedTos { get; set; }
        public string FIO
        {
            get { return FirstName + " " + Name + " " + LastName; }
        }
        public User()
        {
            AddedBys = new List<Friend>();
            AddedTos = new List<Friend>();
        }
    }
}
