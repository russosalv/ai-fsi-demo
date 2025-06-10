using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Banking.Models.Entities
{
    public class BankAccount
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(34)]
        public string Iban { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string AccountName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(20)]
        public string AccountType { get; set; } = string.Empty; // "CHECKING", "SAVINGS", etc.
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; }
        
        public DateTime OpenDate { get; set; }
        
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        
        public bool IsActive { get; set; } = true;
        
        // Foreign key for Customer
        [Required]
        public int CustomerId { get; set; }
        
        // Navigation property for Customer
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; } = null!;
    }
} 