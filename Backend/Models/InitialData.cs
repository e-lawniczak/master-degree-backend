using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClothBackend.Models
{
    public class InitialData
    {
        public int? currentPlaytrough { get; set; }
        public int highScore { get; set; }
        public bool isControlGroup { get; set; }
        public bool canNowSaveGame { get; set; }
    }
}
