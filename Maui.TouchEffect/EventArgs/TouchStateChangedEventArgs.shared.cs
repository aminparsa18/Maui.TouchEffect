using MauiTouchEffect.Enums;

namespace MauiTouchEffect.EventArgs;

public class TouchStateChangedEventArgs : System.EventArgs
{
    internal TouchStateChangedEventArgs(TouchState state)
    {
        State = state;
    }

    public TouchState State { get; }
}