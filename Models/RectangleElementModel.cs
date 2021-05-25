using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Eatsnake.Models
{
    public class RectangleElementModel
    {
        public int X { get; set; }

        public int Y { get; set; }

        public Rectangle Rectangle { get; set; }

        public Brush LastBrush { get; set; }
    }
}
