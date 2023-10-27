using System.ComponentModel.DataAnnotations;

namespace Calendar.shared.Entities
{
    public abstract class EntityBase
    {
        [Key]
        public int Id { get; set; }
    }
}
