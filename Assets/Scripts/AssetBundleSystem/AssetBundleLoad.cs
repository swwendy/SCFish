using UnityEngine;
#if UNITY_EDITOR	
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

/*
 	In this demo, we demonstrate:
	1.	Automatic asset bundle dependency resolving & loading.
		It shows how to use the manifest assetbundle like how to get the dependencies etc.
	2.	Automatic unloading of asset bundles (When an asset bundle or a dependency thereof is no longer needed, the asset bundle is unloaded)
	3.	Editor simulation. A bool defines if we load asset bundles from the project or are actually using asset bundles(doesn't work with assetbundle variants for now.)
		With this, you can player in editor mode without actually building the assetBundles.
	4.	Optional setup where to download all asset bundles
	5.	Build pipeline build postprocessor, integration so that building a player builds the asset bundles and puts them into the player data (Default implmenetation for loading assetbundles from disk on any platform)
	6.	Use WWW.LoadFromCacheOrDownload and feed 128 bit hash to it when downloading via web
		You can get the hash from the manifest assetbundle.
	7.	AssetBundle variants. A prioritized list of variants that should be used if the asset bundle with that variant exists, first variant in the list is the most preferred etc.
*/


// Class takes care of loading assetBundle and its dependencies automatically, loading variants automatically.
public class AssetBundleLoad
{
	static string m_BaseDownloadingURL = "";
    //	static AssetBundleManifest m_AssetBundleManifest = null;
#if UNITY_EDITOR
    //static int m_SimulateAssetBundleInEditor = -1;
    const string kSimulateAssetBundles = "SimulateAssetBundles";
#endif

	static Dictionary<string, AssetBundle> m_LoadedAssetBundles = new Dictionary<string, AssetBundle> ();


	// The base downloading url which is used to generate the full downloading url with the assetBundle names.
	public static string BaseDownloadingURL
	{
		get { return m_BaseDownloadingURL; }
		set { m_BaseDownloadingURL = value; }
	}

	// AssetBundleManifest object which can be used to load the dependecies and check suitable assetBundle variants.
	//public static AssetBundleManifest AssetBundleManifestObject
	//{
		//set {m_AssetBundleManifest = value; }
	//}



	// Get loaded AssetBundle, only return vaild object when all the dependencies are downloaded successfully.
	static public AssetBundle GetLoadedAssetBundle (string assetBundleName)
	{
		if(m_LoadedAssetBundles.ContainsKey(assetBundleName))
		{
			return m_LoadedAssetBundles[assetBundleName];
		}
		return null;
	}
	

	//add download assetbundle
	static public bool AddLoadAssetBundle(string assetname,AssetBundle bundle)
	{
		if(m_LoadedAssetBundles.ContainsKey(assetname))
			return true;
		m_LoadedAssetBundles.Add(assetname,bundle);
		return true;
	}

	// Unload assetbundle and its dependencies.
	static public void UnloadAssetBundle()
	{
		//Debug.Log(m_LoadedAssetBundles.Count + " assetbundle(s) in memory before unloading " + assetBundleName);

		foreach(var keyvalue in m_LoadedAssetBundles )
		{
			AssetBundle bundle = keyvalue.Value;
			bundle.Unload(false);
		}
		m_LoadedAssetBundles.Clear();
	}




} // End of AssetBundleManager.