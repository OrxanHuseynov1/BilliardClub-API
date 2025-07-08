using BusinessLayer.DTOs.SessionProduct;
using Domain.Enums;

namespace BusinessLayer.DTOs.TableSession
{
    public class TableSessionGetDTO
    {
        public Guid Id { get; set; }
        public Guid TableId { get; set; }
        public string TableName { get; set; } 
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; } 
        public decimal HourlyPrice { get; set; }
        public PaymentType PaymentType { get; set; }
        public List<SessionProductGetDTO> SessionProducts { get; set; } = []; 
    }
}