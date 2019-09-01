using UnityEngine;


namespace Snake
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "GameConfig", order = 51)]
    public class GameConfig : ScriptableObject
    {
        [Header("FieldSize")]
        [SerializeField] int fieldSizeX = 15;
        [SerializeField] int fieldSizeY = 15;
        [SerializeField] int fieldSizeZ = 15;

        [Header("Snakes")]
        
        [SerializeField][Range(1, 4)] int snakesCount = 2;
        [SerializeField] int startSegmentsCount = 3;
        [SerializeField] float step = 1f;
        [SerializeField] float stepCooldown = 0.05f;
        [SerializeField] float directionUpdateCooldown = 0.5f;

        [Header("Apples")]
        [SerializeField] float appleSpawnCooldown = 3f;


        public int FieldSizeX => fieldSizeX;
        public int FieldSizeY => fieldSizeY;
        public int FieldSizeZ => fieldSizeZ;

        public int SnakesCount => snakesCount;
        public int StartSegmentsCount => startSegmentsCount;
        public float Step => step;
        public float StepCooldown => stepCooldown;
        public float DirectionUpdateCooldown => directionUpdateCooldown;

        public float AppleSpawnCooldown => appleSpawnCooldown;
    }
}
