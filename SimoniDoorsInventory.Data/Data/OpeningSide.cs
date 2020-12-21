using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimoniDoorsInventory.Data
{
    [Table("OpeningSides")]
    public partial class OpeningSide
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OpeningSideID { get; set; }

        [Required]
        [MaxLength(10)]
        public string Name { get; set; }
    }
}
