using SharpArgParse;
namespace SharpArgParseTest;

[TestClass]
public class RestArgs
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
    public void Empty()
    {
        var (opts, rest) = ArgParse.Parse<Target>([
        ]);
        Assert.IsFalse(opts.Alice);
        Assert.IsFalse(opts.Bob);
        Assert.IsFalse(opts.Charley);
        Assert.IsNull(opts.Recieve);
        Assert.AreEqual(opts.SomeRecieve.Length, 0);
    }
    [TestMethod]
    public void ArgOnly1()
    {
        var (opts, rest) = ArgParse.Parse<Target>([
            "value1",
        ]);
        Assert.IsFalse(opts.Alice);
        Assert.IsFalse(opts.Bob);
        Assert.IsFalse(opts.Charley);
        Assert.IsNull(opts.Recieve);
        Assert.AreEqual(opts.SomeRecieve.Length, 0);
        CollectionAssert.AreEqual(rest, (string[])[
            "value1",
        ]);
    }
    [TestMethod]
    public void ArgOnly3()
    {
        var (opts, rest) = ArgParse.Parse<Target>([
            "value1",
            "value2",
            "value3",
        ]);
        Assert.IsFalse(opts.Alice);
        Assert.IsFalse(opts.Bob);
        Assert.IsFalse(opts.Charley);
        Assert.IsNull(opts.Recieve);
        Assert.AreEqual(opts.SomeRecieve.Length, 0);
        CollectionAssert.AreEqual(rest, (string[])[
            "value1",
            "value2",
            "value3",
        ]);
    }
    [TestMethod]
    public void OptionBreak1()
    {
        // disallow option after "--".
        var (opts, rest) = ArgParse.Parse<Target>([
            "-a",
            "--",
            "-b",
        ], true);
        Assert.IsTrue(opts.Alice);
        Assert.IsFalse(opts.Bob);
        Assert.IsFalse(opts.Charley);
        Assert.IsNull(opts.Recieve);
        Assert.AreEqual(opts.SomeRecieve.Length, 0);
        CollectionAssert.AreEqual(rest, (string[])[
            "-b",
        ]);
    }
    [TestMethod]
    public void OptionBreak2()
    {
        // disallow option after "--".
        var (opts, rest) = ArgParse.Parse<Target>([
            "-a",
            "--",
            "-b",
        ], false);
        Assert.IsTrue(opts.Alice);
        Assert.IsFalse(opts.Bob);
        Assert.IsFalse(opts.Charley);
        Assert.IsNull(opts.Recieve);
        Assert.AreEqual(opts.SomeRecieve.Length, 0);
        CollectionAssert.AreEqual(rest, (string[])[
            "-b",
        ]);
    }
    [TestMethod]
    public void DisallowLater()
    {
        // like: bash -c 'mkdir $@' SHELLNAME -p test
        var (opts, rest) = ArgParse.Parse<Target>([
            "-a",
            "value",
            "-b",
        ], false);
        Assert.IsTrue(opts.Alice);
        Assert.IsFalse(opts.Bob);
        Assert.IsFalse(opts.Charley);
        Assert.IsNull(opts.Recieve);
        Assert.AreEqual(opts.SomeRecieve.Length, 0);
        CollectionAssert.AreEqual(rest, (string[])[
            "value",
            "-b",
        ]);
    }
    [TestMethod]
    public void AllowLater()
    {
        // like: grep REGEX -E
        var (opts, rest) = ArgParse.Parse<Target>([
            "-a",
            "value",
            "-b",
        ], true);
        Assert.IsTrue(opts.Alice);
        Assert.IsTrue(opts.Bob);
        Assert.IsFalse(opts.Charley);
        Assert.IsNull(opts.Recieve);
        Assert.AreEqual(opts.SomeRecieve.Length, 0);
        CollectionAssert.AreEqual(rest, (string[])[
            "value",
        ]);
    }
}