// See https://aka.ms/new-console-template for more information
using FileSignatureCreator.Console.InputValidation;
using FileSignatureCreator.Core.DataProcessing;
using FileSignatureCreator.Core.Infrastructure;
using FileSignatureCreator.Core.Jobs;
using FileSignatureCreator.Core.Logging;

var consoleAdapter = new ConsoleAdapter();
var logger = new Logger(true, consoleAdapter);
var validator = new CommandLineParametersValidator(logger);
var validationResult = validator.Validate(args);
if (validationResult.ValidationError != ValidationError.Success)
{
    Console.WriteLine(ValidationErrorMessageProvider.GetErrorMessage(validationResult));
    return 1;
}

var cancellationTokenSource = new CancellationTokenSource();
var jobFactory = new JobFactory(logger, cancellationTokenSource);

Console.CancelKeyPress += (_, cancelEventArgs) =>
{
    cancelEventArgs.Cancel = true;
    logger.Debug("user has requested process termination");
    cancellationTokenSource.Cancel();
};

var signatureCalculationResult = new FileSignatureCalculationProcessor(
    new FileProcessorsNumberCalculator(),
    logger,
    jobFactory,
    new BlocksNumberCalculator(),
    consoleAdapter)
    .RunSignatureCalculation(
    validationResult.InputParameters.FilePath,
    validationResult.InputParameters.DataBlockSize);

return signatureCalculationResult == OperationResult.Success? 0 : 1;