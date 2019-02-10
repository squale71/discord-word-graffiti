namespace Discord.WordGraffiti.DAL.Models
{
    public class UserWordMapping
    {
        public int Id { get; }
        public int UserId { get; set; }
        public int WordId { get; set; }
        public int Value { get; set; }
    }
}
