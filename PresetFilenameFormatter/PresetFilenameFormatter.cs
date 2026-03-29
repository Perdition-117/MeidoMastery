// #name PresetFilenameFormatter
// #author Perdition
// #desc Separates date and time components in saved preset file names.

using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

public static class PresetFilenameFormatter {
	private static Harmony _instance;

	public static void Main() {
		_instance = Harmony.CreateAndPatchAll(typeof(PresetFilenameFormatter));
	}

	public static void Unload() {
		_instance?.UnpatchSelf();
		_instance = null;
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
