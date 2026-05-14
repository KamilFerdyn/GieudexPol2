namespace GieudexPol.Application.DTOs
{
    public class NbpSyncResultDto
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int Added { get; set; }
        public int Skipped { get; set; }
        public int TablesFetched { get; set; }
        public List<string> ProcessedRanges { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();
    }
}
