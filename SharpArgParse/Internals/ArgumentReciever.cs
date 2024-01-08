namespace SharpArgParse.Internals;

internal class ArgumentReciever<TOptions>
{
    private Trigger[] Shorts { get; }
    private Trigger[] Longs { get; }
    private bool AllowLater { get; }

    private bool RestOnlyMode { get; set; } = false;
    private Trigger CurrentTarget { get; set; } = Trigger.Empty;
    private List<string> RestArgument { get; } = new List<string>();
    // (boxed if struct) options instance
    private object Options { get; }
    public TOptions GetOptions() => (TOptions)Options;
    public string[] GetRest() => RestArgument.ToArray();
    public void Validate()
    {
        if (!Trigger.IsEmpty(CurrentTarget))
        {
            throw new CommandLineException(
                $"option {CurrentTarget.TriggerName} requires a value.");
        }
    }

    private Trigger UniqueLongTrigger(string arg)
    {
        if (arg.Contains('='))
        {
            arg = arg.Split('=', 2)[0];
        }
        var ret = Longs.Where(t => t.TriggerName.StartsWith(arg)).ToArray();
        if (ret.Length == 0)
        {
            throw new CommandLineException(
                $"unknown option: {arg.Substring(2)}" +
                $"");
        }
        if (ret.Length > 1)
        {
            var candidate = ret.Select(t => t.TriggerName);
            throw new CommandLineException(
                $"ambiguous option: {arg.Substring(2)}" +
                $" (could be {string.Join(" or ", candidate)})" +
                $"");
        }
        return ret[0];
    }
    private Trigger UniqueShortTrigger(char arg)
    {
        var ret = Shorts.Where(t => t.ShortTrigger == arg).ToArray();
        if (ret.Length == 0)
        {
            throw new CommandLineException(
                $"unknown option: {arg}" +
                $"");
        }
        if (ret.Length > 1)
        {
            var candidate = ret.Select(t => t.TriggerName);
            throw new CommandLineException(
                $"ambiguous option: {arg}" +
                $"");
        }
        return ret[0];
    }

    public ArgumentReciever(
        object options, bool allowLater, Trigger[] shorts, Trigger[] longs)
    {
        Options = options;
        AllowLater = allowLater;
        Shorts = shorts;
        Longs = longs;
    }
    public void Next(string arg)
    {
        Action<string> action =
            RestOnlyMode ? RestAdd :
            !Trigger.IsEmpty(CurrentTarget) ? RecieveRemainedArgument :
            arg == "--" ? TerminateOption :
            arg.StartsWith("--") ? ApplyLongOption :
            arg.StartsWith('-') ? ApplyShortOption :
            ApplyRestArgument;
        action(arg);
    }

    private void RestAdd(string arg)
        => RestArgument.Add(arg);
    private void RecieveRemainedArgument(string arg)
    {
        CurrentTarget.Apply(Options, arg);
        CurrentTarget = Trigger.Empty;
    }
    private void TerminateOption(string _)
        => RestOnlyMode = true;
    private void ApplyLongOption(string arg)
    {
        Trigger trigger = UniqueLongTrigger(arg);
        if (!trigger.RecieveArgument)
        {
            trigger.Apply(Options);
            return;
        }
        string[] a01 = arg.Split('=', 2);
        if (a01.Length == 2)
        {
            trigger.Apply(Options, a01[1]);
        }
        else
        {
            // need recieve next argument
            CurrentTarget = trigger;
        }
    }
    private void ApplyShortOption(string arg)
    {
        for (string s = arg.Substring(1); s.Length != 0; s = s.Substring(1))
        {
            Trigger t = UniqueShortTrigger(s[0]);
            if (!t.RecieveArgument)
            {
                // no argument option
                // grep -niE
                t.Apply(Options);
            }
            else
            {
                // recieve argument option
                if (s.Length != 1)
                {
                    // not splited : grep -ePATTERN
                    t.Apply(Options, s.Substring(1));
                }
                else
                {
                    // splited : grep -e PATTERN
                    CurrentTarget = t;
                }
                return;
            }
        }
    }
    private void ApplyRestArgument(string arg)
    {
        RestArgument.Add(arg);
        RestOnlyMode = !AllowLater;
    }
}
