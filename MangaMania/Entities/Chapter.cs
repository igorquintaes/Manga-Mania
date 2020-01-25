namespace MangaMania.Entities
{
    public class Chapter
    {
        public Chapter(string number, string name, Manga manga, Scanlator scanlator)
                : this() =>
            (Number, Name, Manga, Scanlator) =
            (number, name, manga, scanlator);

        protected Chapter()
        { }

        public long Id { get; set; }
        public string Number { get; set; }
        public string Name { get; set; }
        public Manga Manga { get; set; }
        public int MangaId { get; set; }
        public Scanlator Scanlator { get; set; }
        public int ScanlatorId { get; set; }
    }
}
