using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace YoothubAPI.Validators
{
    public class YoutubeURL : ValidationAttribute
    {
        public YoutubeURL() : base (ErrorMessages.YoutubeURLInvalid)
        {

        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null && value is string)
            {  
                if(!Regex.IsMatch(value as string, @"^(https?\:\/\/)?(www\.)?(youtube\.com|youtu\.?be)\/.+$"))
                {
                    var errorMessage = FormatErrorMessage(validationContext.DisplayName);
                    return new ValidationResult(errorMessage);
                }
            }

            return ValidationResult.Success;
        }
    }
}
