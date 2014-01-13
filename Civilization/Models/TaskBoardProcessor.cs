using System;
using System.Threading.Tasks;


namespace Civilization.Models
{
    public class TaskBoardProcessor
    {
        private Task[] _tasks;
        private int _tasksCount;
        private int _width;
        private int _height;
        private int _blockSize;
        private int _lastBlockSize;
        private Random[] _rands;
        private bool _divideHorizontaly;

        protected TaskBoardProcessor()
        {}

        public static TaskBoardProcessor CreateNTasksForBoard(int n, int height, int width)
        {
            var taskBoardProcessor = new TaskBoardProcessor();

            taskBoardProcessor.Initialize(n, height, width);
            taskBoardProcessor.DetermineCalculationParams();

            return taskBoardProcessor;
        }

        private void Initialize(int tasksCount, int height, int width)
        {
            _tasksCount = tasksCount;
            _width = width;
            _height = height;
            _tasks = new Task[tasksCount];
            _rands = new Random[tasksCount];
            for (int i = 0; i < tasksCount; i++) _rands[i] = new Random();
        }

        private void DetermineCalculationParams()
        {
            _divideHorizontaly = _height > _width;
            CountBlockSize(_divideHorizontaly ? _height : _width);
        }

        private void CountBlockSize(int param)
        {
            _blockSize = param/_tasksCount;
            _lastBlockSize = param % _tasksCount == 0 ? _blockSize : param % _tasksCount;
        }

        public void StartProcessing(Action<int, int, int, int, Random> processor)
        {
            int i;
            for (i = 0; i < _tasksCount - 1; ++i)
            {
                CreateNthTask(processor, i);
            }
            CreateLastTask(processor);
        }


        private void CreateNthTask(Action<int, int, int, int, Random> processor, int index)
        {
            if (_divideHorizontaly)
                _tasks[index] = Task.Run(() => processor(0, _width, index*_blockSize, (index + 1)*_blockSize, _rands[index]));
            else
                _tasks[index] = Task.Run(() => processor(index*_blockSize, (index + 1)*_blockSize, 0, _height, _rands[index]));
        }

        private void CreateLastTask(Action<int, int, int, int, Random> processor)
        {
            int ind = _tasksCount - 1;
            if (_divideHorizontaly)
                _tasks[ind] = Task.Run(() => processor(0, _width, ind * _blockSize, ind * _blockSize + _lastBlockSize, _rands[ind]));
            else
                _tasks[ind] = Task.Run(() => processor(ind * _blockSize, ind * _blockSize + _lastBlockSize, 0, _width, _rands[ind]));
        }

        public void WaitForEnd()
        {
            Task.WaitAll(_tasks);
        }
    }
}
