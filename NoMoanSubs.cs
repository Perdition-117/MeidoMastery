// #name NoMoanSubs
// #author Perdition
// #desc Hides subtitles for yotogi moaning loops.

using HarmonyLib;

public static class NoMoanSubs {
	private static Harmony _instance;

	public static void Main() {
		_instance = Harmony.CreateAndPatchAll(typeof(NoMoanSubs));
	}

	public static void Unload() {
		_instance?.UnpatchSelf();
		_instance = null;
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(YotogiPlayManager), nameof(YotogiPlayManager.AddRepeatVoiceText))]
	private static bool YotogiPlayManager_AddRepeatVoiceText(SceneScenarioSelect __instance) {
		return false;
	}
}
