using SharpArgParse;
using System.Linq;
namespace SharpArgParseTest;


[TestClass]
class MultipleArgument
{
    class Target
    {
        public string[] Arguments { get; set; } = [];
    }
    [TestMethod]
    public void StyleSplit()
    {
        var (opts, rest) = ArgParse.Parse<Target>([
            "--arguments",
            "value",
        ]);
        CollectionAssert.AreEqual(opts.Arguments, (string[])[
            "value",
        ]);
    }
    [TestMethod]
    public void StyleEqual()
    {
        var (opts, rest) = ArgParse.Parse<Target>([
            "--arguments=value",
        ]);
        CollectionAssert.AreEqual(opts.Arguments, (string[])[
            "value",
        ]);
    }
    [TestMethod]
    public void SomeValue()
    {
        var (opts, rest) = ArgParse.Parse<Target>([
            "--arguments=value1",
            "--arguments=value2",
            "--arguments=value3",
        ]);
        CollectionAssert.AreEqual(opts.Arguments, (string[])[
            "value1",
            "value2",
            "value3",
        ]);
    }
}

