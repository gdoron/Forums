using System.Collections.Generic;

namespace Entities
{
    public class Forum
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public virtual List<Post> Posts{ get; set; }
    }
}