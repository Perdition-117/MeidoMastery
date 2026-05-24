using System.Collections.Generic;
using BepInEx;
using HarmonyLib;

namespace NoTheatreDance;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
class NoTheatreDance : BaseUnityPlugin {
	private readonly Harmony _harmony = Harmony.CreateAndPatchAll(typeof(NoTheatreDance));

	private void OnDestroy() {
		_harmony?.UnpatchSelf();
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(DanceSelect), nameof(DanceSelect.CreateMusicPanel))]
	private static void PreCreateMusicPanel(ref List<DanceData> __state) {
		__state = DanceSelect.dance_data_list_;
		if (RhythmAction_Mgr.NowDance != RhythmAction_Mgr.DanceType.Free) return;
		DanceSelect.dance_data_list_ = new(DanceSelect.dance_data_list_);
		DanceSelect.dance_data_list_.RemoveAll(e => e.bgType == DanceData.BgType.Theater);
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(DanceSelect), nameof(DanceSelect.CreateMusicPanel))]
	private static void CreateMusicPanel(List<DanceData> __state) {
		DanceSelect.dance_data_list_ = __state;
	}
}
