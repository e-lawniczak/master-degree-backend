using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClothBackend.Models.Checkpoint
{
    public class CheckpointResponse
    {

        public int CheckpointId { get; set; }
        public int? Data { get; set; }
        public int LevelNo { get; set; }
        public float PlayerPosX { get; set; }
        public float PlayerPosY { get; set; }
        public int Health { get; set; }
        public List<int> DefeatedEnemiesIds { get; set; } = new List<int>();
        public List<int> CollectedCoinsIds { get; set; } =  new List<int>();
        public int PlaytroughId { get; set; }
        public DateTime Date { get; set; }
    }

    public class CheckpointRequest
    {

        public int CheckpointId { get; set; }
        public int? Data { get; set; }
        public int LevelNo { get; set; }
        public float PlayerPosX { get; set; }
        public float PlayerPosY { get; set; }
        public int Health { get; set; }
        public List<int> DefeatedEnemiesIds { get; set; } = new List<int>();
        public List<int> CollectedCoinsIds { get; set; } = new List<int>();
        public int PlaytroughId { get; set; }
        public DateTime Date { get; set; }
    }
}
