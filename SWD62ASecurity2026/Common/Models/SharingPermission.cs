using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class SharingPermission
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Blog")]    
        public int BlogFK { get; set; }
        public Blog Blog { get; set; }

        public string UserEmail { get; set; }

        public string PermissionType { get; set; } // e.g., "Read", "Write", "Owner"
    }
}
