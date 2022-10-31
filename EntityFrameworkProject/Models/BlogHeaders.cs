using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EntityFrameworkProject.Models
{
    public class BlogHeaders: BaseEntity
    {
        public string Description { get; set; }
        public string Title { get; set; }
    }
}
