using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;


namespace Snake
{
    public class Snake : MonoBehaviour
    {
        enum Direction
        {
            Up = 1,
            Down = 2,
            Left = 3,
            Right = 4,
            Forward = 5,
            Backward = 6
        }


        const int StartSegmentsCount = 3;
        const float Step = 1f;
        const float StepCooldown = 0.1f;
        const float DirectionUpdateCooldown = 2f;


        [SerializeField] Segment segmentPrefab = default;
        [SerializeField] Material headMaterial = default;
        [SerializeField] Material tailMaterial = default;

        GameObject head;

        List<Segment> segments = new List<Segment>();

        Direction currentDirection = default;

        LevelObject[,,] field;

        Segment headSegment;


        public void Init(LevelObject[,,] field)
        {
            this.field = field;

            for (int i = 0; i < StartSegmentsCount; i++)
            {
                Segment newSegment = Instantiate(segmentPrefab, new Vector3(0.0f, i * Step, 0.0f), Quaternion.identity);
                segments.Add(newSegment);

                field[0, i, 0] = newSegment;
                newSegment.PositionOnField = new PositionOnField(0, i, 0);

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

                Segment tailSegment = segments.First();
                segments.Remove(tailSegment);
                field[tailSegment.PositionOnField.X, tailSegment.PositionOnField.Y, tailSegment.PositionOnField.Z] = null;

               


                CheckDirection();

                Vector3 headPosition = headSegment.transform.position;
                tailSegment.PositionOnField = headSegment.PositionOnField;

                switch (currentDirection)
                {
                    case Direction.Up:
                        tailSegment.transform.position = new Vector3(headPosition.x, headPosition.y + Step, headPosition.z);
                        tailSegment.PositionOnField.Y++;

                        break;

                    case Direction.Down:
                        tailSegment.transform.position = new Vector3(headPosition.x, headPosition.y - Step, headPosition.z);
                        tailSegment.PositionOnField.Y--;
                        break;

                    case Direction.Left:
                        tailSegment.transform.position = new Vector3(headPosition.x - Step, headPosition.y, headPosition.z);
                        tailSegment.PositionOnField.X--;
                        break;

                    case Direction.Right:
                        tailSegment.transform.position = new Vector3(headPosition.x + Step, headPosition.y, headPosition.z);
                        tailSegment.PositionOnField.X++;
                        break;

                    case Direction.Forward:
                        tailSegment.transform.position = new Vector3(headPosition.x, headPosition.y, headPosition.z + Step);
                        tailSegment.PositionOnField.Z++;
                        break;

                    case Direction.Backward:
                        tailSegment.transform.position = new Vector3(headPosition.x, headPosition.y, headPosition.z - Step);
                        tailSegment.PositionOnField.Z--;
                        break;
                }
                headSegment.SetMaterial(tailMaterial);

                segments.Add(tailSegment);
                headSegment = tailSegment;

                headSegment.SetMaterial(headMaterial);

                print($" X : {headSegment.PositionOnField.X}  Y : {headSegment.PositionOnField.Y}  Z : {headSegment.PositionOnField.Z}");
                field[headSegment.PositionOnField.X, headSegment.PositionOnField.Y, headSegment.PositionOnField.Z] = headSegment;
            }
        }


        void SetPossibleDirection()
        {
            List<Direction> possibleDirections = ((Direction[])Enum.GetValues(typeof(Direction))).ToList();

            possibleDirections.Remove(currentDirection);

            if (headSegment.PositionOnField.X == 14 || currentDirection == Direction.Left)
                possibleDirections.Remove(Direction.Right);

            if (headSegment.PositionOnField.X == 0 || currentDirection == Direction.Right)
                possibleDirections.Remove(Direction.Left);

            if (headSegment.PositionOnField.Y == 14 || currentDirection == Direction.Down)
                possibleDirections.Remove(Direction.Up);

            if (headSegment.PositionOnField.Y == 0 || currentDirection == Direction.Up)
                possibleDirections.Remove(Direction.Down);

            if (headSegment.PositionOnField.Z == 14 || currentDirection == Direction.Backward)
                possibleDirections.Remove(Direction.Forward);

            if (headSegment.PositionOnField.Z == 0 || currentDirection == Direction.Forward)
                possibleDirections.Remove(Direction.Backward);
            
            currentDirection = possibleDirections[UnityEngine.Random.Range(0, possibleDirections.Count)];
        }


        IEnumerator SetRandomDirection()
        {
            while (true)
            {
                SetPossibleDirection();
                yield return new WaitForSeconds(DirectionUpdateCooldown);
            }
        }


        void CheckDirection()
        {
            switch (currentDirection)
            {
                case Direction.Right:
                    if (headSegment.PositionOnField.X == 14)
                        SetPossibleDirection();
                    break;

                case Direction.Left:
                    if (headSegment.PositionOnField.X == 0)
                        SetPossibleDirection();
                    break;

                case Direction.Up:
                    if (headSegment.PositionOnField.Y == 14)
                        SetPossibleDirection();
                    break;

                case Direction.Down:
                    if (headSegment.PositionOnField.Y == 0)
                        SetPossibleDirection();
                    break;

                case Direction.Forward:
                    if (headSegment.PositionOnField.Z == 14)
                        SetPossibleDirection();
                    break;

                case Direction.Backward:
                    if (headSegment.PositionOnField.Z == 0)
                        SetPossibleDirection();
                    break;
            }
        }
    }
}
