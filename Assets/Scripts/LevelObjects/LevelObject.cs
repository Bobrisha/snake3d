using UnityEngine;


namespace Snake
{
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
