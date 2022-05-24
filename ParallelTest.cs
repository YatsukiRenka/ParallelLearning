using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelTest
{
    public class ParallelTest
    {
        private static void Main(string[] args)
        {
            ParallelTest test = new ParallelTest();
            test.Run();
            Console.ReadLine();
        }

        public void Run()
        {
            TimeSpan totalDuration = TimeSpan.FromSeconds(100);//总时长
            DateTime estimatedEndTime = DateTime.Now + totalDuration;//结束时间

            double Rise_Time_s = 200;//升(s)
            double Fall_Time_s = 160;//降(s)
            double Step_Time_ms = 1000;//step(ms)
            double Timeout_s = 10;//超时(s)
            double samplingInterval_s = 1;//间隔(10*100ms)

            Console.WriteLine($"Start Processing at [{DateTime.Now}]");
            Stopwatch sw = Stopwatch.StartNew();

            Parallel.Invoke(
                () => Collecting(samplingInterval_s, estimatedEndTime),
                () =>
                {
                    while (DateTime.Now < estimatedEndTime)
                    {
                        Rise(Step_Time_ms, estimatedEndTime);

                        #region timeout check

                        Console.WriteLine();
                        Console.WriteLine($"Start Timeout Check...[{DateTime.Now}]");
                        Console.WriteLine($"Total running time: [{sw.ElapsedMilliseconds}]");
                        Console.WriteLine();

                        Stopwatch swTimeout = Stopwatch.StartNew();
                        bool isTimeoutBreak = false;

                        for (int i = 0; i < Timeout_s * 10; i++)
                        {
                            if (DateTime.Now >= estimatedEndTime)
                            {
                                isTimeoutBreak = true;
                                Console.WriteLine($"Process end at [{DateTime.Now}] for [{sw.ElapsedMilliseconds}]ms");
                                break;
                            }
                            Thread.Sleep(1000);
                            Console.WriteLine($"Timeout Check is running...[{swTimeout.ElapsedMilliseconds}]ms");
                        }

                        Console.WriteLine();
                        Console.WriteLine($"Timeout Check done at [{DateTime.Now}] for [{swTimeout.ElapsedMilliseconds}]ms");
                        swTimeout.Stop();

                        if (isTimeoutBreak) { }
                        else
                        {
                            if (DateTime.Now >= estimatedEndTime)
                            {
                                Console.WriteLine($"Process end at [{DateTime.Now}] for [{sw.ElapsedMilliseconds}]ms");
                            }
                        }

                        #endregion timeout check

                        Thread.Sleep((int)(Rise_Time_s - Timeout_s) * 1000);

                        Fall(Step_Time_ms, estimatedEndTime);

                        #region timeout check secondtime

                        Console.WriteLine();
                        Console.WriteLine($"Start Timeout Check...[{DateTime.Now}]");
                        Console.WriteLine($"Total running time: [{sw.ElapsedMilliseconds}]");
                        Console.WriteLine();

                        swTimeout.Reset();
                        swTimeout.Start();
                        isTimeoutBreak = false;

                        for (int i = 0; i < Timeout_s * 10; i++)
                        {
                            if (DateTime.Now >= estimatedEndTime)
                            {
                                isTimeoutBreak = true;
                                Console.WriteLine($"Process end at [{DateTime.Now}] for [{sw.ElapsedMilliseconds}]ms");
                                break;
                            }
                            Thread.Sleep(1000);
                            Console.WriteLine($"Timeout Check is running...[{swTimeout.ElapsedMilliseconds}]ms");
                        }

                        Console.WriteLine();
                        Console.WriteLine($"Timeout Check done at [{DateTime.Now}] for [{swTimeout.ElapsedMilliseconds}]ms");
                        swTimeout.Stop();

                        if (isTimeoutBreak) { }
                        else
                        {
                            if (DateTime.Now >= estimatedEndTime)
                            {
                                Console.WriteLine($"Process end at [{DateTime.Now}] for [{sw.ElapsedMilliseconds}]ms");
                            }
                        }

                        #endregion timeout check secondtime

                        Thread.Sleep((int)(Fall_Time_s - Timeout_s) * 1000);
                    }
                });
        }

        public void Rise(double stepTime, DateTime endTime)
        {
            Console.WriteLine($"Start rising...[{DateTime.Now}]");
            Console.WriteLine();
            Stopwatch swRise = Stopwatch.StartNew();

            for (int i = 0; i < 10; i++)
            {
                if (DateTime.Now >= endTime)
                {
                    Console.WriteLine($"Process end at [{DateTime.Now}]");
                    break;
                }
                Thread.Sleep((int)(stepTime));
                Console.WriteLine($"Rising Step[{i}]...[{swRise.ElapsedMilliseconds}]ms]");
            }

            Console.WriteLine();
            Console.WriteLine($"Rising finished...[{swRise.ElapsedMilliseconds}]ms");
            Console.WriteLine();
        }

        public void Fall(double Step_Time_ms, DateTime estimatedEndTime)
        {
            Console.WriteLine($"Start falling...[{DateTime.Now}]");
            Console.WriteLine();
            Stopwatch swFall = Stopwatch.StartNew();

            for (int i = 0; i < 10; i++)
            {
                if (DateTime.Now >= estimatedEndTime)
                {
                    Console.WriteLine($"Process end at [{DateTime.Now}]");
                    break;
                }
                Thread.Sleep((int)(Step_Time_ms));
                Console.WriteLine($"Falling Step[{i}]...[{swFall.ElapsedMilliseconds}]ms]");
            }

            Console.WriteLine();
            Console.WriteLine($"Falling finished...[{swFall.ElapsedMilliseconds}]ms");
            Console.WriteLine();
        }

        public void TimeoutCheck(Stopwatch sw, double Timeout_s, DateTime estimatedEndTime)
        {
            Console.WriteLine();
            Console.WriteLine($"Start Timeout Check...[{DateTime.Now}]");
            Console.WriteLine($"Total running time: [{sw.ElapsedMilliseconds}]");
            Console.WriteLine();

            Stopwatch swTimeout = Stopwatch.StartNew();
            bool isBreak = false;

            for (int i = 0; i < Timeout_s * 10; i++)
            {
                if (DateTime.Now >= estimatedEndTime)
                {
                    isBreak = true;
                    Console.WriteLine($"Process end at [{DateTime.Now}] for [{sw.ElapsedMilliseconds}]ms");
                    break;
                }
                Thread.Sleep(1000);
                Console.WriteLine($"Timeout Check is running...[{swTimeout.ElapsedMilliseconds}]ms");
            }

            if (isBreak) { }
            else
            {
                if (DateTime.Now >= estimatedEndTime)
                {
                    Console.WriteLine($"Process end at [{DateTime.Now}] for [{sw.ElapsedMilliseconds}]ms");
                }
            }
        }

        public void Collecting(double samplingInterval_s, DateTime estimatedEndTime)
        {
            Stopwatch swCollect = Stopwatch.StartNew();
            bool isBreak = false;

            while (DateTime.Now < estimatedEndTime)
            {
                for (int i = 0; i < samplingInterval_s; i++)
                {
                    if (DateTime.Now >= estimatedEndTime)
                    {
                        isBreak = true;
                        Console.WriteLine($"Process end at [{DateTime.Now}]");
                        break;
                    }
                    Thread.Sleep(1000);
                    Console.WriteLine($"Collecting... [{swCollect.ElapsedMilliseconds}]ms");
                }
            }

            if (isBreak) { }
            else
            {
                if (DateTime.Now >= estimatedEndTime)
                {
                    Console.WriteLine($"Process end at [{DateTime.Now}]");
                }
            }
        }
    }
}
