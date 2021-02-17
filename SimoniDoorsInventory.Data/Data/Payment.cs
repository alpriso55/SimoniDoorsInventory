using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimoniDoorsInventory.Data
{
    [Table("Payments")]
    public partial class Payment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long PaymentID { get; set; }

        [Required]
        public long CustomerID { get; set; }
        public virtual Customer Customer { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public int? PaymentTypeID { get; set; }
        public PaymentType PaymentType { get; set; }

        [Required]
        public DateTimeOffset PaymentDate { get; set; }

        [MaxLength(200)]
        public string Observations { get; set; }

        // -----------------------------------------------
        // Collections
        public string SearchTerms { get; set; }
        public string BuildSearchTerms() =>
            $"{PaymentID} {CustomerID} {Customer?.FirstName} {Customer?.LastName} {PaymentType?.Name}".ToLower();
    }
}
