#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace com.akihiro.dependencies_importer
{
    public class dependencies_importer
    {
        private static List<string> PackageURLs = new List<string> {
            // GitHub, Azure DevOps, Unity, ローカルのUnity Package Manager用URLを追加
            "git+https://github.com/akihiro0105/UnityPackageManagerEditor.git?path=/Assets/com.akihiro.upmeditor/"
        };

        #region Importer
        private static AddRequest request;

        [InitializeOnLoadMethod]
        public static void LoadPackage()
        {
            Debug.Log($"Import {System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}");
            EditorApplication.LockReloadAssemblies();
            importList();
        }
        private static void importList()
        {
            var url = PackageURLs.FirstOrDefault();
            if (!string.IsNullOrEmpty(url))
            {
                PackageURLs.RemoveAt(0);
                request = Client.Add(url);
                EditorApplication.update += progress;
            }
            else
            {
#if UNITY_2020_3_OR_NEWER
                Client.Resolve();
#endif
                EditorApplication.UnlockReloadAssemblies();
                Debug.Log($"Complete {System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}");
            }
        }
        private static void progress()
        {
            if (request.IsCompleted)
            {
                if (request.Status == StatusCode.Failure)
                {
                    Debug.LogError($"Failure {request.Error.message}");
                }
                EditorApplication.update -= progress;
                importList();
            }
        }
        #endregion
    }
}
#endif
