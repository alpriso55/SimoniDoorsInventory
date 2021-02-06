using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimoniDoorsInventory.Data
{
    [Table("InteriorDoorSkins")]
    public partial class InteriorDoorSkin
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [MaxLength(20)]
        public string InteriorDoorSkinID { get; set; }

        public int StockUnits { get; set; }

        public int SafetyStockLevel { get; set; }

        [MaxLength(100)]
        public string Description { get; set; }

        // Collections
        public string SearchTerms { get; set; }
        public string BuildSearchTerms() =>
            $"{InteriorDoorSkinID}".ToLower();

        public virtual ICollection<InteriorDoor> InteriorDoors { get; set; }
    }
}
