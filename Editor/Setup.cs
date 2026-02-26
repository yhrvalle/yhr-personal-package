using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using static System.IO.Directory;
using static System.IO.Path;
using static UnityEditor.AssetDatabase;
using Application = UnityEngine.Application;

namespace PersonalPackage.Editor
{
    public static class Setup
    {
        [MenuItem("Tools/Create Default Folders")]
        public static void CreateDefaultFoldersStructure()
        {
            Folders.CreateDefault("_Project", "Animation", "Art", "Materials", "Prefabs", "ScriptableObjects", "Scripts");
            Refresh();
            Folders.Move("_Project/", "Scenes");
            Folders.Move("_Project/", "Settings");
            Folders.Delete("TutorialInfo");
            Refresh();
            DeleteAsset("Assets/Readme.asset");
            DeleteAsset("Assets/InputSystem_Actions.inputactions"); // Just copy the personal one from the Packages folder to the Settings
            Refresh();
        }

        [MenuItem("Tools/Setup/Import Cinemachine")]
        public static void ImportCinemachine()
        {
            Packages.InstallPackages(new[] { "com.unity.cinemachine" });
        }

        [MenuItem("Tools/Setup/Import PrimeTween Asset")]
        public static void ImportPrimeTweenAsset()
        {
            Assets.ImportAssets("PrimeTween High-Performance Animations and Sequences.unitypackage", "Kyrylo Kuzyk/Editor ExtensionsAnimation");
        }

        [MenuItem("Tools/Setup/Import BetterHierarchy Asset")]
        public static void ImportBetterHierarchyAsset()
        {
            Assets.ImportAssets("Better Hierarchy.unitypackage", "Toaster Head/Editor ExtensionsUtilities");
            
        }

        [MenuItem("Tools/Setup/Import DarkMode Unity Asset")]
        public static void ImportDarkModeUnityAsset()
        {
            Assets.ImportAssets("DarkMode for Unity Editor on Windows.unitypackage", "0x7c13/ScriptingGUI");
            Refresh();
        }

        [MenuItem("Tools/Setup/Import OpenSource SceneRef Attributes")]
        public static void ImportOpenSourceSceneRefAttributes()
        {
            Packages.InstallPackages(new[] { "https://github.com/KyleBanks/scene-ref-attribute.git" });
        }
        
        [MenuItem("Tools/Import Entalphia")]
        public static void ImportEntalphia()
        {
            string homePath =  Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string folderPath = Combine(homePath, "Documents");
            const string folder = "Entalphia Package";
            const string assetName = "entalphia.unitypackage";
            ImportPackage(Combine(folderPath, folder, assetName), false);
        }

        private static class Folders
        {
            public static void CreateDefault(string root, params string[] folders)
            {
                string fullPath = Combine(Application.dataPath, root);
                foreach (string folder in folders)
                {
                    string path = Combine(fullPath, folder);
                    if (!Exists(path)) CreateDirectory(path);
                }
            }

            public static void Move(string newParent, string folderName)
            {
                string sourcePath = $"Assets/{folderName}";
                if (IsValidFolder(sourcePath))
                {
                    string destinationPath = $"Assets/{newParent}/{folderName}";
                    string error = MoveAsset(sourcePath, destinationPath);
                    if (!string.IsNullOrEmpty(error)) Debug.LogError($"Failed to move folder '{folderName}': {error}");
                }
            }

            public static void Delete(string folderName)
            {
                string pathToDelete = $"Assets/{folderName}";
                if (IsValidFolder(pathToDelete)) DeleteAsset(pathToDelete);
            }
        }

        private static class Packages
        {
            private static AddRequest s_request;
            private static readonly Queue<string> PackageToAdd = new();

            public static void InstallPackages(string[] packages)
            {
                foreach (string package in packages)
                {
                    PackageToAdd.Enqueue(package);
                }

                if (PackageToAdd.Count > 0)
                {
                    s_request = Client.Add(PackageToAdd.Dequeue());
                    EditorApplication.update += Progress;
                }
            }

            private static async void Progress()
            {
                if (s_request.IsCompleted)
                {
                    if (s_request.Status == StatusCode.Success)
                        Debug.Log("Installed: " + s_request.Result.packageId);
                    else if (s_request.Status >= StatusCode.Failure) Debug.Log(s_request.Error.message);
                    EditorApplication.update -= Progress;
                    if (PackageToAdd.Count > 0)
                    {
                        await Task.Delay(1000);
                        s_request = Client.Add(PackageToAdd.Dequeue());
                        EditorApplication.update += Progress;
                    }
                }
            }
        }

        private static class Assets
        {
            public static void ImportAssets(string asset, string subfolder)
            {
                string homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                string rootFolder = Combine(homePath, ".local/share/unity3d/Asset Store-5.x");
                ImportPackage(Combine(rootFolder, subfolder, asset), false);
                
            }
        }
    }
}
