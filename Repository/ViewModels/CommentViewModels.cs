using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class CommentViewModels
    {
        public string storeName  { get; set; }
        public string Username { get; set; }
        public string? Cmt { get; set; }
        public DateTime Datecmt { get; set; } = DateTime.Now;

        public string? Relay { get; set; }
        public DateTime? DateRelay { get; set; } = DateTime.Now;
        public bool Status { get; set; } = false;
        public int Rating { get; set; } = 5;
    }
}
