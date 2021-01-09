using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimoniDoorsInventory.Data
{
    [Table("PaymentTypes")]
    public partial class PaymentType
    {
        [Key]
        [DatabaseGenerat‌​ed(DatabaseGeneratedOption.None)]
        public int PaymentTypeID { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        // ----------------------------------------------------------
        // Collections
        public virtual ICollection<Payment> Payments { get; set; }
    }
}
