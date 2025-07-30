using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisCaddy.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Hole> Holes { get; set; } = new();
        public string LocationName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
