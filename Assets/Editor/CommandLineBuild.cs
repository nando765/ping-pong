using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO;

public static class CommandLineBuild
{
    public static void Build()
    {
        // UnityEngine.Application.dataPath = .../Assets ; project root is one level up
        string project = Path.GetFullPath(Path.Combine(UnityEngine.Application.dataPath, ".."));
        string output = Path.Combine(project, "ejecutable");
        Directory.CreateDirectory(output);

        var scenes = new string[]
        {
            "Assets/Scenes/Menu/Menu.unity",
            "Assets/Scenes/SampleScene.unity"
        };

        var options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = Path.Combine(output, "ping-pong.exe"),
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None,
            assetBundleManifestPath = null,
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        UnityEngine.Debug.Log("BUILD RESULT: " + report.summary.result + " path=" + options.locationPathName);
        if (report.summary.result != BuildResult.Succeeded)
        {
            UnityEngine.Debug.LogError("BUILD FAILED");
            EditorApplication.Exit(1);
        }
        else
        {
            EditorApplication.Exit(0);
        }
    }
}
