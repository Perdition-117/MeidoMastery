using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using MaidStatus;

namespace LoadEditedNpcs;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
class LoadEditedNpcs : BaseUnityPlugin {
	private readonly Harmony _harmony = Harmony.CreateAndPatchAll(typeof(LoadEditedNpcs));

	private void OnDestroy() {
		_harmony?.UnpatchSelf();
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(BaseKagManager), nameof(BaseKagManager.TagCharaActivate))]
	private static bool TagCharaActivate(BaseKagManager __instance, ref bool __result, KagTagSupport tag_data) {
		if (__instance.script_mgr_.compatibilityMode) {
			return true;
		}

		if (!(tag_data.IsValid("maid") && tag_data.IsValid("npc"))) {
			return true;
		}

		GameMain.Instance.ScriptMgr.StopMotionScript();

		Maid maid = null;

		var characterMgr = GameMain.Instance.CharacterMgr;
		var npcName = tag_data.GetTagProperty("npc").AsString();

		for (var i = 0; i < characterMgr.GetStockMaidCount(); i++) {
			var stockMaid = characterMgr.GetStockMaid(i);
			if (stockMaid?.status.heroineType == HeroineType.Sub && stockMaid?.status.subCharaData?.uniqueName == npcName) {
				if (stockMaid.ActiveSlotNo < 0) {
					var maidIndex = tag_data.GetTagProperty("maid").AsInteger();
					characterMgr.SetActiveMaid(stockMaid, maidIndex);
					maid = stockMaid;
				}
				break;
			}
		}

		if (maid == null) {
			return true;
		}

		__result = false;

		if (!tag_data.IsValid("noload")) {
			maid.Visible = true;
			__result = __instance.SetMaidAllPropSeqWait(maid);
		}

		return false;
	}
}
