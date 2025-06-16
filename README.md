# ModSettingCommands

Commands to interact with penumbra mod settings using an intuitive syntax.

Installable using my custom repository (instructions here: https://github.com/anya-hichu/DalamudPluginRepo) or from compiled archives.

## Msc command

### Help

```
Usage:
  /msc [options]
Options:
  -h, /h, --help, -?, /? Show help and usage information
Subcommands:
  set                  Set group options
  clear                Clear groups
  add                  Add group options
  remove               Remove group options
  assert               Assert group options
  tmp                  Temporary operations
```

### Permanent subcommands

#### Set

```
Usage:
  /msc set [options]
Options:
  --collection, -c     Collection
  --mod-dir, -m        Mod directory
  --mod-name, -n       Mod name (optional)
  --group, -g          Group
  --option, -o         Options
  --inherit, -i        Inherit flag (optional)
  --priority, -p       Priority (optional)
  --enabled, -e        Enabled flag (optional)
```

#### Clear

```
Usage:
  /msc clear [options]
Options:
  --collection, -c     Collection
  --mod-dir, -m        Mod directory
  --mod-name, -n       Mod name (optional)
  --group, -g          Group
```

#### Add

```
Usage:
  /msc add [options]
Options:
  --collection, -c     Collection
  --mod-dir, -m        Mod directory
  --mod-name, -n       Mod name (optional)
  --group, -g          Group
  --option, -o         Options
```

#### Remove

```
Usage:
  /msc remove [options]
Options:
  --collection, -c     Collection
  --mod-dir, -m        Mod directory
  --mod-name, -n       Mod name (optional)
  --group, -g          Group
  --option, -o         Options
```

#### Assert

```
Usage:
  /msc assert [options]
Options:
  --collection, -c     Collection
  --mod-dir, -m        Mod directory
  --mod-name, -n       Mod name (optional)
  --group, -g          Group
  --option, -o         Options
  --enabled, -e        Enabled flag (optional)
  --priority, -p       Priority (optional)
  --success-cmd, -sc   Success command (optional)
  --failure-cmd, -fc   Failure command (optional)
```

### Temporary subcommands

#### Help

```
Usage:
  /msc tmp [options]
Subcommands:
  set                  Set group options
  clear                Clear groups
  add                  Add group options
  remove               Remove group options
  assert               Assert group options
  revert               Revert temporary settings
```

#### Set

```
Usage:
  /msc tmp set [options]
Options:
  --collection, -c     Collection
  --mod-dir, -m        Mod directory
  --mod-name, -n       Mod name (optional)
  --group, -g          Group
  --option, -o         Options
  --inherit, -i        Inherit flag (optional)
  --priority, -p       Priority (optional)
  --enabled, -e        Enabled flag (optional)
  --key, -k            Key
  --source, -s         Source
```

#### Clear

```
Usage:
  /msc tmp clear [options]
Options:
  --collection, -c     Collection
  --mod-dir, -m        Mod directory
  --mod-name, -n       Mod name (optional)
  --group, -g          Group
  --key, -k            Key
  --source, -s         Source
```

#### Add

```
Usage:
  /msc tmp add [options]
Options:
  --collection, -c     Collection
  --mod-dir, -m        Mod directory
  --mod-name, -n       Mod name (optional)
  --group, -g          Group
  --option, -o         Options
  --key, -k            Key
  --source, -s         Source
```

#### Remove

```
Usage:
  /msc tmp remove [options]
Options:
  --collection, -c     Collection
  --mod-dir, -m        Mod directory
  --mod-name, -n       Mod name (optional)
  --group, -g          Group
  --option, -o         Options
  --key, -k            Key
  --source, -s         Source
```

#### Assert

```
Usage:
  /msc tmp assert [options]
Options:
  --collection, -c     Collection
  --mod-dir, -m        Mod directory
  --mod-name, -n       Mod name (optional)
  --group, -g          Group
  --option, -o         Options
  --enabled, -e        Enabled flag (optional)
  --priority, -p       Priority (optional)
  --success-cmd, -sc   Success command (optional)
  --failure-cmd, -fc   Failure command (optional)
  --key, -k            Key
```

#### Revert

```
Usage:
  /msc tmp revert [options]
Options:
  --collection, -c     Collection
  --key, -k            Key
```

# Deprecated

## Modset command

`/modset [Collection Name or Guid] [Mod Directory] [Mod Name] [Setting Name] (=|+=|-=)( [Setting Value])*`

### Examples

#### Stateless

Set option in group (exclusive):
- `/modset Self Eorzean-Nightlife-V2 Eorzean-Nightlife-V2 Party-Pack = "032 [OCN] Click Click Flash (Hdance)"`

Set options in group (exclusive):
- `/modset Self Eorzean-Nightlife-V2 Eorzean-Nightlife-V2 Party-Pack = "032 [OCN] Click Click Flash (Hdance)" "033 [ChaotixFox] RGB Cheering (Cheer-on)"`

Unset all options in group:
- `/modset Self Eorzean-Nightlife-V2 Eorzean-Nightlife-V2 Party-Pack =`

#### Stateful

##### Set option in group
`/modset Self Eorzean-Nightlife-V2 Eorzean-Nightlife-V2 Party-Pack += "032 [OCN] Click Click Flash (Hdance)"`

##### Set options in group
`/modset Self Eorzean-Nightlife-V2 Eorzean-Nightlife-V2 Party-Pack += "032 [OCN] Click Click Flash (Hdance)" "033 [ChaotixFox] RGB Cheering (Cheer-on)"`

##### Unset option in group
`/modset Self Eorzean-Nightlife-V2 Eorzean-Nightlife-V2 Party-Pack -= "032 [OCN] Click Click Flash (Hdance)"`

##### Unset options in group
`/modset Self Eorzean-Nightlife-V2 Eorzean-Nightlife-V2 Party-Pack -= "032 [OCN] Click Click Flash (Hdance)" "033 [ChaotixFox] RGB Cheering (Cheer-on)"`

## Ifmodset command

`/ifmodset (-?|-!|-$|-e)? [Collection Name or Guid] [Mod Directory] [Mod Name]( [Setting Name] ==( [Setting Value])*)? ;( [Command])+`

`<` and `>` special characters have to be substituted in commands with respectively `[` and `]` (escaping is done by doubling `[` or `]`)

Flags:
 - `-?`: verbose mode - display in your local chatlog any text (including commands) that are sent to the server
 - `-!`: dry run - the same as verbose mode, except nothing is actually sent to the server
 - `-$`: abort the currently running macro (by using the /macrocancel command)
 - `-e`: check mod is enabled

(inspired by https://github.com/PrincessRTFM/TinyCommands)

### Examples

- `/ifmodset -e -$ Self "Starlit" "Starlit" ; /hdance`
- `/ifmodset -e -$ Self "Starlit" "Starlit" "Music Type" == "Original" ; "/hdance"`
- `/ifmodset -e -$ Self "[JULIExT] Noona" "[JULIExT] Noona" "Pose Option" == "[Sit 02] Ears ON (wiggles)" ; "/groundsit [wait.1]" "/dpose 2"`