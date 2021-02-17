using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimoniDoorsInventory.Data
{
    [Table("Orders")]
    public partial class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long OrderID { get; set; }

        [Required]
        public long CustomerID { get; set; }
        public virtual Customer Customer { get; set; }

        [MaxLength(20)]
        public string OrderName { get; set; }

        // [Required]
        // public long AccountID { get; set; }
        // public virtual Account Account { get; set; }

        public int CrewID { get; set; }
        public virtual Crew Crew { get; set; }

        [Required]
        [MaxLength(150)]
        public string AddressLine { get; set; }

        [Required]
        [MaxLength(50)]
        public string City { get; set; }

        [MaxLength(5)]
        public string PostalCode { get; set; }

        public int Floor { get; set; }

        [Required]
        public DateTimeOffset OrderDate { get; set; }

        public DateTimeOffset? DeliveryDateTime { get; set; }

        [Required]
        public int Status { get; set; }
        public virtual OrderStatus OrderStatus { get; set; }

        public decimal? TotalCost { get; set; }

        [MaxLength(200)]
        public string Observations { get; set; }

        [Required]
        public DateTimeOffset LastModifiedOn { get; set; }

        // ------------------------------------------------------------------
        // Collections
        public string SearchTerms { get; set; }
        public string BuildSearchTerms() =>
            $"{OrderID} {CustomerID} {Customer?.FirstName} {Customer?.LastName} {CrewID} {AddressLine} {City} {Status} {OrderStatus?.Name}".ToLower();

        public virtual ICollection<InteriorDoor> InteriorDoors { get; set; }
    }
}
