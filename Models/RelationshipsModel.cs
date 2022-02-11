using System.ComponentModel.DataAnnotations;

namespace ReactProjectsAuthApi.Models
{
    public class RelationshipsModel
    {
        [Key]
        public Guid Id { get; set; }
        
        [EmailAddress]
        public string UserEmail { get; set; }

        public string Friends { get; set; }
    }
}
