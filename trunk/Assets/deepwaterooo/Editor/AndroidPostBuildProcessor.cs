using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor.Android;
using UnityEngine;

namespace Assets.deepwaterooo.Editor {
    public class AndroidPostBuildProcessor : IPostGenerateGradleAndroidProject {
        public int callbackOrder {
            get {
                return 999;
            }
        }

        void IPostGenerateGradleAndroidProject.OnPostGenerateGradleAndroidProject(string path) {
                Debug.Log("Bulid path : " + path);
                string gradlePropertiesFile = path + "/gradle.properties";
                if (File.Exists(gradlePropertiesFile)) {
                    File.Delete(gradlePropertiesFile);
                }
                StreamWriter writer = File.CreateText(gradlePropertiesFile);
                writer.WriteLine("org.gradle.jvmargs=-Xmx8192M");
                writer.WriteLine("android.useAndroidX=false");
                writer.WriteLine("android.enableJetifier=false");
                writer.Flush();
                writer.Close();
            }
    }
}
