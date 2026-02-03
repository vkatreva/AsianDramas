using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AsianDramas.Models.ViewModels
{
    public class DramaActorAddViewModel
    {
        [Required]
        public int DramaId { get; set; }

        [Required]
        [Display(Name = "Actor")]
        public int ActorId { get; set; }

        [Required]
        [Display(Name = "Role type")]
        public string RoleType { get; set; }   

        [Required]
        [StringLength(100)]
        [Display(Name = "Character name")]
        public string CharacterName { get; set; }

        public List<SelectListItem> ActorOptions { get; set; } = new();
    }
}
