using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanonPhotoBooth
{
    public static class World
    {
        public static List<ControlBoard> Boards { get; set; }

        public static void Initialize()
        {
            Boards = new List<ControlBoard>();

            Boards.Add(new ControlBoard());
            Boards.Add(new ControlBoard());
        }
    }
}
