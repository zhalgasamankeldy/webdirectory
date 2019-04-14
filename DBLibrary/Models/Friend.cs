using System;
using System.Collections.Generic;
using System.Text;

namespace DBLibrary.Models
{
    public class Friend
    {
        public int AddedById { get; set; }
        public virtual User AddedBy { get; set; }
        public int AddedToId { get; set; }
        public virtual User AddedTo { get; set; }
    }
}
