#if UNITY_EDITOR_OSX

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using System.IO;
using System.Collections.Generic;

#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

public class MyBuildPostprocessor
{
    ////////////////////////
	// XCode PList
	////////////////////////
	//[PostProcessBuild(-10)]
	public static void OnPostprocessPList (BuildTarget buildTarget, string path)
	{
#if UNITY_IOS
		string plistPath = path + "/Info.plist";
		PlistDocument plist = new PlistDocument ();
		plist.ReadFromString (File.ReadAllText (plistPath));
		
		PlistElementDict rootDict = plist.root;

		if (rootDict ["NSAppTransportSecurity"] == null) {
			Debug.Log ("Creating Kontagent URL Info.plist policy things.");
			PlistElementDict transport = rootDict.CreateDict ("NSAppTransportSecurity");
			transport.SetBoolean ("NSAllowsArbitraryLoads", true);
			
			PlistElementDict domains = transport.CreateDict ("NSExceptionDomains");
			
			string[] urls = new string[] {
                // These are for Upsight on iOS.
				//"test-server.kontagent.com",
				//"api.geo.kontagent.net",
				//"code.jquery.com",
				//"mobile-api.geo.kontageent.net"
			};
			foreach (string url in urls) {
				PlistElementDict domain = domains.CreateDict (url);
				domain.SetBoolean ("NSIncludesSubdomains", true);
				domain.SetBoolean ("NSExceptionAllowsInsecureHTTPLoads", true);
			}
		} else {
			Debug.Log ("XCode Plist - NSAppTransportSecurity already exists?");
		}

		//rootDict.SetString("CFBundleDevelopmentRegionKey", "en_US");

		File.WriteAllText (plistPath, plist.WriteToString ());
#endif
	}
	
	[PostProcessBuild]
	public static void OnPostprocessBuild (BuildTarget buildTarget, string path)
	{
#if UNITY_IOS
		Debug.Log("Starting XCode [General] Post Process");
			
		string projPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";

		PBXProject proj = new PBXProject ();
		proj.ReadFromString (File.ReadAllText (projPath));
		
		string target = proj.TargetGuidByName ("Unity-iPhone");

		proj.SetBuildProperty (target, "ENABLE_BITCODE", "No");
		proj.AddBuildProperty (target, "FRAMEWORK_SEARCH_PATHS", "$(PROJECT_DIR)/Frameworks");
		proj.AddBuildProperty (target, "OTHER_LDFLAGS", "-lc++");
		
		File.WriteAllText (projPath, proj.WriteToString ());
		Debug.Log ("Completed XCode [General] Post Process");
#endif
	}
}
#endif