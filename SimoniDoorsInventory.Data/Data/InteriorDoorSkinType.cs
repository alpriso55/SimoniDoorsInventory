using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimoniDoorsInventory.Data
{
    [Table("InteriorDoorSkinTypes")]
    public partial class InteriorDoorSkinType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [MaxLength(20)]
        public string InteriorDoorSkinTypeID { get; set; }

        [MaxLength(100)]
        public string Description { get; set; }
    }
}
