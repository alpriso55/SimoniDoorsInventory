using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimoniDoorsInventory.Data
{
    [Table("Accounts")]
    public partial class Account
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long AccountID { get; set; }

        [Required]
        public long CustomerID { get; set; }
        public virtual Customer Customer { get; set; }

        [Required]
        public decimal Balance { get; set; }

        [MaxLength(200)]
        public string Observations { get; set; }

        public string SearchTerms { get; set; }
        public string BuildSearchTerms() => $"{CustomerID}".ToLower();

        // --------------------------------------------------------
        // Collections
        public virtual ICollection<Payment> Payments { get; set; }
    }
}
