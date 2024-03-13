namespace RefactorThis.Models
{
	public class ProductOptionDTO
	{
		public Guid Id { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }
		public ProductOptionDTO() { 
		
		}
		public ProductOptionDTO(ProductOption productOption)
		{
			Id = productOption.Id;
			Name = productOption.Name;
			Description = productOption.Description;
		}
	}
}
