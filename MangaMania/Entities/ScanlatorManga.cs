namespace MangaMania.Entities
{
    public class ScanlatorManga
    {
        public long Id { get; set; }
        public int ScanlatorId { get; set; }
        public Scanlator Scanlator { get; set; }
        public int MangaId { get; set; }
        public Manga Manga { get; set; }
    }
}
