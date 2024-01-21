using SharpArgParse;
namespace SharpArgParseTest;

[TestClass]
public class SingleArgumentOption
{
    class Target
    {
        public string? Argument { get; set; }
    }
    [TestMethod]
    public void StyleSplit()
    {
        var (opts, rest) = ArgParse.Parse<Target>([
            "--argument",
            "value",
        ]);
        Assert.AreEqual(opts.Argument, "value");
    }
    [TestMethod]
    public void StyleEqual()
    {
        var (opts, rest) = ArgParse.Parse<Target>([
            "--argument=value",
        ]);
        Assert.AreEqual(opts.Argument, "value");
    }
    [TestMethod]
    public void Overwrite()
    {
        var (opts, rest) = ArgParse.Parse<Target>([
            "--argument=value1",
            "--argument=value2",
            "--argument=value3",
        ]);
        Assert.AreEqual(opts.Argument, "value3");
    }
    [TestMethod]
    public void NoGive()
    {
        var (opts, rest) = ArgParse.Parse<Target>([
        ]);
        // "class Target" default value
        Assert.AreEqual(opts.Argument, null);
    }

    [TestMethod]
    public void Confused1()
    {
        var (opts, rest) = ArgParse.Parse<Target>([
            "--argument",
            "--argument",
        ]);
        Assert.AreEqual(opts.Argument, "--argument");
    }

}