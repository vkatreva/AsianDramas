using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AsianDramas.Models.ViewModels
{
    public class DramaActorEditViewModel
    {
        public int DramaId { get; set; }          

        
        public int ActorId { get; set; }

        
        public int OriginalActorId { get; set; }
        public string ActorName { get; set; }

        public string DramaTitle { get; set; }   

        public string RoleType { get; set; }      
        public string CharacterName { get; set; } 

       
        public List<SelectListItem> ActorOptions { get; set; } = new();
    }
}
