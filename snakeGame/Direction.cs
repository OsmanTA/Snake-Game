using System;
using System.Collections.Generic;

namespace snakeGame
{
    public class Direction
    {
        // direction of snake. With top left corner being (0,0)
        public readonly static Direction Left = new Direction(0, -1);
        public readonly static Direction Right = new Direction(0, 1);
        public readonly static Direction Up = new Direction(-1, 0);
        public readonly static Direction Down = new Direction(1, 0);

        public int RowOffset
        {
            get;
        }
        
        public int ColumnOffset
        {
            get;
        }

        private Direction(int row, int column)
        {
            RowOffset = row;
            ColumnOffset = column;
        }

        public Direction Opposite()
        {
            return new Direction(-RowOffset, -ColumnOffset);
        }

        public override bool Equals(object obj)
        {
            return obj is Direction direction &&
                   RowOffset == direction.RowOffset &&
                   ColumnOffset == direction.ColumnOffset;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(RowOffset, ColumnOffset);
        }

        public static bool operator ==(Direction left, Direction right)
        {
            return EqualityComparer<Direction>.Default.Equals(left, right);
        }

        public static bool operator !=(Direction left, Direction right)
        {
            return !(left == right);
        }
    }
}
