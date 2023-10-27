using System;
using System.Collections.Generic;
using System.IO;

namespace snakeGame
{
    public class gameState
    {
        public int Rows
        {
            get;
        }

        public int Columns
        {
            get;
        }

        public GridValue[,] Grid
        {
            get;
        }

        public Direction Dir
        {
            get;
            private set;
        }

        public int Score
        {
            get;
            private set;
        }

        public bool GameOver
        {
            get;
            private set;
        }
        
        // first element is the head of the snake, last element is the end of the snake
        private readonly LinkedList<Position> snakePositions = new LinkedList<Position>();

        // Used to randomly spawnn in the food
        private readonly Random random = new Random();

        // used to keep track of direction inputs to help the snake from
        // not turning on itself 180 degrees
        private readonly LinkedList<Direction> dirChanges = new LinkedList<Direction>();
        public gameState(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            Grid = new GridValue[Rows, Columns];
            Dir = Direction.Right;
            Score = 0;
            addSnake();
            addFood();
        }

        private void addSnake()
        {
            int midRows = Rows / 2;
            for (int c = 1; c <= 3; c++)
            {
                Grid[midRows, c] = GridValue.Snake;
                snakePositions.AddFirst(new Position(midRows, c));
            }
        }

        private IEnumerable<Position> EmptyPosition()
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0;  c < Columns; c++)
                {
                    if (Grid[r,c] == GridValue.Empty) 
                    {
                        yield return new Position(r, c);
                    }
                }
            }

        }
        
        private void addFood()
        {
            List<Position> empty = new List<Position>(EmptyPosition());
            if ( empty.Count == 0)
            {
                return;
            }

            Position pos = empty[random.Next(empty.Count)];
            Grid[pos.Row, pos.Column] = GridValue.Food;
        }
        
        // Gets the position of the head of the snake
        public Position HeadPosition()
        {
            return snakePositions.First.Value;
        }

        // Gets the position of the tail of the snake
        public Position TailPosition() 
        {
            return snakePositions.Last.Value;
        }

        // Gets all of the snake positions
        public IEnumerable<Position> SnakePositions()
        {
            return snakePositions;
        }

        // adds head of the snake to the front position
        private void AddHead(Position pos)
        {
            snakePositions.AddFirst(pos);
            Grid[pos.Row, pos.Column] = GridValue.Snake;
        }

        // changes position of the end of the snake
        private void RemoveTail()
        {
            Position tail = snakePositions.Last.Value;
            Grid[tail.Row, tail.Column] = GridValue.Empty;
            snakePositions.RemoveLast();
        }

        private Direction GetLastDirection()
        {
            if (dirChanges.Count == 0)
            {
                return Dir;
            }

            return dirChanges.Last.Value;
        }

        private bool CanChangeDirection(Direction newDir)
        {
            if (dirChanges.Count == 2)
            {
                return false;
            }

            Direction lastDir = GetLastDirection();
            return newDir != lastDir && newDir != lastDir.Opposite();
        }

        public void ChangeDirections(Direction direction)
        {
            // checks if change can be made
            if (CanChangeDirection(direction))
            {
                dirChanges.AddLast(direction);
            }
        }

        // checks if a value is outside of our grid
        private bool OutsideGrid(Position pos)
        {
            return pos.Row < 0 || pos.Row >= Rows || pos.Column < 0 || pos.Column >= Columns;
        }

        private GridValue WillHit(Position NewHead)
        {
            if (OutsideGrid(NewHead))
            {
                return GridValue.Outside;
            }

            // if where the new head is going to be is the same place
            // as the current tail, the game does not end because the
            // tail will move to create space as the head moves
            if (NewHead == TailPosition())
            {
                return GridValue.Empty;
            }
            return Grid[NewHead.Row, NewHead.Column];
        }

        public void Move()
        {
            if (dirChanges.Count > 0)
            {
                Dir = dirChanges.First.Value;
                dirChanges.RemoveFirst();
            }
            Position newHead = HeadPosition().Translate(Dir);
            GridValue hit = WillHit(newHead);

            if (hit == GridValue.Outside || hit == GridValue.Snake)
            {
                GameOver = true;
            } else if(hit == GridValue.Empty)
            {
                RemoveTail();
                AddHead(newHead);
            } else if (hit == GridValue.Food)
            {
                AddHead(newHead);
                Score++;
                addFood();
            }           
        }
    }
}
