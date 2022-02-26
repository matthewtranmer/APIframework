using System;

namespace DataStructures
{
    class CircularQueue<T>
    {
        T[] array;
        int front = -1;
        int back = -1;
        int size;

        public int total_elements
        {
            get
            {
                if (is_empty)
                {
                    return 0;
                }

                return ((size - front + back) % size) + 1;
            }
        }

        public bool is_full
        {
            get { return (back + 1) % size == front; }
        }

        public bool is_empty
        {
            get { return front == -1 && back == -1; }
        }

        public CircularQueue(int size)
        {
            this.size = size;
            array = new T[size];
        }

        public void enQueue(T item)
        {
            if (is_full)
            {
                throw new Exception("Queue is full");
            }

            if (is_empty)
            {
                front = 0;
                back = 0;
                array[back] = item;

                return;
            }

            back = ++back % size;
            array[back] = item;
        }

        public T deQueue()
        {
            T item = array[front];
            if (front == back)
            {
                front = -1;
                back = -1;

                return item;
            }

            front = ++front % size;
            return item;
        }

        internal void debug_display()
        {
            Console.WriteLine("Front: " + front);
            Console.WriteLine("Back: " + back);

            foreach (T item in array)
            {
                Console.Write(item + ", ");
            }
            Console.WriteLine();
        }
    }
}