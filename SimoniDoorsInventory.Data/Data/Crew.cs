using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimoniDoorsInventory.Data
{
    [Table("Crews")]
    public partial class Crew
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CrewID { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(20)]
        public string Phone { get; set; }

        [MaxLength(200)]
        public string Observations { get; set; }

        public string SearchTerms { get; set; }
        public string BuildSearchTerms() => $"{CrewID} {Name} {Phone}".ToLower();

        // --------------------------------------------------------
        // Collections
        public virtual ICollection<Order> Orders { get; set; }
    }
}
