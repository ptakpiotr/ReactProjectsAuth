using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ReactProjectsAuthApi.Models
{
    public class MessageModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [EmailAddress]
        [JsonPropertyName("From")]
        public string From { get; set; }

        [Required]
        [EmailAddress]
        [JsonPropertyName("to")]
        public string To { get; set; }

        [MaxLength(100)]
        [JsonPropertyName("content")]
        public string Content { get; set; }

        public DateTime TimeOfCreation { get; set; }
    }
}
