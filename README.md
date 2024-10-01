# ModSettingCommands

Installable using my custom repository (https://github.com/anya-hichu/DalamudPluginRepo) or from compiled archives.

## Modset command

`/modset [Collection Name or Guid] [Mod Directory] [Mod Name] [Setting Name] (=|+=|-=)( [Setting Value])*`

### Examples

#### Stateless

##### Set option in group (exclusive)
`/modset Self "Modpacks" Eorzean-Nightlife-V2 Party-Pack = "032 [OCN] Click Click Flash (Hdance)"` -

##### Set options in group (exclusive)
`/modset Self "Modpacks" Eorzean-Nightlife-V2 Party-Pack = "032 [OCN] Click Click Flash (Hdance)" "033 [ChaotixFox] RGB Cheering (Cheer-on)"`

##### Unset all options in group
`/modset Self "Modpacks" Eorzean-Nightlife-V2 Party-Pack =`

#### Stateful

##### Set option in group
`/modset Self "Modpacks" Eorzean-Nightlife-V2 Party-Pack += "032 [OCN] Click Click Flash (Hdance)"`

##### Set options in group
`/modset Self "Modpacks" Eorzean-Nightlife-V2 Party-Pack += "032 [OCN] Click Click Flash (Hdance)" "033 [ChaotixFox] RGB Cheering (Cheer-on)"`

##### Unset option in group
`/modset Self "Modpacks" Eorzean-Nightlife-V2 Party-Pack -= "032 [OCN] Click Click Flash (Hdance)"`

##### Unset options in group
`/modset Self "Modpacks" Eorzean-Nightlife-V2 Party-Pack -= "032 [OCN] Click Click Flash (Hdance)" "033 [ChaotixFox] RGB Cheering (Cheer-on)"`


## Ifmodset command

`/ifmodset (-?|-!|-$|-e)? [Collection Name or Guid] [Mod Directory] [Mod Name]( [Setting Name] ==( [Setting Value])*)? ;( [Command])+`

`<` and `>` special characters have to be substitued in commands respectively with `[` and `]` (escaping is done by doubling `[` or `]`)

Flags:
 - `-?`: verbose mode - display in your local chatlog any text (including commands) that are sent to the server
 - `-!`: dry run - the same as verbose mode, except nothing is actually sent to the server
 - `-$`: abort the currently running macro (by using the /macrocancel command)

(inspired by https://github.com/PrincessRTFM/TinyCommands)

### Examples

- `/ifmodset -e -$ Self "Starlit" "Starlit" ; /hdance`
- `/ifmodset -e -$ {0} "[JULIExT] Noona" "[JULIExT] Noona" "Pose Option" == "[Sit 02] Ears ON (wiggles)" ; /groundsit`