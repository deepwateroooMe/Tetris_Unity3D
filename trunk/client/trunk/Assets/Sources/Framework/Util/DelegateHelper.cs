using System;
using System.Reflection;
using UnityEngine;

namespace tetris3d {
    public class DelegateHelper {
        private const string TAG = "DelegateHelper";

        private static Assembly assembly;
        
        public static void ReadDelegateSignature() {
        // public static void ReadDelegateSignature(string delegateFullName) {
        
            assembly = Assembly.GetExecutingAssembly();
 
            // Type tdType = assembly.GetType("ConsAppReflectionDelegateTest.TestDelegate");  //  "namespace.delegatename"
            Type tdType = assembly.GetType("EventManager.EventListener");  //  "namespace.delegatename"
            MethodInfo[] mInfos = tdType.GetMethods(BindingFlags.Public);
            MemberInfo[] mi = tdType.GetMember("Invoke");
            int token = mi[0].MetadataToken;
  
            MethodBase methodBase = ((MethodInfo)mi[0]);
            ParameterInfo[] parameterInfos = methodBase.GetParameters();
            MethodBody methodBody = methodBase.GetMethodBody();

            Debug.Log(TAG + " methodBase.ToString(): " + methodBase.ToString()); 
            // Console.WriteLine(methodBase.ToString());
            return;
        }    
    }
}