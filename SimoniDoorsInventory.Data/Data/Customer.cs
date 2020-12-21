using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimoniDoorsInventory.Data
{
    [Table("Customers")]
    public partial class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long CustomerID { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(15)]
        public string Phone1 { get; set; }

        [MaxLength(15)]
        public string Phone2 { get; set; }

        [MaxLength(50)]
        [EmailAddress]
        public string Email { get; set; }

        [MaxLength(120)]
        public string AddressLine { get; set; }

        [MaxLength(30)]
        public string City { get; set; }

        [MaxLength(5)]
        public string PostalCode { get; set; }

        public int Floor { get; set; }

        [Required]
        public DateTimeOffset CreatedOn { get; set; }
        [Required]
        public DateTimeOffset? LastModifiedOn { get; set; }

        [MaxLength(200)]
        public string Observations { get; set; }

        public byte[] Picture { get; set; }
        public byte[] Thumbnail { get; set; }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(LastName))
                return $"{FirstName}";
            else
                return $"{FirstName} {LastName}";
        }

        // ----------------------------------------------------------
        // Collections
        public string SearchTerms { get; set; }
        public string BuildSearchTerms() => 
            $"{CustomerID} {FirstName} {LastName} {Phone1} {Phone2} {Email} {AddressLine} {City}".ToLower();
        
        public virtual ICollection<Order> Orders { get; set; }
    }
}
