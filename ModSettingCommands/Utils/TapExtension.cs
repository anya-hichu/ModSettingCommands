using System;

namespace ModSettingCommands.Utils;

public static class TapExtension
{
    public static T Tap<T>(this T obj, Action<T> block)
    {
        block.Invoke(obj);
        return obj;
    }
}
