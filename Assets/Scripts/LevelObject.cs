using UnityEngine;


namespace Snake
{
    public enum LevelObjectTypes
    {
        None = 0,

        Segment = 1
    }


    public class LevelObject : MonoBehaviour
    {
        [SerializeField] LevelObjectTypes type = default;


        public Vector3 PositionOnField { get; set; }

        public LevelObjectTypes Type
        {
            get => type;
        }
    }
}