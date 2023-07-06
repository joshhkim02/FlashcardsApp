using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashcardsApp
{
    internal class Flashcard
    {
        public int CardID { get; set; }
        public int StackID { get; set; }
        public string FrontDesc { get; set; }
        public string BackDesc { get; set; }
    }
}
