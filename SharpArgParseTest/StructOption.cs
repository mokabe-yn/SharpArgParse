using SharpArgParse;
namespace SharpArgParseTest;

[TestClass]
public class StructOption
{
    struct Target
    {
        public bool Option { get; set; }
        public string? Argument { get; set; }
    }
    [TestMethod]
    public void SwitchOn()
    {
        var (opts, rest) = ArgParse.Parse<Target>([
            "--option",
        ]);
        Assert.IsTrue(opts.Option);
        Assert.IsNull(opts.Argument);
    }
    [TestMethod]
    public void NoTouch()
    {
        var (opts, rest) = ArgParse.Parse<Target>([
        ]);
        Assert.IsFalse(opts.Option);
        Assert.IsNull(opts.Argument);
    }
    [TestMethod]
    public void Assign()
    {
        var (opts, rest) = ArgParse.Parse<Target>([
            "--argument=value",
        ]);
        Assert.IsFalse(opts.Option);
        Assert.AreEqual(opts.Argument, "value");
    }
}

