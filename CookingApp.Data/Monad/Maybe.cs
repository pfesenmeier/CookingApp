using System.Diagnostics.CodeAnalysis;

namespace CookingApp.Data.Monad;

public sealed class Maybe<TSome>
{
    public TSome? Some { get; init; }

    [MemberNotNullWhen(false, nameof(Some))]
    public bool IsNone => Some is null;

    [MemberNotNullWhen(true, nameof(Some))]
    public bool IsSome => !IsNone;

    public static implicit operator Maybe<TSome>(TSome some)
    {
        return new Maybe<TSome>() { Some = some };
    }

    public static implicit operator Maybe<TSome>(None none)
    {
        return new Maybe<TSome>();
    }
}
