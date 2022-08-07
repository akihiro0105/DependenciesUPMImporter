#if UNITY_EDITOR
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
        private static string[] PackageURLs ={
            // GitHub, Azure DevOps, Unity, ローカルのUnity Package Manager用URLを追加
            "git+https://github.com/akihiro0105/UnityPackageManagerEditor.git?path=/Assets/com.akihiro.upmeditor/"
        };

        #region Importer
        private static ListRequest listRequest;
        private static List<string> urlList = new List<string>();
        private static AddRequest addRequest;

        [InitializeOnLoadMethod]
        public static void LoadPackage()
        {
            listRequest = Client.List(true);
            EditorApplication.update += listProgress;
        }

        private static void listProgress()
        {
            if (!listRequest.IsCompleted) return;
            EditorApplication.update -= listProgress;
            if (listRequest.Status == StatusCode.Failure) Debug.LogError($"Failure List {listRequest.Error.message}");
            else
            {
                urlList = PackageURLs.Where(url => listRequest.Result.Where(item => item.packageId.Contains(url)).Count() == 0).ToList();
                if (urlList.Count > 0)
                {
                    Debug.Log($"Import {System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}");
                    EditorApplication.LockReloadAssemblies();
                    addPackage();
                }
            }
        }

        private static void addPackage()
        {
            if (urlList.Count > 0)
            {
                Debug.Log($"Add {urlList[0]}");
                addRequest = Client.Add(urlList[0]);
                urlList.RemoveAt(0);
                EditorApplication.update += addProgress;
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

        private static void addProgress()
        {
            if (!addRequest.IsCompleted) return;
            EditorApplication.update -= addProgress;
            if (addRequest.Status == StatusCode.Failure) Debug.LogError($"Failure Add {addRequest.Error.message}");
            addPackage();
        }
        #endregion
    }
}
#endif
