// #name WorkProgressFix
// #author Perdition
// #desc Fixes displayed work progression on schedule screen.

using System;
using HarmonyLib;
using Schedule;

public static class WorkProgressFix {
	private static Harmony _instance;
	private static Harmony _bootstrapInstance;

	public static void Main() {
		// patching NoonWorkPlayExpRatio invokes static ScheduleCSVData ctor too early
		if (GameUty.FileSystem == null) {
			_bootstrapInstance = Harmony.CreateAndPatchAll(typeof(BootstrapPatch));
		} else {
			_instance = Harmony.CreateAndPatchAll(typeof(WorkProgressFix));
		}
	}

	public static void Unload() {
		_instance?.UnpatchSelf();
		_instance = null;
		_bootstrapInstance?.UnpatchSelf();
		_bootstrapInstance = null;
	}

	// fixes level 2 progress displaying half of actual value
	[HarmonyPrefix]
	[HarmonyPatch(typeof(ScheduleAPI), nameof(ScheduleAPI.NoonWorkPlayExpRatio))]
	private static bool ScheduleAPI_NoonWorkPlayExpRatio(ref float __result, int noonWorkPlayCount) {
		if (noonWorkPlayCount < ScheduleCSVData.noonWorkLv2Exp) {
			__result = noonWorkPlayCount / (float)ScheduleCSVData.noonWorkLv2Exp;
			return false;
		}
		if (noonWorkPlayCount < ScheduleCSVData.noonWorkLv3Exp) {
			__result = (noonWorkPlayCount - ScheduleCSVData.noonWorkLv2Exp) / (float)(ScheduleCSVData.noonWorkLv3Exp - ScheduleCSVData.noonWorkLv2Exp);
			return false;
		}
		__result = 0f;
		return false;
	}

	// fixes progress sometimes getting rounded down one point
	[HarmonyPrefix]
	[HarmonyPatch(typeof(ScheduleTraining), nameof(ScheduleTraining.expRatioFrom0To10), MethodType.Getter)]
	private static bool ScheduleTraining_expRatioFrom0To10(ScheduleTraining __instance, ref int __result) {
		__result = (int)Math.Round((double)(__instance.expRatio * 10f));
		return false;
	}

	class BootstrapPatch {
		[HarmonyPostfix]
		[HarmonyPatch(typeof(GameUty), nameof(GameUty.Init))]
		private static void Init() {
			_instance = Harmony.CreateAndPatchAll(typeof(WorkProgressFix));
		}
	}
}
