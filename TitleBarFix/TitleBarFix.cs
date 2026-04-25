using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace TitleBarFix;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
class TitleBarFix : BaseUnityPlugin {
	private readonly Harmony _harmony = Harmony.CreateAndPatchAll(typeof(TitleBarFix));

	private void OnDestroy() {
		_harmony?.UnpatchSelf();
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(SceneADV), nameof(SceneADV.CallTitleBar))]
	private static void CallTitleBar(SceneADV __instance) {
		if (Screen.width > 1920) {
			var position = __instance.titleBar.transform.localPosition;
			position.x = __instance.titleBarEffectStartPosX += (Screen.width - 1920) / 2;
			__instance.titleBar.transform.localPosition = position;
		}
	}
}
