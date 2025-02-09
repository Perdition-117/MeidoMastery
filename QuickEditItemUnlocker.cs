// #name QuickEditItemUnlocker
// #author Perdition
// #desc Unlocks all available items in quick edit mode.

using HarmonyLib;

public static class QuickEditItemUnlocker {
	private const string QuickEditModeTag = "qasm";

	private static Harmony _instance;
	private static bool isQuickEditMode = false;

	public static void Main() {
		_instance = Harmony.CreateAndPatchAll(typeof(QuickEditItemUnlocker));
	}

	public static void Unload() {
		_instance?.UnpatchSelf();
		_instance = null;
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(SceneEdit), nameof(SceneEdit.Awake))]
	private static void SceneEdit_Awake() {
		isQuickEditMode = GameMain.Instance.ScriptMgr.adv_kag.tag_backup?.ContainsKey(QuickEditModeTag) ?? false;
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(PlayerStatus.Status), nameof(PlayerStatus.Status.IsHavePartsItem))]
	private static bool Status_IsHavePartsItem(ref bool __result) {
		if (isQuickEditMode) {
			__result = true;
			return false;
		}
		return true;
	}
}
