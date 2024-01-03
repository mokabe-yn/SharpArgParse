using SharpArgParse;
namespace SharpArgParseTest;

[TestClass]
public class ShortOptions
{
    class Target
    {
        [Alias('a')]
        public bool Alice { get; set; }
        [Alias('b')]
        public bool Bob { get; set; }
        [Alias('c')]
        public bool Charley { get; set; }
        [Alias('r')]
        public string? Recieve { get; set; }
        [Alias('s')]
        public string[] SomeRecieve { get; set; } = [];
    }
    [TestMethod]
    public void SingleA()
    {
        var (opts, rest) = ArgParse.Parse<Target>([
            "-a",
        ]);
        Assert.IsTrue(opts.Alice);
        Assert.IsFalse(opts.Bob);
        Assert.IsFalse(opts.Charley);
    }
    [TestMethod]
    public void SingleB()
    {
        var (opts, rest) = ArgParse.Parse<Target>([
            "-b",
        ]);
        Assert.IsFalse(opts.Alice);
        Assert.IsTrue(opts.Bob);
        Assert.IsFalse(opts.Charley);
    }
    [TestMethod]
    public void SingleC()
    {
        var (opts, rest) = ArgParse.Parse<Target>([
            "-c",
        ]);
        Assert.IsFalse(opts.Alice);
        Assert.IsFalse(opts.Bob);
        Assert.IsTrue(opts.Charley);
    }

    [TestMethod]
    public void DoubleAB()
    {
        var (opts, rest) = ArgParse.Parse<Target>([
            "-ab",
        ]);
        Assert.IsTrue(opts.Alice);
        Assert.IsTrue(opts.Bob);
        Assert.IsFalse(opts.Charley);
    }
    [TestMethod]
    public void DoubleBA()
    {
        var (opts, rest) = ArgParse.Parse<Target>([
            "-ba",
        ]);
        Assert.IsTrue(opts.Alice);
        Assert.IsTrue(opts.Bob);
        Assert.IsFalse(opts.Charley);
    }
    [TestMethod]
    public void DualAB()
    {
        var (opts, rest) = ArgParse.Parse<Target>([
            "-a",
            "-b",
        ]);
        Assert.IsTrue(opts.Alice);
        Assert.IsTrue(opts.Bob);
        Assert.IsFalse(opts.Charley);
    }
    [TestMethod]
    public void DualBA()
    {
        var (opts, rest) = ArgParse.Parse<Target>([
            "-b",
            "-a",
        ]);
        Assert.IsTrue(opts.Alice);
        Assert.IsTrue(opts.Bob);
        Assert.IsFalse(opts.Charley);
    }

    [TestMethod]
    public void Recieve()
    {
        var (opts, rest) = ArgParse.Parse<Target>([
            "-r",
            "Arg",
        ]);
        Assert.AreEqual(opts.Recieve, "Arg");
    }
    [TestMethod]
    public void Recieve2()
    {
        var (opts, rest) = ArgParse.Parse<Target>([
            "-rArg",
        ]);
        Assert.AreEqual(opts.Recieve, "Arg");
    }
    [TestMethod]
    public void Recieve3()
    {
        var (opts, rest) = ArgParse.Parse<Target>([
            "-rarg",
        ]);
        Assert.AreEqual(opts.Recieve, "arg");
    }
    [TestMethod]
    public void Recieve4()
    {
        var (opts, rest) = ArgParse.Parse<Target>([
            "-abrarg",
        ]);
        Assert.IsTrue(opts.Alice);
        Assert.IsTrue(opts.Bob);
        Assert.IsFalse(opts.Charley);
        Assert.AreEqual(opts.Recieve, "arg");
    }

}