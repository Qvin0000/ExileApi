namespace ExileCore.PoEMemory.Components
{
    public class DeployedObject
    {
        internal DeployedObject(uint objId, ushort objectKey)
        {
            ObjectId = objId;
            ObjectKey = objectKey;
        }

        public uint ObjectId { get; }
        public ushort ObjectKey { get; }

        // public Entity Entity => ObjectManager.Instance.GameController.EntityListWrapper.GetEntityById(ObjectKey);
        //public ActorSkill Skill => ObjectManager.Instance.GameController.Player.GetComponent<Actor>().ActorSkills.Find(x => x.Id == ObjectKey);
    }
}
