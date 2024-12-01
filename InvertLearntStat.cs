// #name InvertLearntStat
// #author Perdition
// #desc Inverts the Learnt maid stat so it goes from 0 to 100%.

using HarmonyLib;

public static class InvertLearntStat {
	private static Harmony _instance;

	public static void Main() {
		_instance = Harmony.CreateAndPatchAll(typeof(InvertLearntStat));
	}

	public static void Unload() {
		_instance?.UnpatchSelf();
		_instance = null;
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
