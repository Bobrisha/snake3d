using UnityEngine;


namespace Snake
{
    public enum LevelObjectTypes
    {
        None = 0,

        Segment = 1
    }


    public class PositionOnField
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public PositionOnField(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }


    public class LevelObject : MonoBehaviour
    {
        [SerializeField] LevelObjectTypes type = default;


        public PositionOnField PositionOnField { get; set; }

        public LevelObjectTypes Type
        {
            get => type;
        }
    }
}