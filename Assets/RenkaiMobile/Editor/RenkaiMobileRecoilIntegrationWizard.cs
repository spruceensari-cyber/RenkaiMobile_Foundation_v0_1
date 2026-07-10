using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using RenkaiMobile.Combat;

namespace RenkaiMobile.EditorTools
{
    public static class RenkaiMobileRecoilIntegrationWizard
    {
        [MenuItem("Renkai Mobile/Connect Mobile Recoil Stack")]
        public static void Build()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorUtility.DisplayDialog("Renkai Mobile", "Play Mode'u kapat ve tekrar çalıştır.", "OK");
                return;
            }

            GameObject player = GameObject.Find("Player");
            if (player == null)
            {
                EditorUtility.DisplayDialog("Renkai Mobile", "Player bulunamadı.", "OK");
                return;
            }

            if (player.GetComponent<DeterministicRecoilPattern>() == null) player.AddComponent<DeterministicRecoilPattern>();
            if (player.GetComponent<MobileCameraMotionStack>() == null) player.AddComponent<MobileCameraMotionStack>();
            if (player.GetComponent<MobileCameraImpulseAdapter>() == null) player.AddComponent<MobileCameraImpulseAdapter>();
            if (player.GetComponent<MobileRecoilDriver>() == null) player.AddComponent<MobileRecoilDriver>();
            if (player.GetComponent<MobileWeaponShotSignal>() == null) player.AddComponent<MobileWeaponShotSignal>();
            if (player.GetComponent<MobileRecoilShotLink>() == null) player.AddComponent<MobileRecoilShotLink>();

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveOpenScenes();
            EditorUtility.DisplayDialog("Renkai Mobile", "Mobile recoil stack bağlandı. Yeni mobile-owned weapon fire sistemi MobileWeaponShotSignal.PublishShot() çağırdığında recoil, camera motion, sight pulse ve crosshair feedback birlikte çalışacak.", "OK");
        }
    }
}
