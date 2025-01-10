namespace Webapi.shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

/*
        Account Start
*/

public class AccountRoles
{
    [Key]
    public int AccountRoleID { get; set; }

    public int RoleID { get; set; } // Foreign key for Roles
    [ForeignKey("RoleID")]
    public virtual Roles Roles { get; set; }

    public string Id { get; set; } // Foreign key for ApplicationUser
    [ForeignKey("Id")]
    public virtual ApplicationUser Account { get; set; }
}

public class SecureAccountA
{
    [Key]
    public int ASecAccountID { get; set; }

    public string AddressLine1 { get; set; } = string.Empty;
    public string AddressLine2 { get; set; } = string.Empty;
    public string AddressLine3 { get; set; } = string.Empty;
    public string AddressLine4 { get; set; } = string.Empty;

    public string Id { get; set; } // Foreign key for ApplicationUser
    [ForeignKey("Id")]
    public virtual ApplicationUser Account { get; set; }
}

/*
        Account Stop
*/

/*
        Order Start
*/

public class Order
{
    [Key]
    public int OrderID { get; set; }

    [Required]
    public DateTime OrderDate { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    // Foreign keys
    public int OrderStatusID { get; set; } // Foreign key for OrderStatus
    [ForeignKey("OrderStatusID")]
    public virtual OrderStatus OrderStatus { get; set; }


    [ForeignKey("OrderID")]
    public virtual ICollection<OrderDetails> OrderDetails { get; set; }

    public string Id { get; set; } // Foreign key for ApplicationUser
    [ForeignKey("Id")]
    public virtual ApplicationUser Account { get; set; }
}

public class OrderDetails
{
    [Key]
    public int OrderDetailID { get; set; }

    [Required]
    public int Quantity { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalPrice { get; set; }

    [ForeignKey("OrderID")]
    public int OrderID { get; set; }

    public int ProductID { get; set; } // Foreign key for Product
    [ForeignKey("ProductID")]
    public virtual Product Product { get; set; }
}

public class OrderStatus
{
    [Key]
    public int OrderStatusID { get; set; }

    public string Status { get; set; }
}
/*
        Order Stop
*/

/*
        Product Start
*/

public class Product
{
    [Key]
    public int ProductID { get; set; }

    public int ProductCategoryID { get; set; } // Foreign key for ProductCategory
    [ForeignKey("ProductCategoryID")]
    public virtual ProductCategory ProductCategory { get; set; }

    public int ProductDataID { get; set; } // Foreign key for ProductData
    [ForeignKey("ProductDataID")]
    public virtual ProductData ProductData { get; set; }

    public int ProductDetailsID { get; set; } // Foreign key for ProductDetails
    [ForeignKey("ProductDetailsID")]
    public virtual ProductDetails ProductDetails { get; set; }

    public int ProductImageID { get; set; } // Foreign key for ProductImage
    [ForeignKey("ProductImageID")]
    public virtual ProductImage ProductImage { get; set; }

    public int ProductTagID { get; set; } // Foreign key for ProductTag
    [ForeignKey("ProductTagID")]
    public virtual ProductTag ProductTag { get; set; }

    public string Id { get; set; } // Foreign key for ApplicationUser
    [ForeignKey("Id")]
    public virtual ApplicationUser Account { get; set; }

    // Collection navigation property for reviews
    [JsonIgnore]
    public virtual ICollection<ProductReviews> ProductReviews { get; set; } = new List<ProductReviews>();
}

public class ProductCategory
{
	[Key]
	public int ProductCategoryID { get; set; }

	[Required]
	[MaxLength(100)]
	public string CategoryName { get; set; } = string.Empty;

	[Required]
	public string CategoryIcon { get; set; } = string.Empty;

	public string CategoryDesc { get; set; } = string.Empty;

	// Navigation property for related products
	[JsonIgnore]
	public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}

public class ProductData
{
    [Key]
    public int ProductDataID { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal ProductPrice { get; set; }

    [Required]
    public int ProductStock { get; set; }

    [Required]
    public int ProductSold { get; set; }

    [Required]
    public int IsFeatured { get; set; }

    [Required]
    public int IsArchived { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }
}

public class ProductDetails
{
    [Key]
    public int ProductDetailsID { get; set; }

    [Required]
    [MaxLength(100)]
    public string ProductName { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string ProductDescription { get; set; } = string.Empty; // Changed from int to string

    [Required]
    [MaxLength(150)]
    public string ProductDescriptionSmall { get; set; } = string.Empty; // Changed from int to string
}

public class ProductImage
{
    [Key]
    public int ProductImageID { get; set; }

    [Required]
    public string SmallImage { get; set; } = string.Empty; // Changed from int to string

    [Required]
    public string BigImage { get; set; } = string.Empty; // Changed from int to string
}

public class ProductReviews
{
    [Key]
    public int ProductReviewID { get; set; }

    [Required]
    public string Header { get; set; } = string.Empty;

    [Required]
    public string Message { get; set; } = string.Empty;

    [Required]
    public int UpVotes { get; set; }

    [Required]
    public int DownVotes { get; set; }

    [Required]
    public int Stars { get; set; }

    [Required]
    public int IsApproved { get; set; }

    public int ProductID { get; set; } // Foreign key for Product

    public string Id { get; set; } // Foreign key for ApplicationUser

}

public class ProductTag
{
    [Key]
    public int ProductTagID { get; set; }

    [Required]
    [MaxLength(100)]
    public string TagName { get; set; } = string.Empty;
}

/*
        Product Stop
*/

/*
        Review Start
*/
public class Review
{
    [Key]
    public int ReviewID { get; set; }

    [Required]
    public string Header { get; set; } = string.Empty;

    [Required]
    public string Message { get; set; } = string.Empty;

    [Required]
    public int Stars { get; set; }

    public string Id { get; set; } // Foreign key for ApplicationUser
    [ForeignKey("Id")]
    public virtual ApplicationUser Account { get; set; }
}
/*
        Review Stop
*/

/*
        Roles Start
*/
public class Roles
{
    [Key]
    public int RoleID { get; set; }

    [Required]
    [MaxLength(50)]
    public string RoleName { get; set; } = string.Empty;
}
/*
        Roles Stop
*/