using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelTest
{
    public class ParallelTest
    {
        private static void Main()
        {
            ParallelTest test = new ParallelTest();
            test.Run();
            Console.ReadLine();
        }

        //task1: 采集数据
        //task2: while(DateTime.Now < estimatedEndTime)
        //       {rise => 检测超时 => 运行剩余rise时间 => fall => 检测超时 => 运行剩余fall时间}
        //时间到，若在rise阶段内，进行fall动作退出，反之则直接退出
        public void Run()
        {
            TimeSpan totalDuration = TimeSpan.FromSeconds(61);//总时长
            DateTime estimatedEndTime = DateTime.Now + totalDuration;//结束时间

            double Rise_Time_s = 20;//升(s)
            double Fall_Time_s = 16;//降(s)
            double Step_Time_ms = 500;//step(ms) 升降总时长均为5s
            double Timeout_s = 5;//超时(s)
            double samplingInterval_s = 1;//间隔(10*100ms)
            bool isBreak = false;

            Console.WriteLine($"Start Processing at [{DateTime.Now}]");
            Stopwatch sw = Stopwatch.StartNew();

            Parallel.Invoke(
                //全程采集数据
                () => Collecting(samplingInterval_s, estimatedEndTime),
                () =>
                {
                    isBreak = false;

                    while (DateTime.Now < estimatedEndTime)
                    {
                        #region rise过程

                        Rise(Step_Time_ms);
                        if (DateTime.Now >= estimatedEndTime)
                        {
                            Fall(Step_Time_ms);
                            break;
                        }
                        if (TimeoutCheck(new Stopwatch(), Step_Time_ms, Timeout_s, estimatedEndTime)) { break; }

                        for (int i = 0; i < (Rise_Time_s - Timeout_s); i++)
                        {
                            if (DateTime.Now >= estimatedEndTime)
                            {
                                isBreak = true;
                                Fall(Step_Time_ms);
                                break;
                            }
                            Thread.Sleep(1000);
                        }
                        if (isBreak) { break; }

                        #endregion rise过程

                        #region fall过程

                        Fall(Step_Time_ms);
                        if (DateTime.Now >= estimatedEndTime)
                        {
                            break;
                        }
                        if (TimeoutCheck(new Stopwatch(), Step_Time_ms, Timeout_s, estimatedEndTime)) { break; }

                        for (int i = 0; i < (Fall_Time_s - Timeout_s); i++)
                        {
                            if (DateTime.Now >= estimatedEndTime)
                            {
                                break;
                            }
                            Thread.Sleep(1000);
                        }
                        if (isBreak) { break; }

                        #endregion fall过程
                    }
                });

            Console.WriteLine($"Process end at [{DateTime.Now}], run [{sw.ElapsedMilliseconds}]ms");
        }

        /// <summary>
        /// 升过程
        /// </summary>
        /// <param name="stepTime"></param>
        /// <param name="endTime"></param>
        public void Rise(double stepTime)
        {
            Console.WriteLine($"Start rising...[{DateTime.Now}]");
            Console.WriteLine();
            Stopwatch swRise = Stopwatch.StartNew();

            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep((int)(stepTime));
                Console.WriteLine($"Rising Step[{i + 1}]...[{swRise.ElapsedMilliseconds}]ms]");
            }

            Console.WriteLine();
            Console.WriteLine($"Rising finished...[{swRise.ElapsedMilliseconds}]ms");
            Console.WriteLine();
        }

        /// <summary>
        /// 降过程
        /// </summary>
        /// <param name="Step_Time_ms"></param>
        /// <param name="estimatedEndTime"></param>
        public void Fall(double Step_Time_ms)
        {
            Console.WriteLine($"Start falling...[{DateTime.Now}]");
            Console.WriteLine();

            Stopwatch swFall = Stopwatch.StartNew();

            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep((int)(Step_Time_ms));
                Console.WriteLine($"Falling Step[{i + 1}]...[{swFall.ElapsedMilliseconds}]ms");
            }

            Console.WriteLine();
            Console.WriteLine($"Falling finished...[{DateTime.Now}] for [{swFall.ElapsedMilliseconds}]ms");
            Console.WriteLine();
            swFall.Stop();
        }

        /// <summary>
        /// 检测超时
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="Timeout_s"></param>
        /// <param name="estimatedEndTime"></param>
        public bool TimeoutCheck(Stopwatch swTimeout, double Step_Time_ms, double Timeout_s, DateTime estimatedEndTime)
        {
            Console.WriteLine($"Start Timeout Check...[{DateTime.Now}]");
            Console.WriteLine();

            bool isBreak = false;
            swTimeout.Reset();
            swTimeout.Start();

            for (int i = 0; i < Timeout_s; i++)
            {
                if (DateTime.Now >= estimatedEndTime)
                {
                    isBreak = true;
                    Fall(Step_Time_ms);
                    break;
                }
                Thread.Sleep(1000);
                Console.WriteLine($"Timeout Check is running...[{swTimeout.ElapsedMilliseconds}]ms");
            }
            swTimeout.Stop();

            if (isBreak) { }
            else
            {
                Console.WriteLine();
                Console.WriteLine($"Timeout Check Finished...[{DateTime.Now}]");
                Console.WriteLine();
            }
            return isBreak;
        }

        /// <summary>
        /// 采集数据
        /// </summary>
        /// <param name="samplingInterval_s"></param>
        /// <param name="estimatedEndTime"></param>
        public void Collecting(double samplingInterval_s, DateTime estimatedEndTime)
        {
            Stopwatch swCollect = Stopwatch.StartNew();

            while (DateTime.Now < estimatedEndTime)
            {
                for (int i = 0; i < samplingInterval_s; i++)
                {
                    if (DateTime.Now >= estimatedEndTime)
                    {
                        break;
                    }
                    Thread.Sleep(1000);
                    Console.WriteLine($"Collecting... [{swCollect.ElapsedMilliseconds}]ms");
                }
            }
            swCollect.Stop();
        }
    }
}
