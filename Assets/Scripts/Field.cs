using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Snake
{
    public class Field : MonoBehaviour
    {
        #region Fields

        const float AppleSpawnCooldown = 3f;
        

        [SerializeField] Snake snakePrefab = default;
        [SerializeField] LevelObject applePrefab = default;


        LevelObject[,,] field = new LevelObject[15, 15, 15];
        List<LevelObject> apples = new List<LevelObject>();
        List<Snake> snakes = new List<Snake>();

        #endregion



        #region Unity lifecycle

        void OnEnable()
        {
            Snake.OnAppleEaten += Snake_OnAppleEaten;
            Snake.OnCollision += Snake_OnCollision;
        }


        void OnDisable()
        {
            Snake.OnAppleEaten -= Snake_OnAppleEaten;
            Snake.OnCollision -= Snake_OnCollision;
        }

       
        void Start()
        {
            Snake snake = Instantiate(snakePrefab);
            snake.Init(field, apples);
            snakes.Add(snake);

            StartCoroutine(SpawnApple());
        }

        #endregion



        #region Private methods

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


        void Snake_OnAppleEaten(PositionOnField applePosition)
        {
            for (int i = 0; i < apples.Count; i++)
            {  
                if (apples[i].PositionOnField.X == applePosition.X 
                    && apples[i].PositionOnField.Y == applePosition.Y 
                    && apples[i].PositionOnField.Z == applePosition.Z)
                {
                    Destroy(apples[i].gameObject);
                    apples.Remove(apples[i]);
                    return;
                }
            }
        }


        void Snake_OnCollision(Snake snake)
        {
            snakes.Remove(snake);

            if (snakes.Count == 0)
            {
                print("All snakes are DEAD!!!");
            }
        }

        #endregion
    }
}
