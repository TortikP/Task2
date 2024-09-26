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

        const int dataReadTime = 100; //Условное время на чтение

        const int dataWriteTime = 100; //Условное время на запись

        static void Main(string[] args)
        {
            Random rnd = new Random();
            Thread[] thread = new Thread[numThreads];
            Parallel.For(0,numThreads, i => {
                if (i % 2 == 0)
                {
                    thread[i] = new Thread(() => WriteMany(10, 1));
                }
                else
                {
                    thread[i] = new Thread(() => ReadMany(3));
                }
                thread[i].Name = new String((char)(i + 65), 1);
                thread[i].Start();
            });
            

            for (int i = 0; i < numThreads; i++)
                thread[i].Join();

            Console.ReadLine();
        }

        //Прочитать данные несколько раз
        static void ReadMany(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                GetCount();
            }
        }

        //Записать данные несколько раз
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
                Thread.Sleep(dataReadTime);
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
                Thread.Sleep(dataWriteTime);
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
