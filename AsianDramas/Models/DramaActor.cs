using System;
using System.ComponentModel.DataAnnotations;

namespace AsianDramas.Models
{
    public class DramaActor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int DramaId { get; set; }
        public Drama Drama { get; set; }

        [Required]
        public int ActorId { get; set; }
        public Actor Actor { get; set; }

        [Required]
        [StringLength(100)]
        public string RoleName { get; set; }
        public bool IsMainRole { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int SortOrder { get; set; } = 0;
    }
}
