using FileSignatureCreator.Core.Logging;
using System.Security;
using System.Text.RegularExpressions;

namespace FileSignatureCreator.Console.InputValidation
{
    internal class CommandLineParametersValidator
    {
        private readonly ILogger _logger;
        private readonly Regex _dataBlockValidationRegex = 
            new Regex(@"^(?<value>\d+)(?<multiplier>[k | m | g]b)?$", 
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public CommandLineParametersValidator(ILogger logger)
        {
            _logger = logger;
        }

        public ValidationResult Validate(string[] args)
        {
            if (args.Length != 2)
            {
                return ValidationResult.Error(ValidationError.WrongArgumentsNumber);
            }

            var fileValidationResult = CheckFileByPath(args[0]);
            if (fileValidationResult != ValidationError.Success)
            {
                if (fileValidationResult == ValidationError.PathNotFound)
                {
                    fileValidationResult = ValidationError.SourceNotExists;
                }

                return ValidationResult.Error(fileValidationResult, args[0]);
            }

            if (!IsDataBlockSizeValid(args[1], out int dataBlockSize))
            {
                return ValidationResult.Error(ValidationError.WrongDataBlockSize, args[1]);
            }

            return ValidationResult.Successful(new JobParameters(args[0], dataBlockSize));
        }

        private ValidationError CheckFileByPath(string filePath)
        {
            try
            {
                using (File.OpenRead(filePath))
                {
                    return ValidationError.Success;
                }
            }
            catch (Exception ex) when (ex is ArgumentException ||
                                       ex is NotSupportedException ||
                                       ex is PathTooLongException)
            {
                _logger.Exception(ex);
                return ValidationError.PathIsIncorrect;
            }
            catch (Exception ex) when (ex is SecurityException ||
                                       ex is UnauthorizedAccessException)
            {
                _logger.Exception(ex);
                return ValidationError.SecurityViolation;
            }
            catch (Exception ex) when (ex is DirectoryNotFoundException ||
                                       ex is FileNotFoundException)
            {
                _logger.Exception(ex);
                return ValidationError.PathNotFound;
            }
            catch (Exception ex)
            {
                _logger.Exception(ex);
                return ValidationError.Other;
            }
        }

        private bool IsDataBlockSizeValid(string inputValue, out int dataBlockSize)
        {
            var match = _dataBlockValidationRegex.Match(inputValue);
            if(!match.Success)
            {
                dataBlockSize = 0;
                return false;
            }

            dataBlockSize = int.Parse(match.Groups["value"].Value);

            if(match.Groups["multiplier"].Success)
            {
                var multiplier = 1;
                switch(match.Groups["multiplier"].Value.ToLower())
                {
                    case "b":
                        break;
                    case "kb":
                        multiplier = 1024;
                        break;
                    case "mb":
                        multiplier = 1024 * 1024;
                        break;
                    case "gb":
                        multiplier = 1024 * 1024 * 1024;
                        break;
                    default:
                        _logger.Debug($"Unknown input parameter specified: {match.Groups["multiplier"].Value}");
                        return false;
                }

                dataBlockSize *= multiplier;
            }

            return dataBlockSize > 0;
        }
    }
}
