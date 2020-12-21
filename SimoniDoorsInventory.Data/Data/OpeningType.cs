using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimoniDoorsInventory.Data
{
    [Table("OpeningTypes")]
    public partial class OpeningType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OpeningTypeID { get; set; }

        [Required]
        [MaxLength(20)]
        public string Name { get; set; }
    }
}
