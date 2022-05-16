namespace FileSignatureCreator.Console.InputValidation
{
    public static class ValidationErrorMessageProvider
    {
        public static string GetErrorMessage(ValidationResult result)
        {
            switch (result.ValidationError)
            {
                case ValidationError.Success:
                    return string.Empty;
                case ValidationError.WrongArgumentsNumber:
                    return "Wrong arguments given. \r\nSyntax: FileSignatureCreator.Console.exe [input_filepath] [data_block_size]\r\n" +
                        "data block size is in bytes by default, but you can also specify kb, mb or gb modifiers";
                case ValidationError.WrongDataBlockSize:
                    return "Wrong data block size given.\r\nSyntax: FileSignatureCreator.Console.exe [input_filepath] [data_block_size]\r\n" +
                        "data block size is in bytes by default, but you can also specify kb, mb or gb modifiers";
                case ValidationError.SourceNotExists:
                    return $"Input file {result.AdditionalValidationData} does not exists";
                case ValidationError.PathIsIncorrect:
                    return $"Specified path {result.AdditionalValidationData} is incorrect. Please check for not allowed symbols " +
                           "and/or path length which should be less than 248 symbols for directory and 260 in total";
                case ValidationError.SecurityViolation:
                    return $"Security access violation for file {result.AdditionalValidationData}";
                case ValidationError.PathNotFound:
                    return $"File or directory specified ({result.AdditionalValidationData}) does not exists";
                case ValidationError.Other:
                    return $"Error occured while checking file {result.AdditionalValidationData}";
                default:
                    throw new ArgumentOutOfRangeException(nameof(result), result, null);
            }
        }
    }
}