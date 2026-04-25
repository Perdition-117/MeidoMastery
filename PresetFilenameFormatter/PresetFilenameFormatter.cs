using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using BepInEx;
using HarmonyLib;

namespace PresetFilenameFormatter;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
class PresetFilenameFormatter : BaseUnityPlugin {
	private readonly Harmony _harmony = Harmony.CreateAndPatchAll(typeof(PresetFilenameFormatter));

	private void OnDestroy() {
		_harmony?.UnpatchSelf();
	}

	private static string GetPresetName(Maid maid) {
		return UTY.FileNameEscape($"pre_{maid.status.lastName}{maid.status.firstName}_{DateTime.Now:yyyyMMdd_HHmmss}");
	}

	[HarmonyTranspiler]
	[HarmonyPatch(typeof(CharacterMgr), nameof(CharacterMgr.PresetSave))]
	private static IEnumerable<CodeInstruction> PresetSave(IEnumerable<CodeInstruction> instructions) {
		return new CodeMatcher(instructions)
			.MatchStartForward(new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(UTY), nameof(UTY.FileNameEscape))))
			.Advance(-1)
			.SetAndAdvance(OpCodes.Ldarg_1, null)
			.SetOperandAndAdvance(AccessTools.Method(typeof(PresetFilenameFormatter), nameof(GetPresetName)))
			.InstructionEnumeration();
	}
}
