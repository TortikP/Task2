using System;
using System.Threading;
using System.Threading.Tasks;

namespace Task2
{
    static class Server
    {

        static int count;

        static ReaderWriterLockSlim rwl = new ReaderWriterLockSlim();

        const int numThreads = 26;

        static void Main(string[] args)
        {
            Random rnd = new Random();
            Thread[] t = new Thread[numThreads];
            Parallel.For(0,numThreads, i => {
                if (i % 2 == 0)
                {
                    t[i] = new Thread(() => WriteMany(10, 50));
                }
                else
                {
                    t[i] = new Thread(() => ReadMany(50));
                }
                t[i].Name = new String((char)(i + 65), 1);
                t[i].Start();
            });
            

            for (int i = 0; i < numThreads; i++)
                t[i].Join();

            Console.ReadLine();
        }

        static void ReadMany(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                GetCount();
            }
        }

        static void WriteMany(int value, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                AddToCount(value);
            }
        }

        static int GetCount()
        {
            rwl.EnterReadLock();
            try
            {
                // It is safe for this thread to read from the shared resource.
                Display("Читатель считал " + count);
                return count;
            }
            finally
            {
                // Ensure that the lock is released.
                rwl.ExitReadLock();
            }
        }

        static void AddToCount(int value)
        {
            rwl.EnterWriteLock();
            try
            {
                // It's safe for this thread to access from the shared resource.
                count += value;
                Display("Писатель добавил " + count);
            }
            finally
            {
                // Ensure that the lock is released.
                rwl.ExitWriteLock();
            }

        }

        static void Display(string message)
        {
            Console.Write("Поток/Клиент {0} {1}. \n", Thread.CurrentThread.Name, message);
        }
    }
}
