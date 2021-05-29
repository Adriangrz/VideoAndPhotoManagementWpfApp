using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManager.Models
{
    public class Category
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }

        public ICollection<Picture> Pictures { get; set; }
        public ICollection<Movie> Movies { get; set; }
    }
}
