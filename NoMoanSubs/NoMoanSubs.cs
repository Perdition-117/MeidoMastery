using BepInEx;
using HarmonyLib;

namespace NoMoanSubs;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
class NoMoanSubs : BaseUnityPlugin {
	private readonly Harmony _harmony = Harmony.CreateAndPatchAll(typeof(NoMoanSubs));

	private void OnDestroy() {
		_harmony?.UnpatchSelf();
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(YotogiPlayManager), nameof(YotogiPlayManager.AddRepeatVoiceText))]
	private static bool YotogiPlayManager_AddRepeatVoiceText() {
		return false;
	}
}
