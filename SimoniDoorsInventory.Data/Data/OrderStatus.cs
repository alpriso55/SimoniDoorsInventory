﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimoniDoorsInventory.Data
{
    [Table("OrderStatus")]
    public partial class OrderStatus
    {
        [Key]
        [DatabaseGenerat‌​ed(DatabaseGeneratedOption.None)]
        public int Status { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}
