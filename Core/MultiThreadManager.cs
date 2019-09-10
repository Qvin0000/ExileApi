using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using JM.LinqFaster;
using SharpDX.Direct2D1;

namespace Exile
{
    [DebuggerDisplay("Name: {Name}, Elapsed: {ElapsedMs}, Completed: {IsCompleted}, Failed: {IsFailed}")]
    public class Job
    {
        public Action Work { get; set; }
        public string Name { get; set; }
        public volatile bool IsCompleted;
        public volatile bool IsFailed;
        public volatile bool IsStarted;
        public ThreadUnit WorkingOnThread { get; set; }
        public double ElapsedMs { get; set; }

        public Job(string name, Action work)
        {
            Name = name;
            Work = work;
        }
    }

    public class MultiThreadManager
    {
        private volatile bool ProcessWorking = false;
        private const long CriticalWorkTimeMs = 750;
        private readonly object locker = new object();
        private bool Closed;

        private SpinWait spinWait;
        private ThreadUnit[] threads;
        public int FailedThreadsCount { get; private set; }

        public MultiThreadManager(int countThreads)
        {
            spinWait = new SpinWait();
            ChangeNumberThreads(countThreads);
        }

        ConcurrentQueue<Job> Jobs = new ConcurrentQueue<Job>();
        Queue<Job> processJobs = new Queue<Job>();
        ConcurrentQueue<ThreadUnit> FreeThreads = new ConcurrentQueue<ThreadUnit>();
        List<ThreadUnit> BrokenThreads = new List<ThreadUnit>();
        public int ThreadsCount { get; private set; }

        public void ChangeNumberThreads(int countThreads)
        {
            lock (locker)
            {
                if (countThreads == ThreadsCount)
                    return;
                ThreadsCount = countThreads;

                if (threads != null)
                {
                    foreach (var thread in threads)
                    {
                        thread?.Abort();
                    }

                    while (!FreeThreads.IsEmpty) FreeThreads.TryDequeue(out _);
                }

                if (countThreads > 0)
                {
                    threads = new ThreadUnit[ThreadsCount];
                    for (var i = 0; i < ThreadsCount; i++)
                    {
                        threads[i] = new ThreadUnit($"Thread #{i}", i);
                        FreeThreads.Enqueue(threads[i]);
                    }
                }
                else
                {
                    threads = null;
                }
                
            }
        }


        public Job AddJob(Job job)
        {
            job.IsStarted = true;
            bool jobAbsorbed = false;
            if (!FreeThreads.IsEmpty)
            {
                FreeThreads.TryDequeue(out var threadUnit);
                if (threadUnit != null)
                {
                    jobAbsorbed = threadUnit.AddJob(job);
                    if (threadUnit.Free)
                    {
                        FreeThreads.Enqueue(threadUnit);
                    }   
                }
            }

            if (!jobAbsorbed)
            {
                Jobs.Enqueue(job);
            }

            return job;
        }

        public Job AddJob(Action action, string name)
        {
            var newJob = new Job(name, action);
     
            return AddJob(newJob);
        }

        private int _lock = 0;

        //Used for debug, maybe now can be delete
        private object _objectInitWork;

        public void Process(object o)
        {
            if (threads==null)
                return;
            if (Interlocked.CompareExchange(ref _lock, 1, 0) == 1)
            {
                return;
            }
            
            if (ProcessWorking)
            {
                DebugWindow.LogMsg($"WTF {_objectInitWork.GetType()}");
            }

            _objectInitWork = o;
            ProcessWorking = true;
            spinWait.Reset();

            while (Jobs.TryDequeue(out var j))
            {
                processJobs.Enqueue(j);
            }


            if (ThreadsCount > 1)
            {
                while (processJobs.Count > 0)
                {
                    if (!FreeThreads.IsEmpty)
                    {
                        FreeThreads.TryDequeue(out var freeThread);
                        var job = processJobs.Dequeue();
                        if (!freeThread.AddJob(job))
                        {
                            processJobs.Enqueue(job);
                        }
                        else
                        {
                            if (freeThread.Free)
                            {
                                FreeThreads.Enqueue(freeThread);
                            }
                        }
                    }
                    else
                    {
                        spinWait.SpinOnce();
                        var allThreadsBusy = true;
                        for (int i = 0; i < threads.Length; i++)
                        {
                            var th = threads[i];
                            if (th.Free)
                            {
                                allThreadsBusy = false;
                                FreeThreads.Enqueue(th);
                            }
                        }

                        if (allThreadsBusy)
                        {
                            for (int i = 0; i < threads.Length; i++)
                            {
                                var th = threads[i];
                                var thWorkingTime = th.WorkingTime;
                                if (thWorkingTime > CriticalWorkTimeMs)
                                {
                                    DebugWindow.LogMsg(
                                        $"Repair thread #{th.Number} with Job1: {th.Job.Name} (C: {th.Job.IsCompleted} F: {th.Job.IsFailed}) && Job2:{th.SecondJob.Name} (C: {th.SecondJob.IsCompleted} F: {th.SecondJob.IsFailed}) Time: {thWorkingTime} > {thWorkingTime >= CriticalWorkTimeMs}",
                                        5);
                                    th.Abort();
                                    BrokenThreads.Add(th);
                                    var newThread = new ThreadUnit($"Repair critical time {th.Number}", th.Number);
                                    threads[th.Number] = newThread;
                                    FreeThreads.Enqueue(newThread);
                                    Thread.Sleep(5);
                                    FailedThreadsCount++;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                var threadUnit = threads[0];
                while (processJobs.Count>0)
                {
                    if (threadUnit.Free)
                    {
                        var job = processJobs.Dequeue();
                        threadUnit.AddJob(job);
                    }
                    else
                    {
                        spinWait.SpinOnce();
                        var threadUnitWorkingTime = threadUnit.WorkingTime;
                        if (threadUnitWorkingTime > CriticalWorkTimeMs)
                        {
                            DebugWindow.LogMsg(
                                $"Repair thread #{threadUnit.Number} withreadUnit Job1: {threadUnit.Job.Name} (C: {threadUnit.Job.IsCompleted} F: {threadUnit.Job.IsFailed}) && Job2:{threadUnit.SecondJob.Name} (C: {threadUnit.SecondJob.IsCompleted} F: {threadUnit.SecondJob.IsFailed}) Time: {threadUnitWorkingTime} > {threadUnitWorkingTime >= CriticalWorkTimeMs}",
                                5);
                            threadUnit.Abort();
                            BrokenThreads.Add(threadUnit);
                            threadUnit = new ThreadUnit($"Repair critical time {threadUnit.Number}", threadUnit.Number);
                            Thread.Sleep(5);
                            FailedThreadsCount++;
                        }
                    }
                }
            }

            if (BrokenThreads.Count > 0)
            {
                var criticalWorkTimeMs = CriticalWorkTimeMs * 2;
                for (var index = 0; index < BrokenThreads.Count; index++)
                {
                    var brokenThread = BrokenThreads[index];
                    if(brokenThread==null) continue;
                    if (brokenThread.WorkingTime > criticalWorkTimeMs)
                    {
                        brokenThread.ForceAbort();
                        BrokenThreads[index] = null;
                    }
                }

                if (BrokenThreads.AllF(x => x == null))
                {
                    BrokenThreads.Clear();
                }
            }

            Interlocked.CompareExchange(ref _lock, 0, 1);
            ProcessWorking = false;
        }

        /*public void Wait() {
            spinWait.Reset();
            bool AnyBusy = true;
            while (AnyBusy && !Closed)
            {
                AnyBusy = false;
                for (int i = 0; i < threads.Length; i++)
                {
                    var threadItem = threads[i];
                    if (!threadItem.Free)
                    {
                        if (threadItem.WorkingTime >= CriticalWorkTimeMs)
                        {
                            threadItem.Abort(true);
                            

                            var newThread = new ThreadUnit($"Repair critical time {threadItem.Number}", threadItem.Number);
                            threads[threadItem.Number] = newThread;
                            FailedThreadsCount++;
                        }
                        AnyBusy = true;
                        spinWait.SpinOnce();
                        break;
                    }
                }
            }

            for (int i = 0; i < threads.Length; i++)
            {
                var th = threads[i];
                FreeThreads.Enqueue(th);
            }            
        }*/

        public void Close()
        {
            foreach (var thread in threads) thread.Abort();

            Closed = true;
        }
    }

    public class ThreadUnit
    {
        private readonly AutoResetEvent _event;
        private readonly Stopwatch sw;
        private readonly Thread thread;

        public static int CountJobs { get; set; }
        public static int CountWait { get; set; }
        private volatile bool abort;

        public ThreadUnit(string name, int number)
        {
            Number = number;
            Job = new Job("InitJob",null)
            {
                IsCompleted = true,
            };
            SecondJob = new Job("InitJob",null)
            {
                IsCompleted = true,
            };
            _event = new AutoResetEvent(false);

            thread = new Thread(DoWork);
            thread.Name = name;
            thread.IsBackground = true;
            thread.Start();
            sw = Stopwatch.StartNew();
        }

        public int Number { get; }

        public Job Job { get; private set; }
        public Job SecondJob { get; private set; }
        public bool Free => Job.IsCompleted || SecondJob.IsCompleted;

        public long WorkingTime => sw.ElapsedMilliseconds;

        private bool _wait = true;

        private bool running = true;

        private void DoWork()
        {
            while (running)
            {
                if (Job.IsCompleted && SecondJob.IsCompleted)
                {
                    _event.WaitOne();
                    CountWait++;
                    _wait = true;
                }

                if (!Job.IsCompleted)
                {
                    try
                    {
                        sw.Restart();
                        Job.Work?.Invoke();
                    }
                    catch (Exception e)
                    {
                        DebugWindow.LogError(e.ToString());
                        Job.IsFailed = true;
                    }
                    finally
                    {
                        Job.ElapsedMs = sw.Elapsed.TotalMilliseconds;
                        Job.IsCompleted = true;
                        sw.Restart();
                    }
                }

                if (!SecondJob.IsCompleted)
                {
                    try
                    {
                        sw.Restart();
                        SecondJob.Work?.Invoke();
                    }
                    catch (Exception e)
                    {
                        DebugWindow.LogError(e.ToString());
                        SecondJob.IsFailed = true;
                    }
                    finally
                    {
                        SecondJob.ElapsedMs = sw.Elapsed.TotalMilliseconds;
                        SecondJob.IsCompleted = true;
                        sw.Restart();
                    }
                }
            }
        }

        public bool AddJob(Job job)
        {
            job.WorkingOnThread = this;
            bool jobSetted = false;
            if (Job.IsCompleted)
            {
                Job = job;
                jobSetted = true;
                CountJobs++;
            }
            else if (SecondJob.IsCompleted)
            {
                SecondJob = job;
                jobSetted = true;
                CountJobs++;
            }

            if (_wait && jobSetted)
            {
                _wait = false;
                _event.Set();
            }
            return jobSetted;
        }

        public void Abort()
        {
            Job.IsCompleted = true;
            SecondJob.IsCompleted = true;
            Job.IsFailed = true;
            Job.IsFailed = true;
            abort = true;
            if (_wait)
            {
                _event.Set();
            }
            running = false;
            //thread.Abort();
        }

        public void ForceAbort()
        {
            abort = true;
            thread.Abort();
        }
    }
}