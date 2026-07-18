using System;
using Verse;

public class Command_Toggle_DynamicDesc : Command_Toggle
{
    public Func<string> dynamicDesc;

    public override string Desc => dynamicDesc?.Invoke() ?? base.Desc;
}