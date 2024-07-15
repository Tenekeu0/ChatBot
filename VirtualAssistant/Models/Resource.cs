namespace VirtualAssistant.Models
{
    public class Resource
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public Resource(int id, string name, string description, string location)
        {
/*            if(string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("le nom ne peut etre null ou vide");
            }
            if (string.IsNullOrEmpty(description))
            {
                throw new ArgumentNullException("la description ne peut etre null ou vide");
            }
            if (string.IsNullOrEmpty(location))
            {
                throw new ArgumentNullException("la localisation ne peut etre null ou vide");
            }
            if(id < 0)
            {
                throw new ArgumentOutOfRangeException("l'identifiant ne peut etre négatif");
            }*/
            Id = id;
            Name = name;
            Description = description;
            Location = location;
        }
    }
}
