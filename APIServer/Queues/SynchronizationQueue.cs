namespace DataStructures
{
    class SynchronizationQueue<T>
    {
        SemaphoreSlim gate;
        CircularQueue<T> queue;
        int size;

        public SynchronizationQueue(int size)
        {
            this.size = size;
            gate = new SemaphoreSlim(0, size);
            queue = new CircularQueue<T>(size);
        }

        public bool isFull()
        {
            if (queue.total_elements == size)
            {
                return true;
            }
            return false;
        }

        public void enQueue(T item)
        {
            queue.enQueue(item);
            gate.Release();
        }

        public async Task<T> deQueue()
        {
            await gate.WaitAsync();
            return queue.deQueue();
        }
        public T deQueue_debug()
        {
            gate.Wait();
            return queue.deQueue();
        }
    }
}
