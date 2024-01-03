using SharpArgParse;
namespace SharpArgParseTest;

[TestClass]
public class BinaryOption
{
    class Target
    {
        public bool Option { get; set; }
    }
    [TestMethod]
    public void SwitchOn()
    {
        var (opts, rest) = ArgParse.Parse<Target>([
            "--option",
        ]);
        Assert.IsTrue(opts.Option);
    }
    [TestMethod]
    public void DoubleOn()
    {
        var (opts, rest) = ArgParse.Parse<Target>([
            "--option",
            "--option",
        ]);
        Assert.IsTrue(opts.Option);
    }
    [TestMethod]
    public void NoTouch()
    {
        var (opts, rest) = ArgParse.Parse<Target>([
        ]);
        Assert.IsFalse(opts.Option);
    }
}

