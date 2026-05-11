
namespace GieudexPol.Domain.Entities
{
    public class AuditLog
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Action { get; set; }
        public string EntityName { get; set; }
        public Guid EntityId { get; set; }
        public string Changes { get; set; }
        public DateTime Timestamp { get; set; }
        public User User { get; set; }
    }
}
