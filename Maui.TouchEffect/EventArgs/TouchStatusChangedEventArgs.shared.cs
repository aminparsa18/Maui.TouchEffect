using MauiTouchEffect.Enums;

namespace MauiTouchEffect.EventArgs;

public class TouchStatusChangedEventArgs : System.EventArgs
{
    internal TouchStatusChangedEventArgs(TouchStatus status)
    {
        Status = status;
    }

    public TouchStatus Status { get; }
}