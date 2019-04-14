using System;
using System.Collections.Generic;
using System.Text;

namespace DBLibrary.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }
        public string Color { get; set; }
        public bool IsFullDay { get; set; }
    }
}
