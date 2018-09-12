using MRC.ToolsAndEx.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MRC.Data
{
    public abstract class ValidationModel
    {
        public virtual void Validate()
        {
            List<ValidationResult> validationResults = new List<ValidationResult>();
            ValidationContext vc = new ValidationContext(this, null, null);
            bool isValid = Validator.TryValidateObject
                    (this, vc, validationResults, true);
            if (isValid == false)
            {
                throw new InvalidInputException(validationResults[0].ErrorMessage);
            }
        }
    }
}
