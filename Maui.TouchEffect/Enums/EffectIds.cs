namespace MauiTouchEffect.Enums;

internal sealed class EffectIds
{
    /// <summary>
    /// The Base Resolution Group Name For Effects
    /// </summary>
    private static readonly string effectResolutionGroupName = $"{nameof(MauiTouchEffect)}.{nameof(Enums)}";

    /// <summary>
    /// Effect Id for <see cref="TouchEffect"/>
    /// </summary>
    public static string TouchEffect => $"{effectResolutionGroupName}.{nameof(TouchEffect)}";
}