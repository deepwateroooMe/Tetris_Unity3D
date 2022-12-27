// Copyright (c) 2016 Square Panda Inc.
// All Rights Reserved.
// Dissemination, use, or reproduction of this material is strictly forbidden 
// unless prior written permission is obtained from Square Panda Inc.

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.ComponentModel;
using UnityEditor.Callbacks;
using System.Xml;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Runtime.InteropServices;

public class SPandaPostProcessorAndroid {
    private const string TAG = "SPandaPostProcessorAndroid";

    [MenuItem("SP/Process Test")]
    static void PostProcessAndroidTest() {
        //PostProcessAndroid(BuildTarget.Android, "bin/square-panda-unity-example.apk");

        //ModifyStringsResource("_TEMP");
        //ModifyOtherResources("Temp/_SquarePandaTEMP");
        //MergeNativeResources ("Temp/_SquarePandaTEMP");

        //UnpackAPK("builds/square-panda-unity-example.apk", "Temp/_TEMP");
        //RepackAPK("Temp/_TEMP","builds/square-panda-unity-example-repacked.apk" );
        //ResignAPK("builds/square-panda-unity-example.apk", "builds/test.apk"); 

//        MergeAndroidResourceXMLFiles(
//            "/Users/dillon/Unity5/square-panda-farm/Assets/NativeAssets-Android/SquarePanda/res/values/strings.xml",
//            "/Users/dillon/Unity5/square-panda-farm/Temp/strings.xml",
//            "/Users/dillon/Unity5/square-panda-farm/Temp/strings-changed.xml"
//        ); 

        RealignAPK("builds/square-panda-realign-test.apk", "builds/square-panda-realign-realigned.apk");
    }

    [MenuItem("SP/Pre-Process Test")]
    static void PreProcessAndroid() {
        /// Writing to the xml file!
        string creditsDestPath = 
            "Temp" + Path.DirectorySeparatorChar
            + "StagingArea" + Path.DirectorySeparatorChar
            + "assets" + Path.DirectorySeparatorChar
            + "credits.pdf";

        string creditsSourcePath = 
            "Assets" + Path.DirectorySeparatorChar
            + "NativeAssets-Android" + Path.DirectorySeparatorChar
            + "SquarePanda" + Path.DirectorySeparatorChar
            + "assets" + Path.DirectorySeparatorChar
            + "credits.pdf";

        CopyFile(creditsSourcePath, creditsDestPath);

        /// Writing to the xml file!
        string backgroundDestPath = 
            "Temp" + Path.DirectorySeparatorChar
            + "StagingArea" + Path.DirectorySeparatorChar
            + "res" + Path.DirectorySeparatorChar
            + "drawable" + Path.DirectorySeparatorChar
            + "game_background.png";

        string backgroundSourcePath = 
            "Assets" + Path.DirectorySeparatorChar
            + "NativeAssets-Android" + Path.DirectorySeparatorChar
            + "SquarePanda" + Path.DirectorySeparatorChar
            + "res" + Path.DirectorySeparatorChar
            + "drawable" + Path.DirectorySeparatorChar
            + "game_background.png";

        CopyFile(backgroundSourcePath, backgroundDestPath);
    }



    public static readonly string APKTOOL_PATH = "java";
    public static readonly string APKTOOL_JAR_PATH = "Assets/deepwaterooo/Tools/apktool/2.3.1/libexec/apktool_2.3.1.jar";

    [PostProcessBuild]
    static void PostProcessAndroid(BuildTarget buildTarget, string pathToBuiltProject) {

        if (buildTarget == BuildTarget.Android) {
#if UNITY_ANDROID
            //string tempPath = System.IO.Path.GetTempPath();
            string tempPath = "Temp" + Path.DirectorySeparatorChar + "_SquarePandaTEMP";

//            string repackPath = Path.GetDirectoryName(pathToBuiltProject) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(pathToBuiltProject) + "-repacked.apk";
//            string resignPath = Path.GetDirectoryName(pathToBuiltProject) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(pathToBuiltProject) + "-repacked-resigned.apk";
            string repackPath = pathToBuiltProject;
            string resignPath = pathToBuiltProject;
            string realignPath = Path.GetDirectoryName(pathToBuiltProject) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(pathToBuiltProject) + "-final.apk";

            UnityEngine.Debug.LogFormat("[{0}] PostProcess Start: {1}, {2}", typeof(SPandaPostProcessorAndroid), pathToBuiltProject, tempPath);

            if (UnpackAPK(pathToBuiltProject, tempPath) == false) { 
                CleanupAndroidResourcesPacking(tempPath);
                return;
            }

            if (ModifyStringsResource(tempPath) == false) { 
                CleanupAndroidResourcesPacking(tempPath);
                return;
            }  

            if (ModifyOtherResources(tempPath) == false) { 
                CleanupAndroidResourcesPacking(tempPath);
                return;
            } 

//            if( MergeNativeResources(tempPath) == false )
//            { 
//                CleanupAndroidResourcesPacking(tempPath);
//                return;
//            }

            if (RepackAPK(tempPath, repackPath) == false) { 
                CleanupAndroidResourcesPacking(tempPath);
                return;
            }

            if (ResignAPK(repackPath, resignPath) == false) {
                CleanupAndroidResourcesPacking(tempPath);
                return;
            }

            if (RealignAPK(repackPath, realignPath) == false) { 
                CleanupAndroidResourcesPacking(tempPath);
                return;
            }

            CleanupAndroidResourcesPacking(tempPath);
#endif
        }
    }

    static bool UnpackAPK(string apkPath, string destPath) {
        Debug.Log(TAG + " UnpackAPK()");
        string output = "";
        string error = "";
        using (System.Diagnostics.Process p = new System.Diagnostics.Process()) { 
            System.Diagnostics.ProcessStartInfo rubyProc = new System.Diagnostics.ProcessStartInfo(APKTOOL_PATH); 
            rubyProc.Arguments = string.Format("-jar {0} {1}", APKTOOL_JAR_PATH, string.Format("decode -f \"{0}\" -o \"{1}\"", apkPath, destPath)); 
            rubyProc.RedirectStandardInput = true; 
            rubyProc.RedirectStandardError = true;
            rubyProc.RedirectStandardOutput = true; 
            rubyProc.UseShellExecute = false;
            rubyProc.CreateNoWindow = true;

            p.StartInfo = rubyProc;
            Debug.Log(p.StartInfo);
            p.Start();

            output = p.StandardOutput.ReadToEnd();
            error = p.StandardError.ReadToEnd(); 
        } 

        if (string.IsNullOrEmpty(error) == false) {
            UnityEngine.Debug.LogErrorFormat("[{0}] Unpack Failed:\n{2}", typeof(SPandaPostProcessorAndroid), output, error);
            return false;
        }

        UnityEngine.Debug.LogFormat("[{0}] Unpack Complete:\n{1}", typeof(SPandaPostProcessorAndroid), output, error);

        return true;
    }

    static bool RepackAPK(string srcPath, string destApk) {
        Debug.Log(TAG + " RepackAPK()");
        string output = "";
        string error = "";

        using (System.Diagnostics.Process p = new System.Diagnostics.Process()) { 
            System.Diagnostics.ProcessStartInfo rubyProc = new System.Diagnostics.ProcessStartInfo(APKTOOL_PATH); 
            rubyProc.Arguments = string.Format("-jar {0} {1}", APKTOOL_JAR_PATH, string.Format("build -f \"{0}\" -o \"{1}\"", srcPath, destApk));
            rubyProc.RedirectStandardInput = true; 
            rubyProc.RedirectStandardError = true;
            rubyProc.RedirectStandardOutput = true; 
            rubyProc.UseShellExecute = false; 

            p.StartInfo = rubyProc; 
            p.Start(); 

            output = p.StandardOutput.ReadToEnd(); 
            error = p.StandardError.ReadToEnd(); 
        } 

        if (string.IsNullOrEmpty(error) == false) {
            UnityEngine.Debug.LogErrorFormat("[{0}] Repack Failed:\n{2}", typeof(SPandaPostProcessorAndroid), output, error);
            return false;
        }

        UnityEngine.Debug.LogFormat("[{0}] Repack Complete:\n{1}", typeof(SPandaPostProcessorAndroid), output, error);

        return true;
    }

    static bool ResignAPK(string srcApk, string destApk) {
        Debug.Log(TAG + " ResignAPK()");
        string keystoreName;
        string keystorePass;
        string keyaliasPass;
        string keyaliasName;

        if (string.IsNullOrEmpty(PlayerSettings.Android.keystoreName) ||
            string.IsNullOrEmpty(PlayerSettings.Android.keystorePass) ||
            string.IsNullOrEmpty(PlayerSettings.Android.keyaliasPass) ||
            string.IsNullOrEmpty(PlayerSettings.Android.keyaliasName)) {
            UnityEngine.Debug.LogWarningFormat("[{0}] PlayerSettings Keystore not configured, using debug", typeof(SPandaPostProcessorAndroid));

#if UNITY_EDITOR_OSX
            string home = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            keystoreName = home + "/.android/debug.keystore";
#else
            throw new Exception("Keystore not programmed for non-osx environments");
#endif
            keystorePass = "android";
            keyaliasPass = "android";
            keyaliasName = "androiddebugkey";
        } else {
            keystoreName = PlayerSettings.Android.keystoreName;
            keystorePass = PlayerSettings.Android.keystorePass;
            keyaliasPass = PlayerSettings.Android.keyaliasPass;
            keyaliasName = PlayerSettings.Android.keyaliasName;
        }

        string output = "";
        string error = "";

        using (System.Diagnostics.Process p = new System.Diagnostics.Process()) { 
        	System.Diagnostics.ProcessStartInfo rubyProc = new System.Diagnostics.ProcessStartInfo("jarsigner"); 
			///jarsigner -keystore ~/.android/debug.keystore -storepass android -keypass android $REPACK_DIR/dist/$file androiddebugkey
			rubyProc.Arguments = string.Format(
				//"-keystore \"{0}\" -storepass {1} -keypass {2} -signedjar \"{3}\" {4} \"{5}\"", 
				"-keystore \"{0}\" -storepass {1} -keypass {2} \"{4}\" \"{5}\"", 
				keystoreName,
				keystorePass,
				keyaliasPass,
				destApk,
				srcApk,
				keyaliasName);

			UnityEngine.Debug.LogFormat("[{0}] Resign Test: {1}", typeof(SPandaPostProcessorAndroid), rubyProc.FileName + " " + rubyProc.Arguments);

			rubyProc.RedirectStandardInput = true; 
			rubyProc.RedirectStandardError = true;
			rubyProc.RedirectStandardOutput = true; 
			rubyProc.UseShellExecute = false; 

			p.StartInfo = rubyProc; 
			p.Start(); 

			output = p.StandardOutput.ReadToEnd(); 
			error = p.StandardError.ReadToEnd(); 
		} 

		if (string.IsNullOrEmpty(error) == false) {
			UnityEngine.Debug.LogErrorFormat("[{0}] Resign Failed: {2}", typeof(SPandaPostProcessorAndroid), output, error);
			return false;
		}

		UnityEngine.Debug.LogFormat("[{0}] Resign Complete: {1}", typeof(SPandaPostProcessorAndroid), output, error);

		return true;
	}

	public static void SortVersionsArray(string[] arr) {
		Array.Sort(arr, (string a, string b) => {
            var versionA = Path.GetFileName(a).Split('.').Select(x => int.Parse(x)).ToArray();
            var versionB = Path.GetFileName(b).Split('.').Select(x => int.Parse(x)).ToArray();

            // -1 means b is greater, 1 means a is greater
            if (versionA[0] < versionB[0])
                return 1;
            else if (versionA[0] > versionB[0])
                return -1;
            else if (versionA[1] < versionB[1])
                return 1;
            else if (versionA[1] > versionB[1])
                return -1;
            else if (versionA[2] < versionB[2])
                return 1;
            else if (versionA[2] > versionB[2])
                return -1;
            return 0;
		});
	}

	static bool RealignAPK(string srcApk, string destApk) {
		string androidHome = EditorPrefs.GetString("AndroidSdkRoot");

		if (string.IsNullOrEmpty(androidHome)) {
			UnityEngine.Debug.LogErrorFormat("[{0}] Realign Failed: Android SDK Path not set in preferences.", typeof(SPandaPostProcessorAndroid));
			return false;
		}
		string buildtoolsPath = Path.Combine(androidHome, "build-tools");

		string[] buildtoolsVersions = Directory.GetDirectories(buildtoolsPath);
		if (buildtoolsVersions.Length <= 0) {
			UnityEngine.Debug.LogErrorFormat("[{0}] Realign Failed: no build-tools found in Android SDK.", typeof(SPandaPostProcessorAndroid));
			return false;
		}
		SortVersionsArray(buildtoolsVersions); 
		string buildtoolsVersion = buildtoolsVersions[0];

		string output = "";
		string error = "";

		using (System.Diagnostics.Process p = new System.Diagnostics.Process()) { 
			string zipalignBin = Path.Combine(Path.Combine(buildtoolsPath, buildtoolsVersion), "zipalign");
			System.Diagnostics.ProcessStartInfo zipalignProc = new System.Diagnostics.ProcessStartInfo(zipalignBin); 
			/// zipalign -v 4 <PATH/TO/YOUR_SIGNED_PROJECT.apk> <PATH/TO/YOUR_SIGNED_AND_ALIGNED_PROJECT.apk>
			zipalignProc.Arguments = string.Format(
				"-v 4 \"{0}\" \"{1}\"",
				srcApk,
				destApk
                );

			UnityEngine.Debug.LogFormat("[{0}] Realign Test: {1}", typeof(SPandaPostProcessorAndroid), zipalignProc.FileName + " " + zipalignProc.Arguments);

			zipalignProc.RedirectStandardInput = true; 
			zipalignProc.RedirectStandardError = true;
			zipalignProc.RedirectStandardOutput = true; 
			zipalignProc.UseShellExecute = false; 

			p.StartInfo = zipalignProc; 
			p.Start(); 

			output = p.StandardOutput.ReadToEnd(); 
			error = p.StandardError.ReadToEnd(); 
		} 

		if (string.IsNullOrEmpty(error) == false) {
			UnityEngine.Debug.LogErrorFormat("[{0}] Realign Failed: {2}", typeof(SPandaPostProcessorAndroid), output, error);
			return false;
		}

		File.Delete(srcApk); 
		File.Move(destApk, srcApk);

		UnityEngine.Debug.LogFormat("[{0}] Realign Complete: {1}", typeof(SPandaPostProcessorAndroid), output, error);

		return true;
	}

	public class NativeResourcesList {
		public List<NativeResourceFile> _files = new List<NativeResourceFile>();

		public void SearchDirectory(string root) {
			AddDirectory(root, root);
		}

		private void AddDirectory(string root, string currentDir) {
			foreach (var file in Directory.GetFiles(currentDir)) {
				if (file.EndsWith(".meta"))
					continue;

				var fileInfo = new NativeResourceFile(root, file);
				if (file.EndsWith(".xml"))
					fileInfo.Action = NativeResourceFile.NativeResourceFileType.Merge;
				
				_files.Add(fileInfo);
			}

			foreach (var dir in Directory.GetDirectories(currentDir)) {
				this.AddDirectory(root, dir);
			}
		}
	}

	public class NativeResourceFile {
		public enum NativeResourceFileType {
			Overwrite,
			Merge,
			Skip}

		;

		protected NativeResourceFileType _action;

		protected string _path;
		protected string _root;

		public string Root { get { return _root; } }

		public string AbsolutePath { get { return _path; } }

		public string LocalPath { get { return _path.Replace(_root + System.IO.Path.DirectorySeparatorChar, ""); } }

		public NativeResourceFileType Action {
			get { return _action; }
			set {
				if (value == NativeResourceFileType.Merge) {
					if (_path.EndsWith(".xml"))
						_action = value;
					else
						UnityEngine.Debug.LogError("[NativeAssets-Android] Only XML files can be merged");
				} else { 
					_action = value;
				}
			}
		}

		public NativeResourceFile(string root, string path) {
			_root = root;
			_path = path; 
		}
	}

	static bool MergeNativeResources(string unpackedPath) { 
		NativeResourcesList list = GetListOfNativeResources(); 

		foreach (var fileInfo in list._files) {
			switch (fileInfo.Action) {
            case NativeResourceFile.NativeResourceFileType.Overwrite: 
                CopyFile(fileInfo.AbsolutePath, Path.Combine(unpackedPath, fileInfo.LocalPath));
                break;
            case NativeResourceFile.NativeResourceFileType.Merge:
                MergeAndroidResourceXMLFiles(fileInfo.AbsolutePath, Path.Combine(unpackedPath, fileInfo.LocalPath));
                break;
			}
		}

		return true;
	}

	static void MergeAndroidResourceXMLFiles(string fromFile, string toFile, string resultFile = null) {
		if (string.IsNullOrEmpty(resultFile))
			resultFile = toFile;
		
		Debug.LogWarningFormat("MERGING {0} {1}", fromFile, toFile);
//		var srcDoc = XDocument.Load (srcFile);
//		var destDoc = XDocument.Load (destFile);

//		var intersect = srcDoc.Elements().Intersect(destDoc.Elements());
//		var intersect = srcDoc.Elements().(destDoc.Elements());
//
//		var combinedUnique = srcDoc.Descendants("AllNodes")
//			.Union(destDoc.Descendants("AllNodes"));

//		var u = srcDoc.Elements().Union(destDoc);
//		destDoc.Root.RemoveNodes();
//		destDoc.Add(u);
//
//		//srcDoc.Descendants().First().na;
//		var root = new XElement("resources",
//			from destResource in destDoc.Elements()
//			join srcResource in srcDoc.Elements()
//			on 
//			(string)destResource.Attribute("name")
//			equals 
//			(string)srcResource.Attribute("name")
//			select new XElement(destResource.Name,
//				new XAttribute("name", (string)srcResource.Attribute("name")),
//				destResource.Value
//			)
//		);
//
//		Console.WriteLine(root);
//		Debug.Log(root);

		XmlDocument toDoc = new XmlDocument();
		toDoc.Load(toFile);

		XmlDocument fromDoc = new XmlDocument();
		fromDoc.Load(fromFile);

		var fromRoot = fromDoc.SelectSingleNode("//resources");
		var toRoot = toDoc.SelectSingleNode("//resources");

		if (fromRoot == null) {
			Debug.LogErrorFormat("[{0}] MergeAndroidResourceXMLFiles FAILED. From File not a proper resource format: {1}", typeof(SPandaPostProcessorAndroid), fromFile);
			return;
		}

		if (toRoot == null) {
			Debug.LogErrorFormat("[{0}] MergeAndroidResourceXMLFiles FAILED. From File not a proper resource format: {1}", typeof(SPandaPostProcessorAndroid), toFile);
			return;
		}

		var fromResources = fromRoot.ChildNodes;
		for (int i = 0; i < fromResources.Count; i++) {
			XmlNode fromResource = fromResources[i];

			var queryString = string.Format("{0}[@name='{1}']", fromResource.Name, fromResource.Attributes["name"].Value);
			var queryNode = toRoot.SelectSingleNode(queryString);

			if (queryNode != null) {
				//Debug.Log("removing existing node...: " + queryNode.OuterXml);
				toRoot.RemoveChild(queryNode);
			} else {
				//Debug.Log("adding new node: " + queryString);
			} 

			toRoot.AppendChild(toDoc.ImportNode(fromResource, true));
		}

		toDoc.Save(resultFile);
	}

	static NativeResourcesList GetListOfNativeResources() {
		NativeResourcesList list = new NativeResourcesList();

		string nativeRoot = Application.dataPath + Path.DirectorySeparatorChar + "NativeAssets-Android";
		foreach (var dir in Directory.GetDirectories(nativeRoot)) {
			list.SearchDirectory(dir);
		}

		return list;
	}

	static bool ModifyOtherResources(string unpackedPath) {
		/// Writing to the xml file!
		string creditsDestPath = 
			unpackedPath + Path.DirectorySeparatorChar
			+ "assets" + Path.DirectorySeparatorChar
			+ "credits.pdf";
 
		string creditsSourcePath = 
			"Assets" + Path.DirectorySeparatorChar
			+ "NativeAssets-Android" + Path.DirectorySeparatorChar
			+ "SquarePanda" + Path.DirectorySeparatorChar
			+ "assets" + Path.DirectorySeparatorChar
			+ "credits.pdf";

		if (CopyFile(creditsSourcePath, creditsDestPath) == false)
			return false; 
		
		/// Writing to the xml file!
		string backgroundDestPath = 
			unpackedPath + Path.DirectorySeparatorChar
			+ "res" + Path.DirectorySeparatorChar
			+ "drawable" + Path.DirectorySeparatorChar
			+ "game_background.png";

		string backgroundSourcePath = 
			"Assets" + Path.DirectorySeparatorChar
			+ "NativeAssets-Android" + Path.DirectorySeparatorChar
			+ "SquarePanda" + Path.DirectorySeparatorChar
			+ "res" + Path.DirectorySeparatorChar
			+ "drawable" + Path.DirectorySeparatorChar
			+ "game_background.png";

		if (CopyFile(backgroundSourcePath, backgroundDestPath) == false)
			return false; 

		return true;
	}

	static bool CopyFile(string srcFile, string destFile) {
		if (File.Exists(srcFile) == false) {
			UnityEngine.Debug.LogErrorFormat("[{0}] Unable to copy {1}. File does not exist", typeof(SPandaPostProcessorAndroid), srcFile); 
			return false;
		}

		if (Directory.Exists(Path.GetDirectoryName(destFile)) == false) {
//			UnityEngine.Debug.LogErrorFormat ("[{0}] Unable to copy {1}. Path does not exist: '{2}'", typeof(SPandaPostProcessorAndroid), srcFile, destFile); 
//			return false;

			Directory.CreateDirectory(Path.GetDirectoryName(destFile));
		}

		File.Copy(srcFile, destFile, true);

		return true;
	}

	static bool ModifyStringsResource(string unpackedPath) { 
		/// Writing to the xml file!
		string stringsPath = 
			unpackedPath + Path.DirectorySeparatorChar
			+ "res" + Path.DirectorySeparatorChar
			+ "values" + Path.DirectorySeparatorChar
			+ "strings.xml";

		if (File.Exists(stringsPath) == false) {
			UnityEngine.Debug.LogErrorFormat("[{0}] Unable to read strings.xml:\n{1}", typeof(SPandaPostProcessorAndroid), stringsPath);
			return false;
		} 
 
		string mystringsPath = 
			"Assets" + Path.DirectorySeparatorChar
			+ "NativeAssets-Android" + Path.DirectorySeparatorChar
			+ "SquarePanda" + Path.DirectorySeparatorChar
			+ "res" + Path.DirectorySeparatorChar
			+ "values" + Path.DirectorySeparatorChar
			+ "strings.xml";

		if (File.Exists(mystringsPath) == false) {
			UnityEngine.Debug.LogWarningFormat("[{0}] '{1}' Doesn't exist. Creating...\nPlease populate this file with your required values.", typeof(SPandaPostProcessorAndroid), mystringsPath);

			XmlDocument newSquarePandaStringsXml = new XmlDocument();

			var declaration = newSquarePandaStringsXml.CreateXmlDeclaration("1.0", "utf-8", null);
			newSquarePandaStringsXml.AppendChild(declaration);
			
			var root = newSquarePandaStringsXml.CreateNode(XmlNodeType.Element, "resources", "");
			newSquarePandaStringsXml.AppendChild(root); 

			XmlNode stringNode;
			XmlAttribute stringNodeName;

			stringNode = newSquarePandaStringsXml.CreateNode(XmlNodeType.Element, "string", "");
			stringNodeName = newSquarePandaStringsXml.CreateAttribute("name");
			stringNode.Attributes.Append(stringNodeName);
			stringNode.InnerText = "com.squarepanda.activity";
			stringNodeName.InnerText = "game_activity";
			root.AppendChild(stringNode);

			stringNode = newSquarePandaStringsXml.CreateNode(XmlNodeType.Element, "string", "");
			stringNodeName = newSquarePandaStringsXml.CreateAttribute("name");
			stringNode.Attributes.Append(stringNodeName);
			stringNode.InnerText = "ID_GOES_HERE";
			stringNodeName.InnerText = "gameID";
			root.AppendChild(stringNode); 

			stringNode = newSquarePandaStringsXml.CreateNode(XmlNodeType.Element, "string", "");
			stringNodeName = newSquarePandaStringsXml.CreateAttribute("name");
			stringNode.Attributes.Append(stringNodeName);
			stringNode.InnerText = "false";
			stringNodeName.InnerText = "Enable_Production";
			root.AppendChild(stringNode);

			newSquarePandaStringsXml.Save(mystringsPath);

			return false;
		} 

		XmlDocument destStringsXml = new XmlDocument();
		destStringsXml.Load(stringsPath); 

		XmlDocument srcStringsXML = new XmlDocument();
		srcStringsXML.Load(mystringsPath);

		SetStringsXmlString(destStringsXml, "game_activity", GetStringsXmlString(srcStringsXML, "game_activity"));
		SetStringsXmlString(destStringsXml, "gameID", GetStringsXmlString(srcStringsXML, "gameID"));
		SetStringsXmlString(destStringsXml, "Enable_Production", GetStringsXmlString(srcStringsXML, "Enable_Production"));

		destStringsXml.Save(stringsPath);

		return true;
	}

	static string GetStringsXmlString(XmlDocument stringsXML, string name) {
		var node = stringsXML.SelectSingleNode(string.Format("//string[@name='{0}']", name));

		if (node == null) {
			UnityEngine.Debug.LogWarningFormat("[{0}] SetStringsXmlString:\n[{1}], {3}", typeof(SPandaPostProcessorAndroid), name, node, node.InnerText); 
			return null;
		}

		return node.InnerText;
	}

	static void SetStringsXmlString(XmlDocument stringsXML, string name, string str) { 
		var node = stringsXML.SelectSingleNode(string.Format("//string[@name='{0}']", name));

		/// If node doesn't exist yet, create it.
		if (node == null) {
			node = stringsXML.CreateNode("string", "string", "");
			var attr = stringsXML.CreateAttribute("name");
			node.Attributes.Append(attr);
			stringsXML.SelectSingleNode("/resources").AppendChild(node);
		} 

		node.InnerText = str;

		UnityEngine.Debug.LogFormat("[{0}] SetStringsXmlString:\n[{1}], {3}", typeof(SPandaPostProcessorAndroid), name, node, node.InnerText);
	}

	static void CleanupAndroidResourcesPacking(string tempPath) {
		//System.IO.Directory.Delete(tempPath); 
	}
}
