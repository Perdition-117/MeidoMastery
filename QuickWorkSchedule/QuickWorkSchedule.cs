using System.Linq;
using BepInEx;
using HarmonyLib;

namespace QuickWorkSchedule;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
class QuickWorkSchedule : BaseUnityPlugin {
	private readonly Harmony _harmony = Harmony.CreateAndPatchAll(typeof(QuickWorkSchedule));

	private void OnDestroy() {
		_harmony?.UnpatchSelf();
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(ScheduleTaskViewer), nameof(ScheduleTaskViewer.OnClickTaskUnit_Work))]
	private static bool OnClickTaskUnit_Work(ScheduleTaskViewer __instance, TaskUnit select_unit) {
		var workTaskUnit = (WorkTaskUnit)select_unit;

		if (workTaskUnit.work.workTyp != Schedule.ScheduleCSVData.WorkType.Basic) {
			return true;
		}

		var facilityManager = GameMain.Instance.FacilityMgr;
		var descScheduleWork = (DescScheduleWork)__instance.descDic[ScheduleTaskCtrl.TaskType.Work];
		var pendingFacilities = facilityManager.GetNextDayFacilityArray();

		facilityManager.UpdateFacilityAssignedMaidData();

		var facility = facilityManager.GetFacilityArray()
			.Where(e => e != null)
			.Where(e => e.defaultData.ID == descScheduleWork.work_data.facility.ID)
			.Where(e => !pendingFacilities.ContainsValue(e))
			.SingleOrDefault();

		if (facility == null) {
			return true;
		}

		var time = ScheduleTaskViewer.ScheduleTime;
		var maid = __instance.taskCtrl.ScheduleCtrl.SelectedMaid;

		if (facility.NowMaidCount(time) >= facility.maxMaidCount) {
			if (Product.supportMultiLanguage) {
				GameMain.Instance.SysDlg.ShowFromLanguageTerm("System/ダイアログ/この施設にはこれ以上メイドを配置出来ません。", null, SystemDialog.TYPE.OK);
			} else {
				GameMain.Instance.SysDlg.Show("この施設にはこれ以上メイドを配置出来ません。", SystemDialog.TYPE.OK);
			}
			return false;
		}

		if (facility.AllocationMaid(maid, time)) {
			__instance.OnClickTaskUnit(workTaskUnit);
		}

		return false;
	}
}
