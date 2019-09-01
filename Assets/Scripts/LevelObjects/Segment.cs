using UnityEngine;


namespace Snake
{
    public class Segment : LevelObject
    {
        [SerializeField] MeshRenderer meshRenderer = default;
        

        public void SetMaterial(Material material) => meshRenderer.material = material;
    }
}
