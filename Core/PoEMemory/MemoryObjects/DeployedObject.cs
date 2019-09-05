namespace PoEMemory
{
    public class DeployedObject
    {
        internal DeployedObject(uint objId, ushort objectKey) {
            ObjectId = objId;
            ObjectKey = objectKey;
        }

        public uint ObjectId { get; private set; }

        public ushort ObjectKey { get; private set; }
        //public Entity Entity => (Entity) ObjectManager.Instance.GameController.EntityListWrapper.GetEntityById(ObjectKey);
        //public ActorSkill Skill => GameController.Instance.Player.GetComponent<Actor>().ActorSkills.Find(x => x.Id == ObjectKey);
    }
}