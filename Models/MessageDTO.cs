using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ReactProjectsAuthApi.Models
{
    public class MessageDTO
    {
        [JsonPropertyName("from")]
        [Required]
        [EmailAddress]
        public string From { get; set; }

        [JsonPropertyName("to")]
        [Required]
        [EmailAddress]
        public string To { get; set; }

        [JsonPropertyName("content")]
        [MaxLength(100)]
        public string Content { get; set; }
    }
}
