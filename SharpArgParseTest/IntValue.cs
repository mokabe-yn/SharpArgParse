using SharpArgParse;
namespace SharpArgParseTest;

[TestClass]
public class IntValue
{
    class Target
    {
        public sbyte I8 { get; set; }
        public short I16 { get; set; }
        public int I32 { get; set; }
        public long I64 { get; set; }
        public byte U8 { get; set; }
        public ushort U16 { get; set; }
        public uint U32 { get; set; }
        public ulong U64 { get; set; }
    }
    [TestMethod]
    public void IntValue1()
    {
        var (opts, rest) = ArgParse.Parse<Target>([
            "--i32=42",
        ]);
        Assert.AreEqual(opts.I32, 42);
    }
    [TestMethod]
    public void IntValueI8()
    {
        var (opts, rest) = ArgParse.Parse<Target>([
            "--i8=42",
        ]);
        Assert.AreEqual(opts.I8, (sbyte)42);
    }
    [TestMethod]
    public void IntValueI16()
    {
        var (opts, rest) = ArgParse.Parse<Target>([
            "--i16=42",
        ]);
        Assert.AreEqual(opts.I16, (short)42);
    }
    [TestMethod]
    public void IntValueI32()
    {
        var (opts, rest) = ArgParse.Parse<Target>([
            "--i32=42",
        ]);
        Assert.AreEqual(opts.I32, (int)42);
    }
    [TestMethod]
    public void IntValueI64()
    {
        var (opts, rest) = ArgParse.Parse<Target>([
            "--i64=42",
        ]);
        Assert.AreEqual(opts.I64, (long)42);
    }

    [TestMethod]
    public void IntValueU8()
    {
        var (opts, rest) = ArgParse.Parse<Target>([
            "--u8=42",
        ]);
        Assert.AreEqual(opts.U8, (byte)42);
    }
    [TestMethod]
    public void IntValueU16()
    {
        var (opts, rest) = ArgParse.Parse<Target>([
            "--u16=42",
        ]);
        Assert.AreEqual(opts.U16, (ushort)42);
    }
    [TestMethod]
    public void IntValueU32()
    {
        var (opts, rest) = ArgParse.Parse<Target>([
            "--u32=42",
        ]);
        Assert.AreEqual(opts.U32, (uint)42);
    }
    [TestMethod]
    public void IntValueU64()
    {
        var (opts, rest) = ArgParse.Parse<Target>([
            "--u64=42",
        ]);
        Assert.AreEqual(opts.U64, (ulong)42);
    }

    [TestMethod]
    public void BigValue()
    {
        var (opts, rest) = ArgParse.Parse<Target>([
            "--u64=18446744073709551615",
        ]);
        Assert.AreEqual(opts.U64, 18446744073709551615L);
    }
    [TestMethod]
    public void TooBigValue()
    {
        Assert.ThrowsException<ArgumentException>(() => {
            var (_, _) = ArgParse.Parse<Target>([
                "--u32=18446744073709551615",
            ]);
        });
    }
}