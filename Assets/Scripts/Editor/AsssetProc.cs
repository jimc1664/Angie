using UnityEngine;
using System.Collections;
using UnityEditor;

public class AssetProc : AssetPostprocessor {

    static void OnPostprocessAllAssets(
        string[] importedAssets,
        string[] deletedAssets,
        string[] movedAssets,
        string[] movedFromAssetPaths) { 


       // foreach(string str in deletedAssets)
       //     Debug.Log("Deleted Asset: " + str);
        foreach(string str in importedAssets) {      
            Debug.Log("importedAssets Asset: " + str);

            if(str == "Assets/Plugins/x64/in/space_cpp.dll") {

                World.reimport();
            }
        }

      //  for(int i = 0; i < movedAssets.Length; i++)
       //     Debug.Log("Moved Asset: " + movedAssets[i] + " from: " + movedFromAssetPaths[i]);
    }
}