using System.ComponentModel.DataAnnotations.Schema;

namespace Amlakbashi.Core.Entities
{
    public class  Setting : Entity<int>
    {
        [Column("SettingID")]
        public override int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
