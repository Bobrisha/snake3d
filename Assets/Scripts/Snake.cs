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

        public static event Action<PositionOnField> OnAppleEaten = delegate { };
        public static event Action<Snake> OnCollision = delegate { };


        [SerializeField] Segment segmentPrefab = default;
        [SerializeField] Material headMaterial = default;
        [SerializeField] Material tailMaterial = default;


        Direction currentDirection = Direction.Up;

        GameConfig config;

        Segment headSegment;
        List<Segment> segments = new List<Segment>();
        List<LevelObject> apples;

        LevelObject[,,] field;
        
        Coroutine SetRandomDirectionCoroutine;

        bool hasAppleToEat;

        #endregion



        #region Public methods

        public void Init(LevelObject[,,] field, List<LevelObject> apples, GameConfig config, PositionOnField startPosition)
        {
            this.config = config;
            this.field = field;
            this.apples = apples;

            for (int i = 0; i < config.StartSegmentsCount; i++)
            {
                Segment newSegment = Instantiate(segmentPrefab, new Vector3(startPosition.X, i, startPosition.Z) * config.Step, Quaternion.identity, transform);
                newSegment.transform.localScale = Vector3.one * config.Step;
                segments.Add(newSegment);

                field[0, i, 0] = newSegment;
                newSegment.PositionOnField = new PositionOnField(startPosition.X, i, startPosition.Z);

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
                yield return new WaitForSeconds(config.StepCooldown);

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

                
                tailSegment.PositionOnField = new PositionOnField(headSegment.PositionOnField.X, headSegment.PositionOnField.Y, headSegment.PositionOnField.Z);

                switch (currentDirection)
                {
                    case Direction.Up:
                        tailSegment.PositionOnField.Y++;
                        break;

                    case Direction.Down:
                        tailSegment.PositionOnField.Y--;
                        break;

                    case Direction.Left:
                        tailSegment.PositionOnField.X--;
                        break;

                    case Direction.Right:
                        tailSegment.PositionOnField.X++;
                        break;

                    case Direction.Forward:
                        tailSegment.PositionOnField.Z++;
                        break;

                    case Direction.Backward:
                        tailSegment.PositionOnField.Z--;
                        break;
                }

                tailSegment.transform.position = new Vector3(tailSegment.PositionOnField.X, tailSegment.PositionOnField.Y, tailSegment.PositionOnField.Z) * config.Step;

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

            if (headSegment.PositionOnField.X == config.FieldSizeX - 1 || currentDirection == Direction.Left)
                possibleDirections.Remove(Direction.Right);

            if (headSegment.PositionOnField.X == 0 || currentDirection == Direction.Right)
                possibleDirections.Remove(Direction.Left);

            if (headSegment.PositionOnField.Y == config.FieldSizeY - 1 || currentDirection == Direction.Down)
                possibleDirections.Remove(Direction.Up);

            if (headSegment.PositionOnField.Y == 0 || currentDirection == Direction.Up)
                possibleDirections.Remove(Direction.Down);

            if (headSegment.PositionOnField.Z == config.FieldSizeZ - 1 || currentDirection == Direction.Backward)
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
                yield return new WaitForSeconds(config.DirectionUpdateCooldown);
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
            
            if ((currentDirection == Direction.Right && headSegment.PositionOnField.X == config.FieldSizeX - 1)
                || (currentDirection == Direction.Left && headSegment.PositionOnField.X == 0)
                || (currentDirection == Direction.Up && headSegment.PositionOnField.Y == config.FieldSizeY - 1)
                || (currentDirection == Direction.Down && headSegment.PositionOnField.Y == 0)
                || (currentDirection == Direction.Forward && headSegment.PositionOnField.Z == config.FieldSizeZ - 1)
                || (currentDirection == Direction.Backward && headSegment.PositionOnField.Z == 0))
            {
                SetPossibleDirection();
            }
        }

        #endregion
    }
}
