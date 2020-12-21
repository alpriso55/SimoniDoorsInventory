using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimoniDoorsInventory.Data
{
    [Table("InteriorDoors")]
    public partial class InteriorDoor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long OrderID { get; set; }

        [Key, Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int InteriorDoorID { get; set; }

        [Required]
        [MaxLength(20)]
        public string InteriorDoorSkinID { get; set; }
        public virtual InteriorDoorSkin InteriorDoorSkin { get; set; }

        [MaxLength(10)]
        public string InteriorDoorDesignID { get; set; }
        public virtual InteriorDoorDesign InteriorDoorDesign { get; set; }

        [Required]
        public int OpeningTypeID { get; set; }
        [Required(ErrorMessage = "Το άνοιγμα της πόρτας είναι υποχρεωτικό")]
        public int OpeningSideID { get; set; }

        [Required(ErrorMessage = "Η πόρτα πρέπει να έχει ένα πλάτος")]
        public int Width { get; set; }

        [Required(ErrorMessage = "Η πόρτα πρέπει να έχει ένα ύψος")]
        public int Height { get; set; }

        [Required(ErrorMessage = "Η πόρτα πρέπει να έχει φάρδος κάσας")]
        public int Lamb { get; set; }

        public int? AccessoryID { get; set; }

        [MaxLength(400)]
        public string Observations { get; set; }

        [Required(ErrorMessage = "Η τιμή της πόρτας είναι υποχρεωτική")]
        public decimal Price { get; set; }

        // -----------------------------------------------
        // Collections
        public string SearchTerms { get; set; }
        public string BuildSearchTerms() =>
            $"{OrderID} {InteriorDoorSkinID} {InteriorDoorDesignID}".ToLower();
    }
}
