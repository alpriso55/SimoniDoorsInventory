using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimoniDoorsInventory.Data
{
    [Table("InteriorDoorDesigns")]
    public partial class InteriorDoorDesign
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [MaxLength(10)]
        public string InteriorDoorDesignID { get; set; }

        [MaxLength(100)]
        public string Description { get; set; }

        public byte[] Picture { get; set; }
    }
}
