using UnityXUMMAPI.Repository;

namespace UnityXUMMAPI.Entities
{
    public class BaseEntity : IEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}
