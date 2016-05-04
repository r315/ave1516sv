using System;

public class NBench
{
    private class TimeReg
    {
        public int ops;
        public int time;
    }

    private static void Collect()
    {
        GC.Collect();
    }
    
    private static void InnerBenchmark(Func<object> func, int time, TimeReg timeReg)
    {
        int ops = 0;
        int begTime = Environment.TickCount, curTime, endTime = begTime + time;
        do {
            func(); func(); func(); func(); func(); func(); func(); func(); 
            func(); func(); func(); func(); func(); func(); func(); func(); 
            func(); func(); func(); func(); func(); func(); func(); func(); 
            func(); func(); func(); func(); func(); func(); func(); func(); 
            curTime = Environment.TickCount;
            ops += 32;
        } while (curTime < endTime);
        timeReg.ops = ops;
        timeReg.time = curTime - begTime;
    }
    
    private static void OutterBenchmark(Func<object> func, int time, int iters, int warmups)
    {
        TimeReg timeReg = new TimeReg();
        for (int w = 1; w <= warmups; ++w) {
            Console.Write("# Warmup Iteration {0,2}: ", w);
            InnerBenchmark(func, time, timeReg);
            Console.WriteLine("{0} ops/s", (((double)timeReg.ops)/timeReg.time)*1000);
            Collect();
        }
        for (int i = 1; i <= iters; ++i) {
            Console.Write("Iteration {0,2}: ", i);
            InnerBenchmark(func, time, timeReg);
            Console.WriteLine("{0} ops/s", (((double)timeReg.ops)/timeReg.time)*1000);
            Collect();
        }
    }

    public static void Benchmark(Func<object> func, String title)
    {
        Console.WriteLine("\n:: BENCHMARKING {0} ::",  title);
        OutterBenchmark(func, 1000, 10, 10);
    }
}
