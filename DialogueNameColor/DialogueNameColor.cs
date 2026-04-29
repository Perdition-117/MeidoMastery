using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using BepInEx;
using HarmonyLib;
using MaidCafe;
using Scourt.Loc;
using UnityEngine;
using static BacklogCtrl;

namespace DialogueNameColor;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
class DialogueNameColor : BaseUnityPlugin {
	private static readonly Color MainCharacterColor = new(0.7f, 0.8f, 1);
	private static readonly Color MaidColor = new(1, 0.7f, 0.7f);
	private static readonly Color NpcColor = new(1, 1, 0.7f);

	private readonly Harmony _harmony = Harmony.CreateAndPatchAll(typeof(DialogueNameColor));

	private static string _speakerTag;
	private static bool _addingMessage;

	private void OnDestroy() {
		_harmony?.UnpatchSelf();
	}

	private static Color GetCharacterColor(string name) {
		var color = NpcColor;
		if (name.Contains("[SF]")) {
			color = MainCharacterColor;
		} else if (Regex.IsMatch(name, @"\[H(F|L|LF)\d?\]")) {
			color = MaidColor;
		}
		return color;
	}

	private static void SetLabelColor(SubtitleDisplayManager subtitlesManager, Color color) {
		SetLabelColor(subtitlesManager?.charaNameUILabel, color);
		SetLabelColor(subtitlesManager?.singleCharaNameUILabel, color);
	}

	private static void SetLabelColor(UILabel label, Color color) {
		if (label != null) {
			label.color = color;
		}
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(ADVKagManager), nameof(ADVKagManager.TagTalk))]
	private static void TagTalk(ADVKagManager __instance, KagTagSupport tag_data) {
		_speakerTag = null;

		if (MaidCafeManager.isStreamingPart) return;

		if (tag_data.IsValid("name")) {
			var name = tag_data.GetTagProperty("name").AsString();
			_speakerTag = name;
			var color = GetCharacterColor(name);
			SetLabelColor(__instance.MessageWindowMgr.message_.subtitles_manager_, color);
			SetLabelColor(__instance.MessageWindowMgr.message_.name_label_, color);
		}
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(ADVKagManager), nameof(ADVKagManager.TagHitRet))]
	private static void PreTagHitRet() {
		_addingMessage = !MaidCafeManager.isStreamingPart && _speakerTag != null;
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(ADVKagManager), nameof(ADVKagManager.TagHitRet))]
	private static void PostTagHitRet() {
		_addingMessage = false;
	}

	private static void SetSpeakerColor(GameObject gameObject, BacklogUnit backlogUnit) {
		if (backlogUnit is BacklogTagUnit { m_speakerTag: { } } backlogTagUnit) {
			var color = GetCharacterColor(backlogTagUnit.m_speakerTag);

			if (Product.supportMultiLanguage) {
				var subtitleManager = gameObject.GetComponent<SubtitleDisplayManager>();
				SetLabelColor(subtitleManager, color);
			} else {
				var label = UTY.GetChildObject(gameObject, "SpeakerName").GetComponent<UILabel>();
				SetLabelColor(label, color);
			}
		}
	}

	[HarmonyTranspiler]
	[HarmonyPatch(typeof(BacklogCtrl), nameof(BacklogCtrl.CreateBacklog))]
	private static IEnumerable<CodeInstruction> CreateBacklog(IEnumerable<CodeInstruction> instructions) {
		var codeMatcher = new CodeMatcher(instructions);

		codeMatcher.MatchStartForward(new CodeMatch(OpCodes.Isinst, typeof(GameObject)));
		codeMatcher.MatchStartBackwards(new CodeMatch(OpCodes.Stloc_S));

		var backlogUnitIndex = codeMatcher.Operand;

		return codeMatcher
			.MatchStartForward(new CodeMatch(OpCodes.Stloc_0))
			.Advance(1)
			.Insert(
				new CodeInstruction(OpCodes.Ldloc_0),
				new CodeInstruction(OpCodes.Ldloc_S, backlogUnitIndex),
				new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(DialogueNameColor), nameof(SetSpeakerColor)))
			)
			.InstructionEnumeration();
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(MessageWindowMgr), nameof(MessageWindowMgr.AddBackLog))]
	private static bool AddBackLog(MessageWindowMgr __instance, IReadOnlyLocalizationString name, IReadOnlyLocalizationString text, string voice_file, int pitch, AudioSourceMgr.Type type) {
		var backlogUnit = new BacklogTagUnit {
			m_speakerName = (name == null) ? string.Empty : name[Product.Language.Japanese],
			m_speakerNameSet = name ?? new LocalizationString(),
			m_msg = text ?? new LocalizationString(),
			m_voiceId = voice_file,
			m_voicePitch = pitch,
			m_voiceType = type,
		};

		if (_addingMessage) {
			backlogUnit.m_speakerTag = _speakerTag;
		}

		__instance.m_listBacklogUnit.Add(backlogUnit);

		if (__instance.m_listBacklogUnit.Count > 30) {
			__instance.m_listBacklogUnit.RemoveAt(0);
		}

		return false;
	}

	class BacklogTagUnit : BacklogUnit {
		public string m_speakerTag;
	}
}
