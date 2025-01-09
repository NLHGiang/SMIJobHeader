namespace SMIJobXml.Entities
{
    public class Template
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Contents { get; set; }
        public int? Order { get; set; }
        public string? ImageView { get; set; }

        public bool? Status { get; set; }
    }
}
