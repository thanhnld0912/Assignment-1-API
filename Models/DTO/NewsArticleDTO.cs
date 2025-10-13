namespace Models.DTO
{
    public class NewsArticleDTO
    {
        public string NewsArticleId { get; set; }
        public string NewsTitle { get; set; }
        public string Headline { get; set; }
        public string? NewsContent { get; set; }
        public string? NewsSource { get; set; }
        public short CategoryId { get; set; }
        public bool NewsStatus { get; set; }

    }

}
