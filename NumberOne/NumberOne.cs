using System.Collections.Generic;
using BepInEx;
using HarmonyLib;

namespace NumberOne;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
class NumberOne : BaseUnityPlugin {
	private readonly Harmony _harmony = Harmony.CreateAndPatchAll(typeof(NumberOne));

	private void OnDestroy() {
		_harmony?.UnpatchSelf();
	}

	private static bool IsNumberTwoSkill(FreeSkillSelect.ButtonData skill) {
		var prerequisites = skill.skill_data.exec_seikeiken;
		return prerequisites.ContainsKey(MaidStatus.Seikeiken.No_Yes)
			&& !prerequisites.ContainsKey(MaidStatus.Seikeiken.Yes_No)
			&& !prerequisites.ContainsKey(MaidStatus.Seikeiken.No_No);
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(FreeSkillSelect), nameof(FreeSkillSelect.CreateButtonData))]
	private static void OnCreateSkill(List<FreeSkillSelect.ButtonData> __result) {
		foreach (var type in __result) {
			foreach (var category in type.children_list) {
				category.children_list.RemoveAll(IsNumberTwoSkill);
			}
		}
	}
}
