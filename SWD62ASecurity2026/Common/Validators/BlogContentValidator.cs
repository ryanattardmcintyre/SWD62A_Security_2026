using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Validators
{
    public class BlogContentValidator: ValidationAttribute
    {
        public int MaxWordCount { get; set; }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            
            //int wordCount = Convert.ToInt16(validationContext.Items["MaxWordCount"]);
            string content = value as string;
            if (content.Split(new char[] { ' '}).Length >= MaxWordCount )
            {
                return new ValidationResult($"Content has more than {MaxWordCount} words");
            }
            return ValidationResult.Success;
        }
    }
}
