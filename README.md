# ModSettingCommands

Installable using my custom repository (https://github.com/anya-hichu/DalamudPluginRepo) or from compiled archives.

## Syntax

`/modset [Collection Name or Guid] [Mod Directory] [Mod Name] [Setting Name] (=|+=|-=)( [Setting Value])*`

## Examples

### Stateless

Set option in group (exclusive):
- `/modset Self Eorzean-Nightlife-V2 Eorzean-Nightlife-V2 Party-Pack = "032 [OCN] Click Click Flash (Hdance)"`

Set options in group (exclusive):
- `/modset Self Eorzean-Nightlife-V2 Eorzean-Nightlife-V2 Party-Pack = "032 [OCN] Click Click Flash (Hdance)" "033 [ChaotixFox] RGB Cheering (Cheer-on)"`

Unset all options in group:
- `/modset Self Eorzean-Nightlife-V2 Eorzean-Nightlife-V2 Party-Pack =`

### Stateful

Set option in group:
- `/modset Self Eorzean-Nightlife-V2 Eorzean-Nightlife-V2 Party-Pack += "032 [OCN] Click Click Flash (Hdance)"`

Set options in group:
- `/modset Self Eorzean-Nightlife-V2 Eorzean-Nightlife-V2 Party-Pack += "032 [OCN] Click Click Flash (Hdance)" "033 [ChaotixFox] RGB Cheering (Cheer-on)"`

Unset option in group:
- `/modset Self Eorzean-Nightlife-V2 Eorzean-Nightlife-V2 Party-Pack -= "032 [OCN] Click Click Flash (Hdance)"`

Unset options in group:
- `/modset Self Eorzean-Nightlife-V2 Eorzean-Nightlife-V2 Party-Pack -= "032 [OCN] Click Click Flash (Hdance)" "033 [ChaotixFox] RGB Cheering (Cheer-on)"`
