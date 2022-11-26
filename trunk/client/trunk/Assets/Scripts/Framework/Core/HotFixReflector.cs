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
// 因为这里用到了这个类,就可以从这里为入口,找到我怎么才能把日志导出来,UnityEditor里的日志实在是太难看了
// 这时原问题是,我再定义自己的类的话所有添加日志的地方都要改?而我同样不可以打破现在两个不同域之间的日志连通
// 想要一个可以拦截unity Debug.Log的方法                
                DebugHelper.LogError(string.Format("Cant't find Class by class name:'{0}'", typeName), true);
                throw new Exception(string.Format("Cant't find Class by class name:'{0}'", typeName));
            }
            return type;
        }
        public object CreateInstance(string typeName) {
            return Activator.CreateInstance(LoadType(typeName));
        }
        public void startEducational() {
            //DoStaticMethod("HotFix.HotFixMain", "startEducational");
        }
        public void startClassical() {
            //DoStaticMethod("HotFix.HotFixMain", "startClassical");
        }
        public void startChallenging() {
            //DoStaticMethod("HotFix.HotFixMain", "startChallenging");
        }
#endregion
    }
}