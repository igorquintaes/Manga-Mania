using System.Collections.Generic;

namespace MangaMania.Entities
{
    public class Manga
    {
        public Manga(string name, string author)
                : this() =>
            (Name, Author) =
            (name, author);

        protected Manga() =>
            (ScanlatorManga, Chapters) = (new List<ScanlatorManga>(), new List<Chapter>());

        public int Id { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public ICollection<ScanlatorManga> ScanlatorManga { get; set; }
        public ICollection<Chapter> Chapters { get; set; }
    }
}
