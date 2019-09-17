using System;
using System.Diagnostics;
using JM.LinqFaster;

namespace ExileCore.Shared
{
    public class DebugInformation
    {
        public static readonly int SizeArray = 512;
        private readonly Stopwatch sw = Stopwatch.StartNew();
        private bool show;
        private double tick;

        public DebugInformation(string name, bool main = true)
        {
            Name = name;
            Main = main;

            for (var i = 0; i < SizeArray; i++)
            {
                Ticks[i] = 0;
                TicksAverage[i] = 0;
            }

            lock (Core.SyncLocker)
            {
                Core.DebugInformations.Add(this);
            }
        }

        public DebugInformation(string name, string description, bool main = true) : this(name, main)
        {
            Description = description;
        }

        public string Name { get; }
        public string Description { get; }
        public bool Main { get; }
        public int IndexTickAverage { get; private set; }
        public int Index { get; private set; }
        public float Sum { get; private set; }
        public float Total { get; private set; }
        private float TotalIndex { get; set; }
        public float TotalMaxAverage { get; private set; }
        public float TotalAverage { get; private set; }
        public float Average { get; private set; }
        public bool AtLeastOneFullTick { get; private set; }

        public double Tick
        {
            get => tick;
            set
            {
                tick = value;

                if (Index >= SizeArray)
                {
                    Index = 0;
                    Sum = Ticks.SumF();
                    TotalIndex += SizeArray;
                    Total += Sum;
                    Average = Sum / SizeArray;
                    TotalAverage = Total / TotalIndex;
                    TotalMaxAverage = Math.Max(TotalMaxAverage, Average);
                    if (IndexTickAverage >= SizeArray) IndexTickAverage = 0;

                    if (IndexTickAverage == 0 && Average > 16)
                    {
                        Average = 0;
                        TotalMaxAverage = 0;
                    }

                    TicksAverage[IndexTickAverage] = Average;
                    IndexTickAverage++;
                    AtLeastOneFullTick = true;
                }

                Ticks[Index] = (float) value;
                Index++;
            }
        }

        public float[] Ticks { get; } = new float[SizeArray];
        public float[] TicksAverage { get; } = new float[SizeArray];

        public void CorrectAfterTick(float val)
        {
            Ticks[Index - 1] = val;
            tick += val;
        }

        public float TickAction(Action action, bool onlyValue = false)
        {
            var start = sw.Elapsed.TotalMilliseconds;
            action.Invoke();
            var value = (float) (sw.Elapsed.TotalMilliseconds - start);
            if (!onlyValue) Tick = value;

            return value;
        }

        public void AddToCurrentTick(float value)
        {
            Ticks[Index] += value;
        }
    }
}
