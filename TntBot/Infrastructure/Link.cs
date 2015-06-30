using System;
using System.Text;

namespace TBot.Infrastructure
{
    public class Link : IComparable<Link>
    {
        public string Href { get; set; }

        public string Name { get; set; }

        public Link()
        {
            Href = string.Empty;
            Name = string.Empty;
        }

        public Link(string name, string href)
        {
            if (string.IsNullOrWhiteSpace(name))
                this.Name = string.Empty;
            else
            {
                var txt = new StringBuilder(name);
                txt.Replace("Ã³", "ó");
                txt.Replace("Ã­", "í");
                txt.Replace("&#039;", "'");
                txt.Replace("Ã±", "ñ");
                txt.Replace("Ã¡", "á");
                txt.Replace("&amp;", "&");
                txt.Replace("Ãº", "ú");

                this.Name = txt.ToString().Trim();
            }

            this.Href = href.Trim();
        }

        public int CompareTo(Link other)
        {
            if (other == null)
                throw new ArgumentException("other");

            return Href.CompareTo(other);
        }

        public override string ToString()
        {
            return string.Format("A[{0},{1}]", Name, Href);
        }
    }
}