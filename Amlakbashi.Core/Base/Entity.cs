using System.ComponentModel.DataAnnotations;

namespace Amlakbashi.Core
{
    public abstract class Entity<Key>
    {
        [Key]
        public virtual Key Id { get; set; }

    }
}
