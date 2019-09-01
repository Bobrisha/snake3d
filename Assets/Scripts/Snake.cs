using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;


namespace Snake
{
    public class Snake : MonoBehaviour
    {
        #region Fields

        const int StartSegmentsCount = 3;
        const float Step = 1f;
        const float StepCooldown = 0.05f;
        const float DirectionUpdateCooldown = 0.5f;


        public static event Action<PositionOnField> OnAppleEaten = delegate { };
        public static event Action<Snake> OnCollision = delegate { };


        [SerializeField] Segment segmentPrefab = default;
        [SerializeField] Material headMaterial = default;
        [SerializeField] Material tailMaterial = default;


        Direction currentDirection = Direction.Up;

        Segment headSegment;
        List<Segment> segments = new List<Segment>();
        List<LevelObject> apples;

        LevelObject[,,] field;
        
        Coroutine SetRandomDirectionCoroutine;

        bool hasAppleToEat;

        #endregion



        #region Public methods

        public void Init(LevelObject[,,] field, List<LevelObject> apples)
        {
            this.field = field;
            this.apples = apples;

            for (int i = 0; i < StartSegmentsCount; i++)
            {
                Segment newSegment = Instantiate(segmentPrefab, new Vector3(0.0f, i * Step, 0.0f), Quaternion.identity, transform);
                segments.Add(newSegment);

                field[0, i, 0] = newSegment;
                newSegment.PositionOnField = new PositionOnField(0, i, 0);

                headSegment = newSegment;
            }
            

            SetRandomDirectionCoroutine = StartCoroutine(SetRandomDirection());
            StartCoroutine(Move());
        }

        #endregion



        #region Private methods

        IEnumerator Move()
        {
            while (true)
            {
                yield return new WaitForSeconds(StepCooldown);

                Segment tailSegment = segments.First();

                if (hasAppleToEat)
                {
                    hasAppleToEat = false;
                    
                    SetRandomDirectionCoroutine = StartCoroutine(SetRandomDirection());
                    tailSegment = Instantiate(segmentPrefab, transform);
                }
                else
                {
                    field[tailSegment.PositionOnField.X, tailSegment.PositionOnField.Y, tailSegment.PositionOnField.Z] = null;
                    segments.Remove(tailSegment);
                }

                CheckDirection();


                Vector3 headPosition = headSegment.transform.position;
                tailSegment.PositionOnField = new PositionOnField(headSegment.PositionOnField.X, headSegment.PositionOnField.Y, headSegment.PositionOnField.Z); ;

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
                tailSegment.SetMaterial(headMaterial);
                segments.Add(tailSegment);

                LevelObject newHeadFieldPoint = field[tailSegment.PositionOnField.X, tailSegment.PositionOnField.Y, tailSegment.PositionOnField.Z];

                if (newHeadFieldPoint != null && newHeadFieldPoint.Type == LevelObjectTypes.Segment)
                {
                    OnCollision(this);
                    segments.Clear();
                    Destroy(gameObject);
                }

                if (newHeadFieldPoint != null && newHeadFieldPoint.Type == LevelObjectTypes.Apple)
                {
                    OnAppleEaten(tailSegment.PositionOnField);
                    hasAppleToEat = true;
                }
                

                field[tailSegment.PositionOnField.X, tailSegment.PositionOnField.Y, tailSegment.PositionOnField.Z] = tailSegment;

                headSegment = tailSegment;
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
            bool isSnakeHasTarget = false;

            for (int i = 0; i < apples.Count; i++)
            {
                if (apples[i].PositionOnField.X == headSegment.PositionOnField.X && apples[i].PositionOnField.Y == headSegment.PositionOnField.Y)
                {
                    currentDirection = apples[i].PositionOnField.Z > headSegment.PositionOnField.Z ? Direction.Forward : Direction.Backward;
                    isSnakeHasTarget = true;
                }

                if (apples[i].PositionOnField.Y == headSegment.PositionOnField.Y && apples[i].PositionOnField.Z == headSegment.PositionOnField.Z)
                {
                    currentDirection = apples[i].PositionOnField.X > headSegment.PositionOnField.X ? Direction.Right : Direction.Left;
                    isSnakeHasTarget = true;
                }

                if (apples[i].PositionOnField.Z == headSegment.PositionOnField.Z && apples[i].PositionOnField.X == headSegment.PositionOnField.X)
                {
                    currentDirection = apples[i].PositionOnField.Y > headSegment.PositionOnField.Y ? Direction.Up : Direction.Down;
                    isSnakeHasTarget = true;
                }
            }

            if (isSnakeHasTarget)
            {
                StopCoroutine(SetRandomDirectionCoroutine);
                return;
            }
      
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

        #endregion
    }
}
