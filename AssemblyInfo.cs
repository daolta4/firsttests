using NUnit.Framework;

// Cho các CLASS test chạy song song với nhau
[assembly: Parallelizable(ParallelScope.Fixtures)]
// Số luồng tối đa chạy cùng lúc (thường = số nhân CPU)
[assembly: LevelOfParallelism(4)]