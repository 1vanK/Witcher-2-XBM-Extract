# Witcher 2 XBM Extractor (XBM to DDS)

Works only with "The Witcher 2: Assassins Of Kings - Enhanced Edition / 3.4.4.1 / GOG"

## Step 1: extract *.DZIP

1. Download and compile https://github.com/gibbed/Gibbed.RED
2. Unpack
```
Gibbed.RED.Unpack.exe "c:\GOG Games\The Witcher 2 Enhanced Edition\CookedPC\pack0.dzip" "c:\temp\Unpack_Witcher2"

Gibbed.RED.Unpack.exe "c:\GOG Games\The Witcher 2 Enhanced Edition\CookedPC\abetterui.dzip" "c:\temp\Unpack_Witcher2_abetterui"
Gibbed.RED.Unpack.exe "c:\GOG Games\The Witcher 2 Enhanced Edition\CookedPC\alchemy_suit.dzip" "c:\temp\Unpack_Witcher2_alchemy_suit"
Gibbed.RED.Unpack.exe "c:\GOG Games\The Witcher 2 Enhanced Edition\CookedPC\arena.dzip" "c:\temp\Unpack_Witcher2_alchemy_arena"
Gibbed.RED.Unpack.exe "c:\GOG Games\The Witcher 2 Enhanced Edition\CookedPC\base_scripts.dzip" "c:\temp\Unpack_Witcher2_base_scripts"
Gibbed.RED.Unpack.exe "c:\GOG Games\The Witcher 2 Enhanced Edition\CookedPC\darkdiff.dzip" "c:\temp\Unpack_Witcher2_darkdiff"
Gibbed.RED.Unpack.exe "c:\GOG Games\The Witcher 2 Enhanced Edition\CookedPC\dlc_finishers.dzip" "c:\temp\Unpack_Witcher2_dlc_finishers"
Gibbed.RED.Unpack.exe "c:\GOG Games\The Witcher 2 Enhanced Edition\CookedPC\elf_flotsam.dzip" "c:\temp\Unpack_Witcher2_elf_flotsam"
Gibbed.RED.Unpack.exe "c:\GOG Games\The Witcher 2 Enhanced Edition\CookedPC\hairdresser.dzip" "c:\temp\Unpack_Witcher2_hairdresser"
Gibbed.RED.Unpack.exe "c:\GOG Games\The Witcher 2 Enhanced Edition\CookedPC\harpy_feathers.dzip" "c:\temp\Unpack_Witcher2_harpy_feathers"
Gibbed.RED.Unpack.exe "c:\GOG Games\The Witcher 2 Enhanced Edition\CookedPC\krbr.dzip" "c:\temp\Unpack_Witcher2_krbr"
Gibbed.RED.Unpack.exe "c:\GOG Games\The Witcher 2 Enhanced Edition\CookedPC\magical_suit.dzip" "c:\temp\Unpack_Witcher2_magical_suit"
Gibbed.RED.Unpack.exe "c:\GOG Games\The Witcher 2 Enhanced Edition\CookedPC\merchant.dzip" "c:\temp\Unpack_Witcher2_merchant"
Gibbed.RED.Unpack.exe "c:\GOG Games\The Witcher 2 Enhanced Edition\CookedPC\roche_jacket.dzip" "c:\temp\Unpack_Witcher2_roche_jacket"
Gibbed.RED.Unpack.exe "c:\GOG Games\The Witcher 2 Enhanced Edition\CookedPC\succubuss.dzip" "c:\temp\Unpack_Witcher2_succubuss"
Gibbed.RED.Unpack.exe "c:\GOG Games\The Witcher 2 Enhanced Edition\CookedPC\summer.dzip" "c:\temp\Unpack_Witcher2_summer"
Gibbed.RED.Unpack.exe "c:\GOG Games\The Witcher 2 Enhanced Edition\CookedPC\swordsman_suit.dzip" "c:\temp\Unpack_Witcher2_swordsman_suit"
Gibbed.RED.Unpack.exe "c:\GOG Games\The Witcher 2 Enhanced Edition\CookedPC\troll.dzip" "c:\temp\Unpack_Witcher2_troll"
Gibbed.RED.Unpack.exe "c:\GOG Games\The Witcher 2 Enhanced Edition\CookedPC\tutorial.dzip" "c:\temp\Unpack_Witcher2_tutorial"
Gibbed.RED.Unpack.exe "c:\GOG Games\The Witcher 2 Enhanced Edition\CookedPC\winter.dzip" "c:\temp\Unpack_Witcher2_winter"
```

## Step 2: decode *.XBM
```
W2XBM.exe "input dir or file" "output dir"
```

Examples:

1. Extract file to "c:\temp\Decode Witcher 2"
```
W2XBM.exe "c:\temp\Unpack_Witcher2\characters\main_npc\triss__woman\model\triss__b2.xbm" "c:\temp\Decode Witcher 2"
```

2. Extract file to current dir
```
W2XBM.exe "c:\temp\Unpack_Witcher2\characters\main_npc\zoltan__dwarf\model\zoltan__b1.xbm" "."
```

3. Extract all *.xbm files
```
W2XBM.exe "c:\temp\Unpack_Witcher2" "c:\temp\Decode Witcher 2"
```

---------------------------------------

Многие ресурсы игры хранятся в универсальном контейнере CR2W. У файлов разные расширения, но внутри файлов контейнер CR2W.
Текстуры хранятся в файлах с расширениями *.XBM. Данная утилита написана максимально просто. Контейнер не парсится полностью,
а только те его части, которые используются при хранении текстур.

---------------------------------------

License: Public Domain
