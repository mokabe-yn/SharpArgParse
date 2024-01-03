using SharpArgParse;
namespace SharpArgParseTest;

[TestClass]
public class MultiWord
{
    class Target
    {
        // PascalCase is converted to kebab-case in commandline.
        public bool MultiWordOption { get; set; }
    }
    [TestMethod]
    public void SwitchOn()
    {
        var (opts, rest) = ArgParse.Parse<Target>([
            "--multi-word-option",
        ]);
        Assert.IsTrue(opts.MultiWordOption);
    }
}
