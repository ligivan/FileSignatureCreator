namespace FileSignatureCreator.Console.InputValidation
{
    public class ValidationResult
    {
        public ValidationError ValidationError { get; private set; }

        public string AdditionalValidationData { get; private set; } = "";
        
        public JobParameters InputParameters { get; private set; }

        public static ValidationResult Successful(JobParameters parameters) => new ValidationResult
        {
            ValidationError = ValidationError.Success,
            InputParameters = parameters
        };

        public static ValidationResult Error(ValidationError validationError, string additionalData = "") => new ValidationResult
        {
            ValidationError = validationError,
            AdditionalValidationData = additionalData
        };
    }
}