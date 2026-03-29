// #name QuickEditItemUnlocker
// #author Perdition
// #desc Unlocks all available items in quick edit mode.

using HarmonyLib;

public static class QuickEditItemUnlocker {
	private const string QuickEditModeTag = "qasm";

	private static Harmony _instance;
	private static bool _isQuickEditMode = false;

	public static void Main() {
		_instance = Harmony.CreateAndPatchAll(typeof(QuickEditItemUnlocker));
		if (GameMain.Instance.GetNowSceneName() == "SceneEdit") {
			_isQuickEditMode = IsQuickEditMode();
		}
	}

	public static void Unload() {
		_instance?.UnpatchSelf();
		_instance = null;
	}

	private static bool IsQuickEditMode() {
		return GameMain.Instance.ScriptMgr.adv_kag.tag_backup?.ContainsKey(QuickEditModeTag) ?? false;
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(SceneEdit), nameof(SceneEdit.Awake))]
	private static void SceneEdit_Awake() {
		_isQuickEditMode = IsQuickEditMode();
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(PlayerStatus.Status), nameof(PlayerStatus.Status.IsHavePartsItem))]
	private static bool Status_IsHavePartsItem(ref bool __result) {
		if (_isQuickEditMode) {
			__result = true;
			return false;
		}
		return true;
	}
}
