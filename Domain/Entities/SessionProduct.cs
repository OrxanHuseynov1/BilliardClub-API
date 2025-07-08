    using Domain.Entities.Common;


    namespace Domain.Entities;

    public class SessionProduct : AuditableEntity
    {
        public Guid TableSessionId { get; set; }
        public TableSession TableSession { get; set; } = default!;

        public Guid ProductId { get; set; }
        public Product Product { get; set; } = default!;

        public int Quantity { get; set; } = 0;
        public decimal UnitPrice { get; set; }
        public Guid CompanyId { get; set; }
        public Company Company { get; set; } = default!;
}
