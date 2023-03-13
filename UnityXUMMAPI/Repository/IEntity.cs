namespace UnityXUMMAPI.Repository
{
    public interface IEntity
    {
        Guid Id { get; set; }
         DateTime CreatedOn { get; set; }
    }
}
