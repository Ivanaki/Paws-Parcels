namespace MotorHome.Scripts
{
    [System.Serializable]
    public class ItemData
    {
        public Item.ItemPlace Type;
        public int QueueNumberInsideCar;
        public int QueueNumberUpCar;
        public Vector3DTO Position;
        public QuaternionDTO Rotation;
        //public bool IsTouchable;
        public Vector3DTO LinearVelocity;
        public Vector3DTO AngularVelocity;

        public ItemData()
        {
        }

        public ItemData(Item.ItemPlace type, Vector3DTO position, QuaternionDTO rotation/*,  bool isTouchable*/, 
            Vector3DTO linearVelocity, Vector3DTO angularVelocity, int queueNumberInsideCar,  int queueNumberUpCar)
        {
            Type = type;
            Position = position;
            Rotation = rotation;
            //IsTouchable = isTouchable;
            LinearVelocity = linearVelocity;
            AngularVelocity = angularVelocity;
            QueueNumberInsideCar = queueNumberInsideCar;
            QueueNumberUpCar = queueNumberUpCar;
        }

        public ItemData(Item item, int queueNumberInsideCar, int queueNumberUpCar)
        {
            QueueNumberInsideCar = queueNumberInsideCar;
            QueueNumberUpCar = queueNumberUpCar;
            Type = item.Place;
            Position = new Vector3DTO(item.transform.position);
            Rotation = new QuaternionDTO(item.transform.rotation);
            //IsTouchable = item.IsTouchable;
            LinearVelocity = new Vector3DTO(item.Rigidbody.linearVelocity);
            AngularVelocity = new Vector3DTO(item.Rigidbody.angularVelocity);
        }
    }
}