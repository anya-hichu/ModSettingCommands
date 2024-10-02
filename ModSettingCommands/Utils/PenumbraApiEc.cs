// Source: https://github.com/Ottermandias/Penumbra.Api/blob/97e9f427406f82a59ddef764b44ecea654a51623/Enums/PenumbraApiEc.cs

namespace ModSettingCommands.Utils;

/// <summary>
/// Error codes returned by some Penumbra.Api calls.
/// </summary>
public enum PenumbraApiEc
{
    Success = 0,
    NothingChanged = 1,
    CollectionMissing = 2,
    ModMissing = 3,
    OptionGroupMissing = 4,
    OptionMissing = 5
}
