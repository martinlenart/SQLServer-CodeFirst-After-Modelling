using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SeidoDemoModels
{
    
    public class Order
    {
        #region Uncomment to create the Data model
        
        [Key]
        public int OrderID { get; set; }

        [ForeignKey(nameof(CustomerID))]
        public int CustomerID { get; set; }
        virtual public Customer Customer { get; set; }

        [Column(TypeName = "nvarchar (200)")]
        public string Comment { get; set; }
        
        #endregion

        #region Uncomment to seed and query the Database
        
        public Order()
        {
            Comment = $"specific comment for Customer";
        }
        
        #endregion
    }
    
}
