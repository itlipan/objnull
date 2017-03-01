using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCWeb.Model.Models
{
    public class Resume
    {
        public Resume()
        {
            ID = Guid.NewGuid();
        }

        public Guid ID { get; set; }

        public bool Public { get; set; }

        public string MD { get; set; }

        public string Html { get; set; }

        public Guid UserID { get; set; }

        public virtual NullUser User { get; set; }
    }
}
