using System.Collections.Generic;

namespace MangaMania.Entities
{
    public class Scanlator
    {
        public Scanlator(string name, string website)
                : this() =>
            (Name, WebSite) =
            (name, website);

        protected Scanlator() =>
            (ScanlatorManga, Chapters) = (new List<ScanlatorManga>(), new List<Chapter>());

        public int Id { get; set; }
        public string Name { get; set; }
        public string WebSite { get; set; }
        public ICollection<ScanlatorManga> ScanlatorManga { get; set; }
        public ICollection<Chapter> Chapters { get; set; }
    }
}
