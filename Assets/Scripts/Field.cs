using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Snake
{
    public class Field : MonoBehaviour
    {
        [SerializeField] Snake SnakePrefab = default;


        LevelObject[,,] field = new LevelObject[15, 15, 15];

       
        private void Start()
        {
            Instantiate(SnakePrefab).Init(field);
        }
    }
}