using System.ComponentModel.DataAnnotations;

namespace Banking.Models.Entities
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string TaxCode { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string Email { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        public bool IsActive { get; set; } = true;
        
        // Navigation property for bank accounts
        public virtual ICollection<BankAccount> BankAccounts { get; set; } = new List<BankAccount>();
    }
} 