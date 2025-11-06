namespace cs_RestApiDemo.Models
{
    // This represents one industrial device in a factory.
    public class Device
    {
        public int Id { get; set; }                 // primary identifier
        public string Name { get; set; } = "";      // e.g. "Mixing PLC"
        public string IpAddress { get; set; } = ""; // e.g. "192.168.0.21"
        public string Location { get; set; } = "";  // e.g. "Line 1 / Panel A"
        public bool IsOnline { get; set; }          // status (could be from ping later)
    }
}
