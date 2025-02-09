// #name SoftReset
// #author Perdition
// #desc Returns to title screen using a hotkey.

using UnityEngine;

public static class SoftReset {
	private static GameObject _gameObject;

	public static void Main() {
		_gameObject = new GameObject();
		_gameObject.AddComponent<UpdateBehaviour>();
	}

	public static void Unload() {
		GameObject.Destroy(_gameObject);
		_gameObject = null;
	}

	private class UpdateBehaviour : MonoBehaviour {
		private void Awake() {
			DontDestroyOnLoad(this);
		}

		private void Update() {
			if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R)) {
				GameMain.Instance.LoadScene("SceneToTitle");
				GameMain.Instance.MainCamera.FadeOut(0f);
			}
		}
	}
}
