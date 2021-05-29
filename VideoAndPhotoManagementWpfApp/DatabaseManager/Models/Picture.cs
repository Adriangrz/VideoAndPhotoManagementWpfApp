using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManager.Models
{
    public class Picture
    {
        public Guid PictureId { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }

        public Guid CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
