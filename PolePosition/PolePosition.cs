using System.Collections.Generic;
using System.Linq;
using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace PolePosition;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
class PolePosition : BaseUnityPlugin {
	private static readonly Color color = new(1, 0.85f, 0.85f);

	private readonly Harmony _harmony = Harmony.CreateAndPatchAll(typeof(PolePosition));

	private void OnDestroy() {
		_harmony?.UnpatchSelf();
	}

	private static bool IsPoleDance(DanceData danceData) {
		return danceData.bgType == DanceData.BgType.PoleDance;
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(DanceSelect), nameof(DanceSelect.CreateMusicPanel))]
	private static void PreCreateMusicPanel(ref List<DanceData> __state) {
		__state = DanceSelect.dance_data_list_;
		var poleDances = DanceSelect.dance_data_list_.Where(IsPoleDance).ToArray();
		DanceSelect.dance_data_list_ = new(DanceSelect.dance_data_list_);
		DanceSelect.dance_data_list_.RemoveAll(IsPoleDance);
		DanceSelect.dance_data_list_.InsertRange(0, poleDances);
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(DanceSelect), nameof(DanceSelect.CreateMusicPanel))]
	private static void CreateMusicPanel(DanceSelect __instance, List<DanceData> __state) {
		DanceSelect.dance_data_list_ = __state;
		foreach (var danceData in __instance.m_SongPlatelUIPair.Where(e => IsPoleDance(e.Key))) {
			danceData.Value.backup_defaultColor = color;
			danceData.Value.defaultColor = color;
			danceData.Value.hover = color;
		}
	}
}
