using System.ComponentModel.DataAnnotations;

namespace Webapi.shared.DTOs
{
    public class orderDto
    {
        public int OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; } // Could be OrderStatusDto if needed
        public IEnumerable<OrderDetailDto> OrderDetails { get; set; }

    }

    public class OrderDetailDto
    {
        public int OrderDetailID { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; } // Include if needed
    }

    public class AccountRolesDto
    {
        public int AccountRoleID { get; set; }
        public int RoleID { get; set; }
        public string AccountId { get; set; } // Adjust if necessary
    }

    public class SecureAccountADto
    {
        public int ASecAccountID { get; set; }
        public string AddressLine1 { get; set; } = string.Empty;
        public string AddressLine2 { get; set; } = string.Empty;
        public string AddressLine3 { get; set; } = string.Empty;
        public string AddressLine4 { get; set; } = string.Empty;
    }

	public class ProductDto
	{
		public int ProductID { get; set; }
		public string ProductName { get; set; }
		public decimal ProductPrice { get; set; }
		public int ProductStock { get; set; }
		public int ProductQuantity { get; set; }
		public string ProductDescription { get; set; }

		public int isFeatured { get; set; }

		public int isArchived { get; set; }

		public int NumberofSold { get; set; }

		public DateTime CreatedAt { get; set; }

		public string Productimg { get; set; } = string.Empty;

		public int Productreviewcount { get; set; }

		public string Producttags { get; set; } = string.Empty;

		public string countstars { get; set; } = string.Empty;

		public int countreviews { get; set; }

		public string CategoryName { get; set; } = string.Empty;
		public string CategoryIcon { get; set; } = string.Empty;
	}

	public class CategoryDto
	{
		public int CategoryId { get; set; }
		public string CategoryName { get; set; } = string.Empty;
		public string CategoryIcon { get; set; } = string.Empty;
		public int ProductCount { get; set; }
	}
    public class ProductCategoryDto {
		[Key]
		public int ProductCategoryID { get; set; }

		[Required]
		[MaxLength(100)]
		public string CategoryName { get; set; } = string.Empty;

		[Required]
		public string CategoryIcon { get; set; } = string.Empty;

		public string CategoryDesc { get; set; } = string.Empty;
	}
}
