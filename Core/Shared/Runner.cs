using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using ExileCore.Shared.Interfaces;
using JM.LinqFaster;

namespace ExileCore.Shared
{
    public struct CoroutineDetails
    {
        private const int JOB_TIMEOUT_MS = 1000 / 5;

        public CoroutineDetails(string name, string ownerName, long ticks, DateTime started, DateTime finished)
        {
            Name = name;
            OwnerName = ownerName;
            Ticks = ticks;
            Started = started;
            Finished = finished;
        }

        public string Name { get; set; }
        public string OwnerName { get; set; }
        public long Ticks { get; set; }
        public DateTime Started { get; set; }
        public DateTime Finished { get; set; }
    }

    public class Runner
    {
        private readonly HashSet<Coroutine> _autorestartCoroutines = new HashSet<Coroutine>();
        private readonly List<CoroutineDetails> _finishedCoroutines = new List<CoroutineDetails>();
        private readonly object locker = new object();
        private readonly Stopwatch sw;
        private readonly List<Job> jobs = new List<Job>(16);
        private double time;

        public Runner(string name)
        {
            Name = name;
            sw = Stopwatch.StartNew();
        }

        public MultiThreadManager MultiThreadManager { get; set; }
        public string Name { get; }
        public int CriticalTimeWork { get; set; } = 150;
        public bool IsRunning => Coroutines.Count > 0;
        public int CoroutinesCount => Coroutines.Count;
        public List<CoroutineDetails> FinishedCoroutines => _finishedCoroutines.ToList();
        public int FinishedCoroutineCount { get; private set; }
        public IList<Coroutine> Coroutines { get; } = new List<Coroutine>();
        public IEnumerable<Coroutine> WorkingCoroutines => Coroutines.Where(x => x.Running);
        public int CountAddCoroutines { get; private set; }
        public int CountFalseAddCoroutines { get; private set; }
        public int IterationPerFrame { get; set; } = 2;
        public Dictionary<string, double> CoroutinePerformance { get; } = new Dictionary<string, double>();
        public int Count => Coroutines.Count;

        public Coroutine Run(IEnumerator enumerator, IPlugin owner, string name = null)
        {
            if (enumerator == null) throw new NullReferenceException("Coroutine cant not be null.");

            var routine = new Coroutine(enumerator, owner, name);

            lock (locker)
            {
                var first = Coroutines.FirstOrDefault(x => x.Name == routine.Name && x.Owner == routine.Owner);

                if (first != null)
                {
                    CountFalseAddCoroutines++;
                    return first;
                }

                Coroutines.Add(routine);
                CoroutinePerformance.TryGetValue(routine.Name, out var t);
                CoroutinePerformance[routine.Name] = t;

                CountAddCoroutines++;
                return routine;
            }
        }

        public Coroutine Run(Coroutine routine)
        {
            if (routine == null) throw new NullReferenceException("Coroutine cant not be null.");

            lock (locker)
            {
                var first = Coroutines.FirstOrDefault(x => x.Name == routine.Name && x.Owner == routine.Owner);

                if (first != null)
                {
                    CountFalseAddCoroutines++;
                    return first;
                }

                Coroutines.Add(routine);
                CoroutinePerformance.TryGetValue(routine.Name, out var t);
                CoroutinePerformance[routine.Name] = t;
                CountAddCoroutines++;
                return routine;
            }
        }

        public void PauseCoroutines(IList<Coroutine> coroutines)
        {
            foreach (var coroutine in coroutines)
            {
                coroutine.Pause();
            }
        }

        public void ResumeCoroutines(IList<Coroutine> coroutines)
        {
            foreach (var coroutine in coroutines)
            {
                if (coroutine.AutoResume)
                    coroutine.Resume();
            }
        }

        public Coroutine FindByName(string name)
        {
            return Coroutines.FirstOrDefault(x => x.Name == name);
        }

        public Coroutine ByFuncName(Func<string, bool> predicate)
        {
            return Coroutines.FirstOrDefault(x => predicate(x.Name));
        }

        public void Update()
        {
            if (Coroutines.Count <= 0) return;

            for (var i = 0; i < Coroutines.Count; i++)
            {
                var coroutine = Coroutines[i];

                if (!coroutine.IsDone)
                {
                    if (!coroutine.Running) continue;

                    try
                    {
                        time = sw.Elapsed.TotalMilliseconds;
                        double delta = 0;

                        var moveNext = coroutine.MoveNext();
                        if (!moveNext) coroutine.Done();
                        delta = sw.Elapsed.TotalMilliseconds - time;
                        CoroutinePerformance[coroutine.Name] += delta;

                        if (delta > CriticalTimeWork)
                        {
                            Console.WriteLine(
                                $"Coroutine {coroutine.Name} ({coroutine.OwnerName}) [{Name}] {delta} $Performance coroutine");
                        }
                    }
                    catch (Exception e)
                    {
                        CoroutinePerformance[$"{coroutine.Name} | ({DateTime.Now})"] =
                            CoroutinePerformance[coroutine.Name] + (sw.Elapsed.TotalMilliseconds - time);

                        CoroutinePerformance[coroutine.Name] = 0;
                        Console.WriteLine($"Coroutine {coroutine.Name} ({coroutine.OwnerName}) error: {e}");
                    }
                }
                else
                {
                    _finishedCoroutines.Add(new CoroutineDetails(coroutine.Name, coroutine.OwnerName, coroutine.Ticks, coroutine.Started,
                        DateTime.Now));

                    FinishedCoroutineCount++;
                    Coroutines.Remove(coroutine);
                }
            }
        }

        public void ParallelUpdate()
        {
            if (MultiThreadManager == null || MultiThreadManager.ThreadsCount < 1)
            {
                Update();
                return;
            }

            if (Coroutines.Count <= 0) return;
            jobs.Clear();

            for (var i = 0; i < Coroutines.Count; i++)
            {
                var coroutine = Coroutines[i];

                if (!coroutine.IsDone)
                {
                    if (!coroutine.Running) continue;

                    if (coroutine.NextIterRealWork && !coroutine.SyncModWork)
                    {
                        var addJob = MultiThreadManager.AddJob(() =>
                        {
                            var moveNext = coroutine.MoveNext();
                            if (!moveNext) coroutine.Done();
                        }, coroutine.Name);

                        jobs.Add(addJob);
                    }
                    else
                    {
                        time = sw.Elapsed.TotalMilliseconds;
                        double delta = 0;

                        var moveNext = coroutine.MoveNext();
                        if (!moveNext) coroutine.Done();
                        delta = sw.Elapsed.TotalMilliseconds - time;
                        CoroutinePerformance[coroutine.Name] += delta;

                        if (delta > CriticalTimeWork)
                        {
                            Console.WriteLine(
                                $"Coroutine {coroutine.Name} ({coroutine.OwnerName}) [{Name}] {delta} $Performance coroutine");
                        }
                    }
                }
                else
                {
                    _finishedCoroutines.Add(new CoroutineDetails(coroutine.Name, coroutine.OwnerName, coroutine.Ticks, coroutine.Started,
                        DateTime.Now));

                    FinishedCoroutineCount++;
                    Coroutines.Remove(coroutine);
                }
            }

            MultiThreadManager.Process(this);
            SpinWait.SpinUntil(() => jobs.AllF(job => job.IsCompleted), 500);

            foreach (var job in jobs)
            {
                CoroutinePerformance[job.Name] += job.ElapsedMs;
            }
        }
    }
}
