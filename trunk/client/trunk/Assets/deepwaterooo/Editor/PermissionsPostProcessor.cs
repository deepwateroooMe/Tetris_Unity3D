using System.Collections;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

public class PermissionsPostProcessor
{
    [PostProcessBuild(-10)]
    public static void OnPostprocessPList(BuildTarget buildTarget, string path)
    {
#if UNITY_IOS
        string plistPath = path + "/Info.plist";
        PlistDocument plist = new PlistDocument();
        plist.ReadFromString(File.ReadAllText(plistPath));
		
        PlistElementDict rootDict = plist.root;
		
		
//		////////////////////////
//		// Upsight
//		////////////////////////
//		if( rootDict["NSAppTransportSecurity"] == null )
//		{
//			Debug.Log("Creating Kontagent URL Info.plist policy things.");
//			PlistElementDict transport = rootDict.CreateDict("NSAppTransportSecurity");
//			transport.SetBoolean("NSAllowsArbitraryLoads", true);
//			
//			PlistElementDict domains = transport.CreateDict("NSExceptionDomains");
//			
//			string[] urls = new string[] {
//				"test-server.kontagent.com",
//				"api.geo.kontagent.net",
//				"code.jquery.com",
//				"mobile-api.geo.kontageent.net"
//			};
//			foreach( string url in urls )
//			{
//				PlistElementDict domain = domains.CreateDict(url);
//				domain.SetBoolean("NSIncludesSubdomains", true);
//				domain.SetBoolean("NSExceptionAllowsInsecureHTTPLoads", true);
//			}
//		}
//		else
//		{
//			Debug.Log("Kontagent - NSAppTransportSecurity already exists?");
//		}
//
//		rootDict.SetString("CFBundleDevelopmentRegionKey", "en_US");

        if (rootDict.values.ContainsKey("NSCameraUsageDescription") == false)
            rootDict.SetString("NSCameraUsageDescription", "Allow access to use camera");
        if (rootDict.values.ContainsKey("NSPhotoLibraryUsageDescription") == false)
            rootDict.SetString("NSPhotoLibraryUsageDescription", "Allow access to photos");
        if (rootDict.values.ContainsKey("NSBluetoothPeripheralUsageDescription") == false)
            rootDict.SetString("NSBluetoothPeripheralUsageDescription", "Allow access to bluetooth");

        File.WriteAllText(plistPath, plist.WriteToString());

#endif
    }
}
