using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClothBackend.Models
{
    public class UserDataResponse
    {
        public string userName { get; set; }
        public string email { get; set; }
        public bool isControlGroup { get; set; }
        public bool firstLogin { get; set; }
        public bool canNowSaveGame { get; set; }
        public int? currentPlaytrough { get; set; }
        public int attempts { get; set; }
        public int deaths { get; set; }
        public int highScore { get; set; }
    }
}
