namespace FileSignatureCreator.Console.InputValidation
{
    public enum ValidationError
    {
        Success,
        WrongArgumentsNumber,
        WrongDataBlockSize,
        SourceNotExists,
        PathIsIncorrect,
        SecurityViolation,
        PathNotFound,
        Other
    }
}