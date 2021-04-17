﻿using UnityEngine;
    {
        if(ManifestAsssetbundle == null)
        {
            string path = GameDefine.AssetBundleSavePath + GameDefine.DependenciesAssetBundleName;

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            {
#if UNITY_IOS
            path = Application.dataPath+"/AssetBundles/IOS/IOS";
#elif UNITY_ANDROID
            path = Application.dataPath + "/AssetBundles/Android/Android";
#else
            path = Application.dataPath + "/AssetBundles/Windows/Windows";
#endif
            }
#endif
            AssetBundle ab = AssetBundle.LoadFromFile(path);
            if (ab == null)
                return false;
            ManifestAsssetbundle = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            ab.Unload(false);
            if (ManifestAsssetbundle == null)
                return false;
        }

        try
        {           
            //获取加载ab的依赖信息，参数为ab名称，如cube.ab
            string[] dependsFile = ManifestAsssetbundle.GetAllDependencies(srcABName);
            if (dependsFile.Length > 0)
            {
                string desppath = GameDefine.AssetBundleSavePath;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                if (!Luancher.UpdateWithLuncher)
                {
#if UNITY_IOS
                desppath = Application.dataPath+"/AssetBundles/IOS/";
#elif UNITY_ANDROID
                desppath = Application.dataPath + "/AssetBundles/Android/";
#else
                desppath = Application.dataPath + "/AssetBundles/Windows/";
#endif
                }

#endif
                //根据获取到的依赖信息加载所有依赖资源ab     
                for (int i = 0; i < dependsFile.Length; i++)
                {
                    if (dictAssetBundleRefs.ContainsKey(dependsFile[i]))
                        continue;                    
                    AssetBundle bundle = AssetBundle.LoadFromFile(desppath + dependsFile[i]);
                    if (bundle == null)
                        return false;

                    AssetBundleRef abRef = new AssetBundleRef(desppath, dependsFile[i], 1);
                    abRef.assetBundle = bundle;
                    AddAssetBundle(dependsFile[i], abRef);
                }
            }
        }
        catch (InvalidCastException e)
        {
            Debug.LogException(e);
        }
        return true;
    }
            return abRef.assetBundle;
        {
            //尝试加载一下
            if (!LoadAssetBundleFromLocal(GameDefine.AssetBundleSavePath, abname))
                return null;
            else
                dictAssetBundleRefs.TryGetValue(abname, out abRef);
        }
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        {
#if UNITY_IOS
            path = Application.dataPath+"/AssetBundles/IOS/";
#elif UNITY_ANDROID
            path = Application.dataPath + "/AssetBundles/Android/";
#else
            path = Application.dataPath + "/AssetBundles/Windows/";
#endif
        }
#endif

        if (dictAssetBundleRefs.ContainsKey(abname))
        //return abcr;
        //callback(abcr.assetBundle);

        //Debug.Log("加载bundle名称:" + abname + " 结束:" + DateTime.Now.Ticks);

        AssetBundleRef abRef = new AssetBundleRef(path, abname, 1);