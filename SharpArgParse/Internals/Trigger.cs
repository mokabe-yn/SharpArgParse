using System;
using System.Reflection;

namespace SharpArgParse.Internals
{
    internal class Trigger
    {
        public static Trigger Empty = new Trigger(
            typeof(Trigger).GetProperty(nameof(TriggerName))
            ?? throw new InternalException(""));
        public static bool IsEmpty(Trigger t) => ReferenceEquals(t, Empty);

        public PropertyInfo TargetProperty { get; }
        public Type ElementType { get; }
        public bool IsMultiArgument { get; }
        public bool RecieveArgument { get; }
        public string TriggerName { get; }
        public char ShortTrigger { get; }
        public bool IsShortTrigger => ShortTrigger != '\0';
        private static readonly object UnsetValue = new object();
        private readonly object _backingTargetValue;
        public object TargetValue
        {
            get
            {
                if (ReferenceEquals(UnsetValue, _backingTargetValue))
                {
                    throw new InternalException(
                        $"{TargetProperty.Name}({TriggerName}):" +
                        "TargetValue is not available."
                        );
                }
                return _backingTargetValue;
            }
        }
        public Trigger(PropertyInfo targetProperty)
        {
            bool isbool = targetProperty.PropertyType == typeof(bool);
            _backingTargetValue = isbool ? true : UnsetValue;
            RecieveArgument = !isbool;
            TargetProperty = targetProperty;

            // T[] => T, T => T
            Type type = TargetProperty.PropertyType;
            IsMultiArgument = type.IsArray;
            ElementType = IsMultiArgument ?
                type.GetElementType() ?? throw new InternalException("") :
                type;

            ShortTrigger = '\0';
            TriggerName = "--" + StringConvert.ToKebabCase(targetProperty.Name);
        }
        public Trigger(PropertyInfo targetProperty, AliasAttribute alias)
            : this(targetProperty)
        {
            if (alias.IsShortAlias)
            {
                ShortTrigger = alias.ShortAlias;
            }
            else
            {
                TriggerName = "--" + StringConvert.ToKebabCase(alias.Alias);
            }

            if (alias is ValueAliasAttribute valias)
            {
                RecieveArgument = false;
                _backingTargetValue = valias.Value;
            }
        }
        private void ApplyValue(object options, object value)
        {
            if (IsMultiArgument)
            {
                // TODO: make T[] every time: O(N^2) => use List<T>: O(N)
                // [1,2,3] => [1,2,3,value]
                Array old = (Array)(TargetProperty.GetValue(options, null)
                    ?? Array.CreateInstance(ElementType, 0));
                Array dst = Array.CreateInstance(
                    ElementType, old.Length + 1);
                old.CopyTo(dst, 0);
                dst.SetValue(value, old.Length);
                TargetProperty.SetValue(options, dst, null);
            }
            else
            {
                TargetProperty.SetValue(options, value, null);
            }
        }
        public void Apply(object options)
            => ApplyValue(options, _backingTargetValue);
        public void Apply(object options, string stringValue)
        {
            object value;
            try
            {
                value = Utility.ConvertTo(ElementType, stringValue);
            }
            catch (Exception ex)
            {
                throw new CommandLineException(
                    $"{TriggerName}: {ex.Message}", ex);
            }
            ApplyValue(options, value);
        }
    }
}
