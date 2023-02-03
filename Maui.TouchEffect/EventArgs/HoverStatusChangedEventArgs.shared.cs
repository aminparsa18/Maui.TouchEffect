using MauiTouchEffect.Enums;

namespace MauiTouchEffect.EventArgs;

public class HoverStatusChangedEventArgs : System.EventArgs
{
    internal HoverStatusChangedEventArgs(HoverStatus status)
    {
        Status = status;
    }

    public HoverStatus Status { get; }
}