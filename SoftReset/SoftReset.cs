using BepInEx;
using UnityEngine;

namespace SoftReset;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
class SoftReset : BaseUnityPlugin {
	private void Update() {
		if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R)) {
			GameMain.Instance.LoadScene("SceneToTitle");
			GameMain.Instance.MainCamera.FadeOut(0f);
		}
	}
}
