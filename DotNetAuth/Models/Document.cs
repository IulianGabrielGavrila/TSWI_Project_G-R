using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotNetAuth.Models
{
    public class Document
    {

        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public string OwnerId { get; set; } = "";
        [ForeignKey("OwnerId")]
        public IdentityUser? Owner { get; set; }
    }
}
