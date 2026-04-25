using BepInEx;
using HarmonyLib;

namespace InvertLearntStat;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
class InvertLearntStat : BaseUnityPlugin {
	private readonly Harmony _harmony = Harmony.CreateAndPatchAll(typeof(InvertLearntStat));

	private void OnDestroy() {
		_harmony?.UnpatchSelf();
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(ProfileCtrl), nameof(ProfileCtrl.LoadMaidParamData))]
	private static void ProfileCtrl_LoadMaidParamData(ProfileCtrl __instance) {
		__instance.m_lStudyRate.text = __instance.ToPercent(1000 - __instance.m_maidStatus.studyRate).ToString();
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(StatusCtrl), nameof(StatusCtrl.SetData))]
	private static void StatusCtrl_SetData(StatusCtrl __instance, StatusCtrl.Status status) {
		__instance.m_lStudyRate.text = ((status.studyRate >= 0) ? (100 - status.studyRate).ToString() : "-");
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(YotogiParameterViewer), nameof(YotogiParameterViewer.UpdateTextParam))]
	private static void YotogiParameterViewer_UpdateTextParam(YotogiParameterViewer __instance) {
		var status = __instance.maid_.status;
		__instance.label_dictionary_[YotogiParameterViewer.LabelType.StudyRate].text = (100 - status.studyRate / 10).ToString();
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(YotogiOldParameterViewer), nameof(YotogiOldParameterViewer.UpdateTextParam))]
	private static void YotogiOldParameterViewer_UpdateTextParam(YotogiOldParameterViewer __instance) {
		var status = __instance.maid_.status;
		__instance.label_dictionary_[YotogiOldParameterViewer.LabelType.StudyRate].text = (100 - status.studyRate / 10).ToString();
	}
}
