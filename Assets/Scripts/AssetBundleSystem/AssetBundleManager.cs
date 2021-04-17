using UnityEngine;using System;using System.Collections;using System.Collections.Generic;using System.IO;static public class AssetBundleManager{    /// <summary>    /// 保存AssetBundle资源包引用的容器，其中key值是下载地址+版本号的组合，value值是AssetBundleRef类数据    /// </summary>    static private Dictionary<string, AssetBundleRef> dictAssetBundleRefs;    static AssetBundleManager()    {        dictAssetBundleRefs = new Dictionary<string, AssetBundleRef>();    }    static private AssetBundleManifest ManifestAsssetbundle = null;    /// <summary>    /// 包含AssetBundle引用的类，其中包括url地址和version版本号    /// </summary>    public class AssetBundleRef    {        public AssetBundle assetBundle = null;  //资源包        public int version; //版本号        public string url;  //下载地址        public string abname; //ab名称        public AssetBundleRef(string strUrlIn, string abnameIn,int intVersionIn)        {            url = strUrlIn;            abname = abnameIn;            version = intVersionIn;        }    };    public static bool AddAssetBundle(string abname, AssetBundleRef abRef)    {        if (dictAssetBundleRefs.ContainsKey(abname))        {            Debug.Log("重复添加assetbundle,name:" + abname);            return false;        }        dictAssetBundleRefs.Add(abname, abRef);        return true;    }    private static bool LoadDependenciesAssetBundle(string srcABName)
    {
        if(ManifestAsssetbundle == null)
        {
            string path = GameDefine.AssetBundleSavePath + GameDefine.DependenciesAssetBundleName;

#if UNITY_EDITOR || UNITY_STANDALONE_WIN        if (!Luancher.UpdateWithLuncher)
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
    }    /// <summary>    /// 从保存的容器中获取资源包    /// </summary>    /// <param name="url">下载地址</param>    /// <param name="version">版本号</param>    /// <returns>返回获取的资源包</returns>    public static AssetBundle GetAssetBundle(string abname)    {        AssetBundleRef abRef;        if (dictAssetBundleRefs.TryGetValue(abname, out abRef))    //将空的abRef传递进去
            return abRef.assetBundle;        else
        {
            //尝试加载一下
            if (!LoadAssetBundleFromLocal(GameDefine.AssetBundleSavePath, abname))
                return null;
            else
                dictAssetBundleRefs.TryGetValue(abname, out abRef);
        }        return abRef.assetBundle;    }    /// <summary>    /// 下载assetbundle    /// </summary>    /// <param name="url">下载地址</param>    /// <param name="abname">assetbundle名称</param>    /// <param name="version">版本号</param>    /// <returns></returns>    public static IEnumerator downloadAssetBundle(string url,string abname, int version)    {        string path = url+ abname;        if (dictAssetBundleRefs.ContainsKey(abname))           yield return null;        else        {            while (!Caching.ready)                yield return null;            using (WWW www = WWW.LoadFromCacheOrDownload(path, version)) //异步方式加载            {                yield return www;                if (www.error != null)                    throw new Exception("WWW download:" + www.error);                AssetBundleRef abRef = new AssetBundleRef(url,abname, version);                abRef.assetBundle = www.assetBundle;                AddAssetBundle(abname, abRef);            }        }    }    /// <summary>    /// 同步加载assetbundle    /// </summary>    /// <param name="path">本地加载路径</param>    /// <param name="abname">文件名</param>    /// <returns></returns>    public static bool LoadAssetBundleFromLocal(string path, string abname)    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN        if (!Luancher.UpdateWithLuncher)
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

        if (dictAssetBundleRefs.ContainsKey(abname))        {            //Debug.Log("重复加载assetbundle,name:" + abname);            return true;        }        if (!File.Exists(path+abname))        {            Debug.Log("加载的Assetbundle文件不存在" + abname);            return false;        }        //加载依赖资源        LoadDependenciesAssetBundle(abname);        //Debug.Log("加载bundle名称:" + abname+" 开始:" + DateTime.Now.Ticks);        AssetBundle bundle = AssetBundle.LoadFromFile(path + abname);        if (bundle == null)           return false;        //AssetBundleCreateRequest abcr = AssetBundle.LoadFromFileAsync(path);
        //return abcr;
        //callback(abcr.assetBundle);

        //Debug.Log("加载bundle名称:" + abname + " 结束:" + DateTime.Now.Ticks);

        AssetBundleRef abRef = new AssetBundleRef(path, abname, 1);        abRef.assetBundle = bundle;        AddAssetBundle(abname, abRef);        return true;    }    /// <summary>    /// 删除容器中保存的资源数据    /// </summary>    /// <param name="name">名称</param>    /// <param name="bUnloadAllObj">表示是否删除资源包的所有资源</param>    public static void UnloadAssetBundle(string name,bool bUnloadAllObj = true)    {        AssetBundleRef abRef;        if (dictAssetBundleRefs.TryGetValue(name, out abRef))        {            abRef.assetBundle.Unload(bUnloadAllObj);            abRef.assetBundle = null;            dictAssetBundleRefs.Remove(name);        }    }}