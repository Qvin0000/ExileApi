using System;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;

namespace ExileCore.PoEMemory.Elements
{
    public class LabelOnGround : RemoteMemoryObject
    {
        private readonly Lazy<string> debug;
        private readonly Lazy<long> labelInfo;

        public LabelOnGround()
        {
            labelInfo = new Lazy<long>(GetLabelInfo);

            debug = new Lazy<string>(() =>
            {
                return ItemOnGround.HasComponent<WorldItem>()
                    ? ItemOnGround.GetComponent<WorldItem>().ItemEntity?.GetComponent<Base>()?.Name
                    : ItemOnGround.Path;
            });
        }

        public bool IsVisible => Label?.IsVisible ?? false;

        public Entity ItemOnGround
        {
            get
            {
                var readObjectAt = ReadObjectAt<Entity>(0x10);
                return readObjectAt.Address == 0 ? null : readObjectAt;
            }
        }

        public Element Label
        {
            get
            {
                var readObjectAt = ReadObjectAt<Element>(0x18);
                return readObjectAt.Address == 0 ? null : readObjectAt;
            }
        }

        //Temp solution for pick it, need test PickTest and PickTest2
        public bool CanPickUp
        {
            get
            {
                var label = Label;

                if (label != null)
                    return M.Read<long>(label.Address + 0x420) == 0;

                return true;
            }
        }

        public TimeSpan TimeLeft
        {
            get
            {
                if (CanPickUp) return new TimeSpan();
                if (labelInfo.Value == 0) return MaxTimeForPickUp;
                var futureTime = M.Read<int>(labelInfo.Value + 0x38);
                return TimeSpan.FromMilliseconds(futureTime - Environment.TickCount);
            }
        }

        //Temp solution for pick it
        public TimeSpan MaxTimeForPickUp =>
            TimeSpan.Zero; // !CanPickUp ? TimeSpan.FromMilliseconds(M.Read<int>(labelInfo.Value + 0x34)) : new TimeSpan();

        private long GetLabelInfo()
        {
            return Label != null ? Label.Address != 0 ? M.Read<long>(Label.Address + 0x3A8) : 0 : 0;
        }

        public override string ToString()
        {
            return debug.Value;
        }
    }
}
