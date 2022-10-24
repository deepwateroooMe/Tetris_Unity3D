using Framework.ResMgr;
using Framework.Util;
using System;
using System.Reflection;
using UnityEngine;
using System.Linq;

namespace Framework.Core {

    public class HotFixReflector : SingletonMono<HotFixReflector>, IHotFixMain {

        public static Assembly assembly;
        void Start() {
            ResourceConstant.Loader.LoadAssetAsyn<TextAsset>("HotFix.dll", "HotFix.dll",
                                                             LoadHotFixDllSuccess,
                                                             EAssetBundleUnloadLevel.ChangeSceneOver);
        }

        public void Update() { // place holder
        }

        void LoadHotFixDllSuccess(TextAsset dllAsset) {
            if (GameApplication.Instance.usePDB) {
                ResourceConstant.Loader.LoadAssetAsyn<TextAsset>("HotFix.pdb", "HotFix.pdb", (pdbAsset) => {
                    assembly = Assembly.Load(dllAsset.bytes, pdbAsset.bytes);
                    StartApplication();
                }, EAssetBundleUnloadLevel.ChangeSceneOver);
            } else {
                assembly = AppDomain.CurrentDomain.Load(dllAsset.bytes);
                StartApplication();
            }
        }

        void StartApplication() {
            try {
                Type hotfixMainType = assembly.GetType("HotFix.HotFixMain");
                MethodInfo startMethod = hotfixMainType.GetMethod("Start");
                startMethod.Invoke(null, null);
            }
            catch (Exception e) {
                string errorMessage = string.Empty;
                if (e.InnerException != null) {
                    errorMessage = e.InnerException.Message + e.InnerException.StackTrace;
                } else {
                    errorMessage = e.Message + e.StackTrace;
                }
                DebugHelper.LogError(errorMessage, true);
            }
        }
#region Override
        public Type LoadType(string typeName) {
            Type type = assembly.GetTypes().FirstOrDefault(t => t.FullName == typeName);
            if (type == null) {
                DebugHelper.LogError(string.Format("Cant't find Class by class name:'{0}'", typeName), true);
                throw new Exception(string.Format("Cant't find Class by class name:'{0}'", typeName));
            }
            return type;
        }
        public object CreateInstance(string typeName) {
            return Activator.CreateInstance(LoadType(typeName));
        }
#endregion
    }
}