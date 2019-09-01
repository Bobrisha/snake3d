using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Snake
{
    public class Field : MonoBehaviour
    {
        const float AppleSpawnCooldown = 3f;
        

        [SerializeField] Snake snakePrefab = default;
        [SerializeField] LevelObject applePrefab = default;

        LevelObject[,,] field = new LevelObject[15, 15, 15];

        List<LevelObject> apples = new List<LevelObject>();


        void OnEnable()
        {
            Snake.OnAppleEaten += Snake_OnAppleEaten;
        }


        void OnDisable()
        {
            Snake.OnAppleEaten -= Snake_OnAppleEaten;
        }

       
        void Start()
        {
            Instantiate(snakePrefab).Init(field, apples);
            StartCoroutine(SpawnApple());
        }


        IEnumerator SpawnApple()
        {
            while (true)
            {
                yield return new WaitForSeconds(AppleSpawnCooldown);

                int x = Random.Range(0, 15);
                int y = Random.Range(0, 15);
                int z = Random.Range(0, 15);

                while (field[x, y, z] != null)
                {
                    x = Random.Range(0, 15);
                    y = Random.Range(0, 15);
                    z = Random.Range(0, 15);
                }

                LevelObject apple = Instantiate(applePrefab, new Vector3(x, y, z), Quaternion.identity);
                apple.PositionOnField = new PositionOnField(x, y, z);
                field[x, y, z] = apple;

                apples.Add(apple);
            }
        }


        void Snake_OnAppleEaten(LevelObject apple)
        {
            apples.Remove(apple);
            Destroy(apple.gameObject);
        }
    }
}