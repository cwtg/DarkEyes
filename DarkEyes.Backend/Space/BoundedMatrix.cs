namespace DarkEyes.Backend.Space
{
    internal class BoundedMatrix<T>
    {
        private readonly T[] _storage;
        private readonly int _minRow;
        private readonly int _maxRow;
        private readonly int _minCol;
        private readonly int _maxCol;
        private readonly int _totalCols;

        public BoundedMatrix(int minRow, int maxRow, int minCol, int maxCol)
        {
            _minRow = minRow;
            _maxRow = maxRow;
            _minCol = minCol;
            _maxCol = maxCol;

            int totalRows = (maxRow - minRow) + 1;
            _totalCols = (maxCol - minCol) + 1;

            _storage = new T[totalRows * _totalCols];
        }

        public T Get(int row, int col)
        {
            int zeroBasedRow = row - _minRow;
            int zeroBasedCol = col - _minCol;

            int index = (zeroBasedRow * _totalCols) + zeroBasedCol;

            return _storage[index];
        }

        public void Set(int row, int col, T value)
        {
            int zeroBasedRow = row - _minRow;
            int zeroBasedCol = col - _minCol;
            int index = (zeroBasedRow * _totalCols) + zeroBasedCol;

            _storage[index] = value;
        }
    }
}
