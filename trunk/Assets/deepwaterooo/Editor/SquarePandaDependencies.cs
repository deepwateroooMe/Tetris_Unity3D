// <copyright file="GPGSDependencies.cs" company="Google Inc.">
// Copyright (C) 2015 Google Inc. All Rights Reserved.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//    limitations under the License.
// </copyright>

using System;
using System.Collections.Generic;
using UnityEditor;

/// Sample dependencies file.  Change the class name and dependencies as required by your project, then
/// save the file in a folder named Editor (which can be a sub-folder of your plugin).
///   There can be multiple dependency files like this one per project, the  resolver will combine them and process all
/// of them at once.
[InitializeOnLoad]
public class SquarePandaDependencies : AssetPostprocessor
{
#if UNITY_ANDROID
	/// <summary>Instance of the PlayServicesSupport resolver</summary>
	public static object svcSupport;
#endif  // UNITY_ANDROID

	/// Initializes static members of the class.
	static SquarePandaDependencies()
	{ 
		///
		/// NOTE:
		///
		///       UNCOMMENT THIS CALL TO MAKE THE DEPENDENCIES BE REGISTERED.
		///   THIS FILE IS ONLY A SAMPLE!!
		///

		RegisterDependencies();		
	}


	/// <summary>
	/// Registers the dependencies needed by this plugin.
	/// </summary>
	public static void RegisterDependencies()
	{
#if UNITY_ANDROID
		RegisterAndroidDependencies();
#elif UNITY_IOS
		RegisterIOSDependencies();
#endif
	}

	/// <summary>
	/// Registers the android dependencies.
	/// </summary>
	public static void RegisterAndroidDependencies()
	{
#if UNITY_ANDROID
		// Setup the resolver using reflection as the module may not be
		// available at compile time.
		Type playServicesSupport = Google.VersionHandler.FindClass("Google.JarResolver", "Google.JarResolver.PlayServicesSupport");
		if( playServicesSupport == null )
			return;

		svcSupport = svcSupport ?? Google.VersionHandler.InvokeStaticMethod(
			playServicesSupport, "CreateInstance",
			new object[] {
				"GooglePlayGames",
				EditorPrefs.GetString("AndroidSdkRoot"),
				"ProjectSettings"
			});

		/// 
		/// " https://jcenter.bintray.com "
		/// compile 'com.android.support:appcompat-v7:23.4.0'
		/// compile 'com.google.code.gson:gson:2.6.2'
		/// compile 'com.squareup.retrofit2:retrofit:2.1.0'
		/// compile 'com.squareup.retrofit2:converter-gson:2.0.2'
		/// compile 'com.squareup.okhttp3:logging-interceptor:3.3.1'
		/// compile 'com.squareup.picasso:picasso:2.5.2'
		/// compile 'uk.co.chrisjenx:calligraphy:2.2.0'
		/// 

// 我把这部分去掉了        
		// Google.VersionHandler.InvokeInstanceMethod(
		// 	svcSupport, "DependOn",
		// 	new object[] { "com.android.support", "appcompat-v7", "23.4.0" },
		// 	namedArgs: new Dictionary<string, object>() {
		// 		{ "packageIds", new string[] { "extra-android-m2repository" } }
		// 	});
		

//		// For example to depend on play-services-games version 9.6.0  you need to specify the
//		// package, artifact, and version as well as the packageId from the SDK manager in case
//		// a newer version needs to be downloaded to build.
//
//		Google.VersionHandler.InvokeInstanceMethod(
//			svcSupport, "DependOn",
//			new object[] {
//				"com.google.android.gms",
//				"play-services-games",
//				"9.6.0"
//			},
//			namedArgs: new Dictionary<string, object>() {
//				{ "packageIds", new string[] { "extra-google-m2repository" } }
//			});
//
//		// This example gets the com.android.support.support-v4 library, version 23.1 or greater.
//		// notice it is in a different package  than the play-services libraries.
//
//		Google.VersionHandler.InvokeInstanceMethod(
//			svcSupport, "DependOn",
//			new object[] { "com.android.support", "support-v4", "23.1+" },
//			namedArgs: new Dictionary<string, object>() {
//				{ "packageIds", new string[] { "extra-android-m2repository" } }
//			});
#endif
	}

	/// <summary>
	/// Registers the IOS dependencies.
	/// </summary>
	public static void RegisterIOSDependencies()
	{

		// Setup the resolver using reflection as the module may not be
		// available at compile time.
		Type iosResolver = Google.VersionHandler.FindClass(
			                   "Google.IOSResolver", "Google.IOSResolver");
		if( iosResolver == null )
		{
			return;
		}

		// Dependencies for iOS are added by referring to CocoaPods.  The libraries and frameworkds are
		//  and added to the Unity project, so they will automatically be included.
		//
		// This example add the GooglePlayGames pod, version 5.0 or greater, disabling bitcode generation.

		Google.VersionHandler.InvokeStaticMethod(
			iosResolver, "AddPod",
			new object[] { "GooglePlayGames" },
			namedArgs: new Dictionary<string, object>() {
				{ "version", "5.0+" },
				{ "bitcodeEnabled", false },
			});
	}

	// Handle delayed loading of the dependency resolvers.
	private static void OnPostprocessAllAssets(
		string[] importedAssets, string[] deletedAssets,
		string[] movedAssets, string[] movedFromPath )
	{
		foreach( string asset in importedAssets )
		{
			if( asset.Contains("IOSResolver") ||
			    asset.Contains("JarResolver") )
			{
				RegisterDependencies();
				break;
			}
		}
	}
}

