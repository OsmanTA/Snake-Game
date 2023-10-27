using System;
using System.Collections.Generic;

namespace snakeGame
{
    public class Position
    {
        public int Row
        {
            get;
        }

        public int Column
        {
            get;
        }

        public Position(int row, int column)
        {
            Row = row;
            Column = column;
        }

        // sets new position of snake by adding/subtracting the col/row
        // based on the direction input
        public Position Translate(Direction dir)
        {
            return new Position(Row + dir.RowOffset, Column + dir.ColumnOffset);
        }

        public override bool Equals(object obj)
        {
            return obj is Position position &&
                   Row == position.Row &&
                   Column == position.Column;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Row, Column);
        }

        public static bool operator ==(Position left, Position right)
        {
            return EqualityComparer<Position>.Default.Equals(left, right);
        }

        public static bool operator !=(Position left, Position right)
        {
            return !(left == right);
        }
    }
}
