using SharpArgParse;
namespace SharpArgParseTest;

[TestClass]
public class EnumValue
{
    enum E
    {
        A, B, C, D, E,
        MultiWord,
    }
    class Target
    {
        // TODO: 以下のオプションを等価にできるAttribute追加
        // * --argument=a
        // * --argument-is-a
        // * -a
        [ValueAlias('a', E.A)]
        [ValueAlias("a", E.A)]
        [ValueAlias("mw", E.MultiWord)]
        public E Argument { get; set; }
    }
    [TestMethod]
    public void SetValue1()
    {
        var (opts, rest) = ArgParse.Parse<Target>([
            "--argument=b",
        ]);
        Assert.AreEqual(opts.Argument, E.B);
    }
    [TestMethod]
    public void SetValue2()
    {
        var (opts, rest) = ArgParse.Parse<Target>([
            "--argument=c",
        ]);
        Assert.AreEqual(opts.Argument, E.C);
    }
    public void SetValue3()
    {
        var (opts, rest) = ArgParse.Parse<Target>([
            "--argument=multi-word",
        ]);
        Assert.AreEqual(opts.Argument, E.MultiWord);
    }
    public void SetValue4()
    {
        var (opts, rest) = ArgParse.Parse<Target>([
            "--a",
        ]);
        Assert.AreEqual(opts.Argument, E.A);
    }
    public void SetValue5()
    {
        var (opts, rest) = ArgParse.Parse<Target>([
            "--mw",
        ]);
        Assert.AreEqual(opts.Argument, E.MultiWord);
    }
    public void Short()
    {
        var (opts, rest) = ArgParse.Parse<Target>([
            "-a",
        ]);
        Assert.AreEqual(opts.Argument, E.A);
    }
}