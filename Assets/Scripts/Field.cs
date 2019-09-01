using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Snake
{
    public class Field : MonoBehaviour
    {
        #region Fields

        [SerializeField] GameConfig config = default;
        [SerializeField] Snake snakePrefab = default;
        [SerializeField] LevelObject applePrefab = default;
        [SerializeField] GameObject cagePrefab = default;
        
        
        LevelObject[,,] field;
        PositionOnField[] startSnakesPositions;

        List<LevelObject> apples = new List<LevelObject>();
        List<Snake> snakes = new List<Snake>();

        #endregion



        #region Unity lifecycle
        
        void Awake()
        {
            field = new LevelObject[config.FieldSizeX, config.FieldSizeY, config.FieldSizeZ];

            GameObject cage = Instantiate(cagePrefab, transform);
            cage.transform.position = new Vector3(config.FieldSizeX - 1, config.FieldSizeY - 1, config.FieldSizeZ - 1) / 2.0f * config.Step;
            cage.transform.localScale = new Vector3(config.FieldSizeX, config.FieldSizeY, config.FieldSizeZ) * config.Step * 1.001f;

            startSnakesPositions = new PositionOnField[]
            {
                new PositionOnField(0, 0, 0),
                new PositionOnField(config.FieldSizeX - 1, 0, 0),
                new PositionOnField(0, 0, config.FieldSizeZ - 1),
                new PositionOnField(config.FieldSizeX - 1, 0, config.FieldSizeZ - 1)
            };

            StartGame();
        }


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

        #endregion



        #region Private methods

        void StartGame()
        {
            for (int i = 0; i < config.SnakesCount; i++)
            {
                Snake snake = Instantiate(snakePrefab, transform);
                snake.Init(field, apples, config, startSnakesPositions[i]);
                snakes.Add(snake);
            }

            StartCoroutine(SpawnApple());
        }


        IEnumerator SpawnApple()
        {
            while (true)
            {
                yield return new WaitForSeconds(config.AppleSpawnCooldown);

                int x = Random.Range(0, config.FieldSizeX);
                int y = Random.Range(0, config.FieldSizeY);
                int z = Random.Range(0, config.FieldSizeZ);

                while (field[x, y, z] != null)
                {
                    x = Random.Range(0, config.FieldSizeX);
                    y = Random.Range(0, config.FieldSizeY);
                    z = Random.Range(0, config.FieldSizeZ);
                }

                LevelObject apple = Instantiate(applePrefab, transform);
                apple.PositionOnField = new PositionOnField(x, y, z);
                apple.transform.position = new Vector3(apple.PositionOnField.X, apple.PositionOnField.Y, apple.PositionOnField.Z) * config.Step;
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

            if (snakes.Count != 0) return;

            print("All snakes are DEAD!!!");

            for (int i = 0; i < apples.Count; i++)
            {
                Destroy(apples[i].gameObject);
            }
            apples.Clear();

            print("Here we go again)");
            StartGame();
        }

        #endregion
    }
}
