namespace AsianDramas.Models.ViewModels
{
    public class CastItemVM
    {
        public int DramaActorId { get; set; }   
        public int ActorId { get; set; }         

        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string RoleLabel { get; set; }      
        public string CharacterName { get; set; }
    }
}
