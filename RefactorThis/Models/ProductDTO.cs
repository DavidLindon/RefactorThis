namespace RefactorThis.Models {
	public class ProductDTO {
		public Guid Id { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public decimal Price { get; set; }

		public decimal DeliveryPrice { get; set; }
		public ProductDTO(Product product) { 
			Id = product.Id;
			Name = product.Name;
			Description = product.Description;
			Price = product.Price;
			DeliveryPrice = product.DeliveryPrice;
		}
	}
}
