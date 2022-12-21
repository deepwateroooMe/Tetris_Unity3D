using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System;
using System.Xml;
using System.Xml.Linq;
using System.Linq;


#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

public class SPandaPostProcessorIOS
{

	[PostProcessScene]
	static void ConfigureBuildSettings()
	{
#if UNITY_IOS
		int[] version = PlayerSettings.iOS.targetOSVersionString.Split('.').Select(x => int.Parse(x)).ToArray();

		if (version.Length < 1)
		{
			UnityEngine.Debug.LogWarningFormat("[{0}] Minimum Version not set: '{1}'", typeof(SPandaPostProcessorIOS), PlayerSettings.iOS.targetOSVersionString);
		} 

		if (version[0] < 8)
		{
			UnityEngine.Debug.LogErrorFormat("[{0}] SquarePanda requires iOS target SDK 8.0 or higher (because it requires embedded frameworks). Currently it is set to '{1}'", typeof(SPandaPostProcessorIOS), PlayerSettings.iOS.targetOSVersionString);
		}
#endif
	}

	[PostProcessBuild]
	static void PostProcessIOS(BuildTarget buildTarget, string pathToBuiltProject)
	{
		if (buildTarget == BuildTarget.iOS)
		{
#if UNITY_IOS
			//string projPath = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
			string projPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);

			UnityEngine.Debug.LogFormat("Paths: {0}\n{1}", projPath, pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj");

			PBXProject project = new PBXProject();
			project.ReadFromString(File.ReadAllText(projPath));

			string target = project.TargetGuidByName("Unity-iPhone");


			project.AddFileToBuild(target, project.AddFile("usr/lib/libz.tbd", "Frameworks/libz.tbd", PBXSourceTree.Sdk));
//			PBXSourceTree.

			string pandaPath = Application.dataPath.Replace("Assets", "") + "NativeSquarePanda";

			UnityEngine.Debug.LogFormat("[SPPostProcessor] Path to project; " + pandaPath);

			string[] folders = new string[]
			{
				"Fonts",
				"Images",
				"Plist",
				"Sounds"
			};

			foreach (var folder in folders)
			{ 
				string path = Path.Combine(pandaPath, folder);

				//CopyAndReplaceDirectory("SquarePanda/" + folder, pathToBuiltProject + "/SquarePanda/" + folder);
				//project.AddFile (path, "SquarePanda/" + folder, PBXSourceTree.Developer);
				//project.AddFolderReference (path, path);
			} 

//			CopyAndReplaceDirectory ("SquarePanda", Path.Combine (pathToBuiltProject, "SquarePanda"));
//			string spFolder = project.AddFolderReference ("SquarePanda", "SquarePanda", PBXSourceTree.Absolute);
//			string frameworkID = project.AddFile ("SquarePanda/SquarePanda.framework", "SquarePanda/SquarePanda.framework", PBXSourceTree.Source);
//			project.AddFileToBuild (target, frameworkID);
//			project.AddFileToBuild (target, spFolder); 


//			/// THIS IS THE CORRECT STUFF (Aside from coping files);
//			CopyAndReplaceDirectory ("NativeSquarePanda/SquarePanda.framework", Path.Combine (pathToBuiltProject, "SquarePanda/SquarePanda.framework"));
//			project.AddFileToBuild (target, project.AddFile ("SquarePanda/SquarePanda.framework",
//				"SquarePanda/SquarePanda.framework", PBXSourceTree.Source)); 
//
//			project.AddBuildProperty (target, "FRAMEWORK_SEARCH_PATHS", "$(PROJECT_DIR)/SquarePanda"); 

			File.WriteAllText(projPath, project.WriteToString());
#endif
		}
	}



	[PostProcessBuild(-10)]
	public static void OnPostprocessPList(BuildTarget buildTarget, string path)
	{
		if (buildTarget == BuildTarget.iOS)
		{
#if UNITY_IOS
			string plistPath = path + "/Info.plist";
			PlistDocument plist = new PlistDocument();
			plist.ReadFromString(File.ReadAllText(plistPath));

			PlistElementDict rootDict = plist.root; 


			rootDict.SetString("Game_ID", "you id goes here");
			rootDict.SetBoolean("ProductionMode", true);
			rootDict.SetBoolean("AutoSync", false);
			rootDict.SetInteger("SyncPeriod", 180);

			var fontList = rootDict.CreateArray("UIAppFonts");
			fontList.AddString("Averta-Thin.ttf");
			fontList.AddString("Averta-Light.ttf");
			fontList.AddString("Averta-Semibold.ttf");
			fontList.AddString("Averta.ttf");
			fontList.AddString("SquarePanda.ttf");
			fontList.AddString("Arrows.ttf");

			var bgModes = rootDict.CreateArray("UIBackgroundModes");
			bgModes.AddString("audio");
			bgModes.AddString("bluetooth-central");

			File.WriteAllText(plistPath, plist.WriteToString());
#endif
		}
	}

	public static void CopyAndReplaceDirectory(string srcPath, string dstPath)
	{
		if (Directory.Exists(dstPath))
			Directory.Delete(dstPath, true);
		if (File.Exists(dstPath))
			File.Delete(dstPath);
		
		Directory.CreateDirectory(dstPath);
		
		foreach (var file in Directory.GetFiles(srcPath))
		{
			File.Copy(file, Path.Combine(dstPath, Path.GetFileName(file)));
		}
		
		foreach (var dir in Directory.GetDirectories(srcPath))
		{
			CopyAndReplaceDirectory(dir, Path.Combine(dstPath, Path.GetFileName(dir)));
		}
	}

	public static void CopyAndReplaceFile(string srcPath, string dstPath)
	{
		if (Directory.Exists(dstPath))
			Directory.Delete(dstPath, true);
		if (File.Exists(dstPath))
			File.Delete(dstPath);
		
		//Directory.CreateDirectory (dstPath);
		
		File.Copy(srcPath, dstPath);
		
		//		foreach (var file in Directory.GetFiles(srcPath)) {
		//			File.Copy (file, Path.Combine (dstPath, Path.GetFileName (file)));
		//		}
		//
		//		foreach (var dir in Directory.GetDirectories(srcPath)) {
		//			CopyAndReplaceDirectory (dir, Path.Combine (dstPath, Path.GetFileName (dir)));
		//		}
	}
}
