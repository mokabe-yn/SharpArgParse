using SharpArgParse;
namespace SharpArgParseTest;

[TestClass]
class EnumValue
{
    enum E { 
        A, B, C, D, E,
        MultiWord,
    }
    class Target
    {
        // TODO: 以下のオプションを等価にできるAttribute追加
        // * --argument=a
        // * --argument-is-a
        // * -a
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
}