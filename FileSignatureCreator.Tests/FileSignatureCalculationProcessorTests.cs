using FileSignatureCreator.Core.DataProcessing;
using FileSignatureCreator.Core.Infrastructure;
using FileSignatureCreator.Core.Jobs;
using FileSignatureCreator.Core.Logging;
using Moq;

namespace FileSignatureCreator.Tests
{
    [TestClass]
    public class FileSignatureCalculationProcessorTests
    {
        [TestMethod]
        public void TestFileSignatureCalculatuionWith1MbBlockSize()
        {
            var consoleMoq = new ConsoleMock();

            var loggerMoq = new Mock<ILogger>().Object;
            var cancellationTokenSource = new CancellationTokenSource();
            var dataBlockSize = 1024 * 1024;

            var processor = new FileSignatureCalculationProcessor(
                new FileProcessorsNumberCalculator(),
                loggerMoq,
                new JobFactory(loggerMoq, cancellationTokenSource),
                new BlocksNumberCalculator(),
                consoleMoq);
            var result = processor.RunSignatureCalculation("IMG_3594.mp4", dataBlockSize);

            Assert.AreEqual(OperationResult.Success, result);

            var actualBlocksInformation = consoleMoq.ReadAllLines();
            var desiredBlocksInformation = new[]
            {
                "Block 0 Hash aad99adb3dc165f2ad8d218e0a800119422ad2320f0f98d2a670cae52aa3ffa9",
                "Block 1 Hash 71caaffa58bf32a8e6dca752bf8016c03b0b264fa47187ea6c4ffe23e65e7a6f",
                "Block 2 Hash bea34a2b5f97e4d839c4d5b941ec2a59a0159c1244c3d0339d9c8b3c3c709003",
                "Block 3 Hash d760c47fe68f9dcf283bf03fd91a9cf4bbc90f37337fc6c2af66a47c6c0025e2",
                "Block 4 Hash f94259084c7a07575600e669377f69854fa15bb08d3e6c686eaaf54d44133018",
                "Block 5 Hash cbe9c72ae8e8a7e1d62cd3a02767e5c51733d8356a3bbccc7ba99015a3586ca5",
                "Block 6 Hash 6ab84bdd21b3afbad46a559f158ad6ce03ce1afc41f424efe8bf30c46c489a2b",
                "Block 7 Hash 29103cb8b7852e3f642913b219606db28d6f0965633075a779c80b16d7210e97",
                "Block 8 Hash 5b9197702cdf9d03be452a3efc0caed000e105d6f4aca2315522644679d7c563",
                "Block 9 Hash f6518420aa1b54d5155b4f7d1c39e334bea8eab83aae94687a26c49972eb10b4",
                "Block 10 Hash 75af233cdc58c423afe277201ff73e835f81e7baceb46b8abd179072ac52e34d",
                "Block 11 Hash 98524155aba2c2df5ba66c603440f6c42e992afa6e27b16a6d9f5c7095adbbd9",
                "Block 12 Hash 84333bc3a3d4f58ee103af677b24d8c0830682d82cfa2b4accf4cd17c38990c8",
                "Block 13 Hash a43d08632c172805615bd646feb932e063736d10badf21e13f98017dfec06bd1"
            };

            Assert.IsTrue(Enumerable.SequenceEqual(desiredBlocksInformation, actualBlocksInformation));
        }

        [TestMethod]
        public void TestFileSignatureCalculatuionWith10MbBlockSize()
        {
            var consoleMoq = new ConsoleMock();

            var loggerMoq = new Mock<ILogger>().Object;
            var cancellationTokenSource = new CancellationTokenSource();
            var dataBlockSize = 10 * 1024 * 1024;

            var processor = new FileSignatureCalculationProcessor(
                new FileProcessorsNumberCalculator(),
                loggerMoq,
                new JobFactory(loggerMoq, cancellationTokenSource),
                new BlocksNumberCalculator(),
                consoleMoq);
            var result = processor.RunSignatureCalculation("IMG_3594.mp4", dataBlockSize);

            Assert.AreEqual(OperationResult.Success, result);

            var actualBlocksInformation = consoleMoq.ReadAllLines();
            var desiredBlocksInformation = new[]
            {
                "Block 0 Hash 5695cb98698a36599b815ea564f6631ad5a41d10e65a91c62c4078405cab3e82",
                "Block 1 Hash 3347030c487d86ce06603f8ce84f303559701a8f6014440c473ef7c3ca485bed"
            };

            Assert.IsTrue(Enumerable.SequenceEqual(desiredBlocksInformation, actualBlocksInformation));
        }

        [TestMethod]
        public void TestFileSignatureCalculatuionWith100MbBlockSize()
        {
            var consoleMoq = new ConsoleMock();

            var loggerMoq = new Mock<ILogger>().Object;
            var cancellationTokenSource = new CancellationTokenSource();
            var dataBlockSize = 100 * 1024 * 1024;

            var processor = new FileSignatureCalculationProcessor(
                new FileProcessorsNumberCalculator(),
                loggerMoq,
                new JobFactory(loggerMoq, cancellationTokenSource),
                new BlocksNumberCalculator(),
                consoleMoq);
            var result = processor.RunSignatureCalculation("IMG_3594.mp4", dataBlockSize);

            Assert.AreEqual(OperationResult.Success, result);

            var actualBlocksInformation = consoleMoq.ReadAllLines();
            var desiredBlocksInformation = new[]
            {
                "Block 0 Hash 2fd2bee83ba620201846d393571e2ecec41e9b608f12b4b18d90d6d929e96aba"
            };

            Assert.IsTrue(Enumerable.SequenceEqual(desiredBlocksInformation, actualBlocksInformation));
        }

        [TestMethod]
        public void TestAccessRestrictedError()
        {
            var consoleMoq = new ConsoleMock();

            var logger = new Logger(false, consoleMoq);
            var cancellationTokenSource = new CancellationTokenSource();
            var dataBlockSize = 1024 * 1024;

            using (var restrictedFileStream = File.OpenWrite("restrictedFile.txt"))
            {
                var processor = new FileSignatureCalculationProcessor(
                    new FileProcessorsNumberCalculator(),
                    logger,
                    new JobFactory(logger, cancellationTokenSource),
                    new BlocksNumberCalculator(),
                    consoleMoq);
                var result = processor.RunSignatureCalculation("restrictedFile.txt", dataBlockSize);

                Assert.AreEqual(OperationResult.Error, result);
                var errorString = consoleMoq.ReadLine();
                Assert.IsTrue(errorString.Contains("System.IO.IOException: The process cannot access the file "));
            }
        }

        [TestMethod]
        public void TestEmptyFileSignatureCalculation()
        {
            var consoleMoq = new ConsoleMock();

            var loggerMoq = new Mock<ILogger>().Object;
            var cancellationTokenSource = new CancellationTokenSource();
            var dataBlockSize = 1024;

            File.WriteAllText("EmptyFile.txt", string.Empty);

            var processor = new FileSignatureCalculationProcessor(
                new FileProcessorsNumberCalculator(),
                loggerMoq,
                new JobFactory(loggerMoq, cancellationTokenSource), 
                new BlocksNumberCalculator(), 
                consoleMoq);
            var result = processor.RunSignatureCalculation("EmptyFile.txt", dataBlockSize);

            Assert.AreEqual(OperationResult.Success, result);
            Assert.AreEqual("Block 0 Hash e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855", 
                consoleMoq.ReadLine());
        }

        [TestMethod]
        public void TestCancelFileSignatureCalculatuion()
        {
            var consoleMoq = new ConsoleMock();

            var logger = new Logger(false, consoleMoq);
            var cancellationTokenSource = new CancellationTokenSource();
            var dataBlockSize = 1024;

            var processor = new FileSignatureCalculationProcessor(
                new FileProcessorsNumberCalculator(),
                logger,
                new JobFactory(logger, cancellationTokenSource),
                new BlocksNumberCalculator(),
                consoleMoq);

            new Thread(() =>
            {
                Thread.Sleep(500);
                cancellationTokenSource.Cancel();
            }).Start();

            var result = processor.RunSignatureCalculation("IMG_3594.mp4", dataBlockSize);

            Assert.AreEqual(OperationResult.Error, result);
            var processLogs = consoleMoq.ReadAllLines().Last();
            Assert.IsTrue(processLogs.Contains("System.OperationCanceledException: The operation was canceled"));
        }
    }
}