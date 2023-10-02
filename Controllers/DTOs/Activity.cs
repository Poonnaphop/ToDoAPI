namespace ToDoAPI.DTOs
{
    public class Activity
    {
        public string Name { get; set; }
        public DateTime When { get; set; }
    }

    public class DeleteBody
    {
        public uint Id { get; set; }
    }
}