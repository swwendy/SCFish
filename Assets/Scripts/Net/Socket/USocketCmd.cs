namespace USocket.Cmd   //Updata By zw
{
	public class CommandID
	{
		/** 登陆消息命令 **/
		public static int LOGIN = 1001;
		
	}
	
	/// <summary>
	///消息命令常量
	/// </summary>
	/// 与服务端FBNetwork_Msg.h中消息号一致
    //public enum EMSG_TYPE01 : uint
    //{

        //EMSG_U3D_BASE,
        ////MAX_TYPE = MAX_SUB * 0xff,
        ////========================DB数据消息起始===============================
        //EMSG_U3D_D2L_DBSERVER_LOGIN = 1,         //DBServer连接登陆服务器

        ////角色登陆相关消息
        //EMSG_U3D_C2L_ROLE_LOGIN = 100,            //玩家登陆游戏的消息
        //EMSG_U3D_C2L_ROLE_CREATE,                   //玩家创建角色
        //EMSG_U3D_L2C_ROLE_DATA,                      //登陆成功后角色数据


        //EMSG_U3D_C2S_ROLE_CHAT,                     //玩家发起聊天消息
        //EMSG_U3D_Msg_Proxy,                              //消息转发

        //EMSG_U3D_C2S_MODE_ENTER,                  //玩家进入合作模式
        //EMSG_U3D_S2C_MODE_START,                  //合作模式开始游戏

        //EMSG_U3D_C2S_ROLE_PLAYSKILL,             //玩家施放技能
        //EMSG_U3D_C2S_ROLE_MOVE,                    //玩家移动
        //EMSG_U3D_C2S_ROLE_SHOOT,
        //EMSG_U3D_C2S_ROLE_DEAD,
        //EMSG_U3D_C2S_ROLE_RELIVE,
        //EMSG_U3D_C2S_MODE_END,
        //EMSG_U3D_C2S_ROLE_OFFLINE,

        //EMSG_U3D_C2S_ROOM_ENTER = 500,				//加入房间(进入匹配)
        //EMSG_U3D_S2C_ROOM_START,					//匹配完成，进入游戏
        //EMSG_U3D_C2S_GAME_START,					//所有玩家已进入游戏场景，开始游戏

	//}
	
	/// <summary>
	/// 错误定义
	/// </summary>
	//与服务端FBNetwork_Msg.h中ERRORDEFINE定义一致
	public enum ErrorDefine : uint
	{
		/// <summary>
		///无错误，命令正确接收
		/// </summary>
		ERRORDEFINE_NONE = 0,
		LOGIN_VERSION_ERROR,                    //客户端版本号不正确 
		LOGIN_ACCOUNT_ERROR,                    //账号错误
		LOGIN_PASSWORD_ERROR,                   //密码错误
		LOGIN_ACCOUNT_BANED,                    //账号被封
		
		LOGIN_ACCOUNT_NOROLE,                   //账号未创建角色
		CREATEROLE_NAME_REPEAT,                 //创建角色名字已经存在

	}
}
