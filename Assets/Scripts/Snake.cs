using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


namespace Snake
{
    public class Snake : MonoBehaviour
    {
        enum Direction
        {
            None = 0,

            Up = 1,
            Down = 2,
            Left = 3,
            Right = 4,
            Forward = 5,
            Backward = 6
        }


        const int StartSegmentsCount = 3;
        const float Step = 0.1f;
        const float StepCooldown = 0.3f;
        const float DirectionUpdateCooldown = 5f;


        [SerializeField] GameObject segmentPrefab = default;

        GameObject head;

        List<GameObject> segments = new List<GameObject>();

        Direction currentDirection = default;


        void Start()
        {
            for (int i = 0; i < StartSegmentsCount; i++)
            {
                segments.Add(Instantiate(segmentPrefab, new Vector3(0.0f, i * Step, 0.0f), Quaternion.identity));
            }

            StartCoroutine(SetRandomDirection());
            StartCoroutine(Move());
        }


        IEnumerator Move()
        {
            while (true)
            {
                yield return new WaitForSeconds(StepCooldown);

                Vector3 headPosition = segments.Last().transform.position;

                switch (currentDirection)
                {
                    case Direction.Up:
                        segments.First().transform.position = new Vector3(headPosition.x, headPosition.y + Step, headPosition.z);
                        break;

                    case Direction.Down:
                        segments.First().transform.position = new Vector3(headPosition.x, headPosition.y - Step, headPosition.z);
                        break;

                    case Direction.Left:
                        segments.First().transform.position = new Vector3(headPosition.x - Step, headPosition.y, headPosition.z);
                        break;

                    case Direction.Right:
                        segments.First().transform.position = new Vector3(headPosition.x + Step, headPosition.y, headPosition.z);
                        break;

                    case Direction.Forward:
                        segments.First().transform.position = new Vector3(headPosition.x, headPosition.y, headPosition.z + Step);
                        break;

                    case Direction.Backward:
                        segments.First().transform.position = new Vector3(headPosition.x, headPosition.y, headPosition.z - Step);
                        break;
                }

                GameObject tailSegment = segments.First();
                segments.Remove(tailSegment);
                segments.Add(tailSegment);
            }
        }


        IEnumerator SetRandomDirection()
        {
            while (true)
            {
                currentDirection = (Direction)Random.Range(1, 7);
                yield return new WaitForSeconds(DirectionUpdateCooldown);
            }
        }
    }
}
