using SharpArgParse;
namespace SharpArgParseTest;

[TestClass]
class ArgumentOption
{
    class Target
    {
        public string? Argument { get; set; }
    }
    [TestMethod]
    public void TestMethod1()
    {
        var (opts, rest) = ArgParse.Parse<Target>([
            "--argument",
            "value",
        ]);
        Assert.AreEqual(opts.Argument, "value");
    }

}