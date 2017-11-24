using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MagazinesWeb.Models
{
    public class Magazine
    {
        public int Id { get; set; }
        public int? LP { get; set; }
        public string Title { get; set; }
        public string ISSN { get; set; }
        public string EISSN { get; set; }
        public string Year { get; set; }
        public int Points { get; set; }
        public string List { get; set; }
    }
}
