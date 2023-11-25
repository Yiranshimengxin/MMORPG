using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

namespace GameCore
{
    public delegate void SceneLoadedHandler();
    public sealed class ResMgr : BaseMgr<ResMgr>
    {
        private SceneLoadedHandler mSceneLoadedCallback;

        public override void Init(GameObject owner)
        {
           
        }

        public T LoadResource<T>(string assetPath) where T : UnityEngine.Object
        {
            return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
        }

        public UnityEngine.Object LoadResource(String resPath)
        {
#if UNITY_EDITOR
            UnityEngine.Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(resPath);
            return obj;
#else
			LogModule.ErrorLog( "EditorLoader not have LoadResource " );
			return null;
#endif
        }

        public UnityEngine.GameObject InstantiateGameObject(String resPath, bool isActive)
        {
            UnityEngine.GameObject obj = LoadResource(resPath) as GameObject;
            if (obj != null)
            {
                GameObject go = GameObject.Instantiate(obj) as GameObject;
                if (go == null)
                {
                    return null;
                }
                go.SetActive(isActive);
                return go;

            }
            else
            {
                return null;
            }
        }


        public UnityEngine.Object LoadInstantiateObject(string assetPath, bool isActive)
        {
            return InstantiateGameObject(assetPath, isActive);
        }

        //public void LoadGameScene()
        //{
        //    SceneManager.sceneLoaded += OnSwitchSceneLoad;
        //    SceneManager.LoadCustomScene("Village");
        //    //SceneManager.LoadScene("Test");
        //}

        public void LoadCustomScene(string customSceneName, SceneLoadedHandler callback)
        {
            mSceneLoadedCallback = callback;
            SceneManager.sceneLoaded += OnCustomSceneLoad;
            var mode = LoadSceneMode.Single;
            SceneManager.LoadScene(customSceneName, mode);
        }

        private void OnSwitchSceneLoad(Scene scene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= OnSwitchSceneLoad;
            OnSceneLoad(scene, mode);
        }

        private void OnCustomSceneLoad(Scene scene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= OnCustomSceneLoad;
            OnSceneLoad(scene, mode);
        }

        private void OnSceneLoad(Scene scene, LoadSceneMode mode)
        {
            mSceneLoadedCallback();
        }
    }
}
