using System;
using System.Collections;
using __CoreGameLib._Scripts._Services._RemoteConfig;
using _Data;
using _Infrastructure;
using _Services._Localization;
using _Services._PlatformActions;
using Core._Purchasing;
using Core._Services._Saving;
using core.purchasing;
using Game.SoundManagement;
using GamePush;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Services {
    public class Bootstrapper_Project : MonoBehaviour {
        [Inject] private IPurchaser _purchaser;
        [Inject] private IDataSaver _dataSaver;
        [Inject] private Localizer _localizer;
        [Inject] private IRemoteConfig _remoteConfig;
        [Inject] private IPlatformActionProvider _langProvider;
        [Inject] private ProjectSettings _projectSettings;
        [Inject] private PlayerProgressService _playerProgressService;
        [Inject] private AudioService _audioService;
        [Inject] private PurchaseHandler _purchaseHandler;


        private IEnumerator Start() {
/*#if UNITY_EDITOR
            iapSupport = false;
#endif*/

            var start = DateTime.Now;
            var b = DateTime.Now;
            yield return InitSDK();
            yield return null;
            _localizer.Initialize();

            yield return _playerProgressService.Initialize();

            PrintTime("GP_Init.isReady", b);
            _purchaseHandler.Initialize();
            yield return _purchaser.Initialize(true);

            b = DateTime.Now;
            var rc = new RCKeysStorage();
            Debug.Log(
                $"1212 _remoteConfig.LoadConfigs(, rc == null: {rc == null}, _remoteConfig type: {_remoteConfig.GetType().FullName}");
            yield return _remoteConfig.LoadConfigs(rc);
            PrintTime("_remoteConfig.LoadConfigs()", b);

            b = DateTime.Now;
            PrintTime("_playerProgressService.Initialize()", b);

            b = DateTime.Now;
            _audioService.Initialize();

            b = DateTime.Now;
            PrintTime("GP_Game.GameReady()", b);
            PrintTime("Total time", start);

            SceneManager.LoadScene("MainScene");
        }

        private IEnumerator InitSDK() {
            if (_projectSettings.SDKType == SDK_Type.GamePush) {
                yield return new WaitUntil(() => GP_Init.isReady);
            }
        }

        private void PrintTime(string placement, DateTime startTime) {
            var a = DateTime.Now;
            var t = (a - startTime).TotalSeconds;
            print($"Bootstrapper_. Time for {placement}: {t}");
        }
    }
}
