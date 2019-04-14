using DBLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppBallet.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string FIO { get; set; }
        public string Position { get; set; }
        public string CityNumber { get; set; }
        public string LocalNumber { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public string CabinetNumber { get; set; }
        public int SequenceNumber { get; set; }
        public int? DepartmentId { get; set; }
        public Department Department { get; set; }
    }
}
