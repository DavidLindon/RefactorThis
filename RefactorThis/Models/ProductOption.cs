namespace RefactorThis.Models
{
	public class ProductOption
	{
		public Guid Id { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }
		public virtual Product Product { get; set; }
	}
}
