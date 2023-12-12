using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;

namespace API.Entites
{
    public class UserLike
    {
        public int SourceUserId { get; set; }
 
        public int TargetUserId { get; set; }

        public virtual AppUser SourceUser { get; set; }

        public virtual AppUser TargetUser { get; set; }
    }
}
