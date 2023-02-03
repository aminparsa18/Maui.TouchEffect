using MauiTouchEffect.Enums;

namespace MauiTouchEffect.EventArgs;

public class HoverStateChangedEventArgs : System.EventArgs
{
    internal HoverStateChangedEventArgs(HoverState state)
    {
        State = state;
    }

    public HoverState State { get; }
}