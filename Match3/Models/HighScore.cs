using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Match3.Models
{
    public class HighScore
    {
        public int Id { get; set; }
        public int Score { get; set; }
        public Player Player { get; set; }
    }
}
