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
        const float Step = 1f;
        const float StepCooldown = 0.3f;
        const float DirectionUpdateCooldown = 5f;


        [SerializeField] LevelObject segmentPrefab = default;

        GameObject head;

        List<LevelObject> segments = new List<LevelObject>();

        Direction currentDirection = default;

        LevelObject[,,] field;

        Vector3 headPositionOnField;

        LevelObject headSegment;


        public void Init(LevelObject[,,] field)
        {
            this.field = field;

            for (int i = 0; i < StartSegmentsCount; i++)
            {
                LevelObject newSegment = Instantiate(segmentPrefab, new Vector3(0.0f, i * Step, 0.0f), Quaternion.identity);
                segments.Add(newSegment);

                field[i, 0, 0] = newSegment;
                newSegment.PositionOnField = new Vector3(i, 0, 0);

                headSegment = newSegment;
            }

            StartCoroutine(SetRandomDirection());
            StartCoroutine(Move());
        }


        IEnumerator Move()
        {
            while (true)
            {
                yield return new WaitForSeconds(StepCooldown);

                LevelObject tailSegment = segments.First();
                segments.Remove(tailSegment);
                field[(int)tailSegment.PositionOnField.x, (int)tailSegment.PositionOnField.y, (int)tailSegment.PositionOnField.z] = null;

                Vector3 headPosition = segments.Last().transform.position;

                switch (currentDirection)
                {
                    case Direction.Up:
                        tailSegment.transform.position = new Vector3(headPosition.x, headPosition.y + Step, headPosition.z);
                        tailSegment.PositionOnField = headPosition + Vector3.up;

                        break;

                    case Direction.Down:
                        tailSegment.transform.position = new Vector3(headPosition.x, headPosition.y - Step, headPosition.z);
                        tailSegment.PositionOnField = headPosition + Vector3.down;
                        break;

                    case Direction.Left:
                        tailSegment.transform.position = new Vector3(headPosition.x - Step, headPosition.y, headPosition.z);
                        tailSegment.PositionOnField = headPosition + Vector3.left;
                        break;

                    case Direction.Right:
                        tailSegment.transform.position = new Vector3(headPosition.x + Step, headPosition.y, headPosition.z);
                        tailSegment.PositionOnField = headPosition + Vector3.right;
                        break;

                    case Direction.Forward:
                        tailSegment.transform.position = new Vector3(headPosition.x, headPosition.y, headPosition.z + Step);
                        tailSegment.PositionOnField = headPosition + Vector3.forward;
                        break;

                    case Direction.Backward:
                        tailSegment.transform.position = new Vector3(headPosition.x, headPosition.y, headPosition.z - Step);
                        tailSegment.PositionOnField = headPosition + Vector3.back;
                        break;
                }

                segments.Add(tailSegment);
                field[(int)tailSegment.PositionOnField.x, (int)tailSegment.PositionOnField.y, (int)tailSegment.PositionOnField.z] = tailSegment;
            }
        }


        IEnumerator SetRandomDirection()
        {
            while (true)
            {
                currentDirection = (Direction)UnityEngine.Random.Range(1, 7);
                yield return new WaitForSeconds(DirectionUpdateCooldown);
            }
        }
    }
}
