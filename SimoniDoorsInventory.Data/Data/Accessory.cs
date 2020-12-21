using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimoniDoorsInventory.Data
{
    [Table("Accessories")]
    public partial class Accessory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AccessoryID { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }
    }
}
