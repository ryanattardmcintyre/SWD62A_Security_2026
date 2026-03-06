using Domain.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Blog
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(AllowEmptyStrings =false, ErrorMessage ="Title cannot be left blank")]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters")]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "Title can only contain letters, numbers, and spaces")]
        public string Title { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Content cannot be left blank")]
        [StringLength(1000, ErrorMessage = "Content cannot be longer than 100 characters")]
        
        [RegularExpression(@"^[a-zA-Z0-9\s@_-|]+$", ErrorMessage = "Content can only contain letters, numbers, and spaces")]
        [BlogContentValidator(MaxWordCount = 5, ErrorMessage = "Content cannot have more than 5 words")]
        public string Content { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Date cannot be left blank")]
        public DateTime CreatedAt { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address format")]
        public string? AuthorEmail { get; set; }

        [DefaultValue(false)]
        public bool? Public { get; set; }
        public string? FilePath { get; set; }

        [Range(0, 1000, ErrorMessage = "Price must be between 0 and 1000, Contact admin if you would like to set a higher price")]
        public double Price { get; set; }

        public string? Password { get; set; }
        
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string? ConfirmPassword { get; set; }

    }
}
