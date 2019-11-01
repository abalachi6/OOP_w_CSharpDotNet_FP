using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goforth_Howser_FinalProject
{
    class FirstQueryObjects
    {

        //mpYear, mpTitle, mpDirector, mpCast, mpPlot

        public string Year { get; set; }
        public string Title { get; set; }
        public string Director { get; set; }
        public string Cast { get; set; }
        public string Summary { get; set; }

        public FirstQueryObjects() { }

        public FirstQueryObjects(string year, string title, string director, string cast, string summary)
        {
            Year = year;
            Title = title;
            Director = director;
            Cast = cast;
            Summary = summary;
        }        




    }
}
