using System;

namespace DWater {
    public interface ISDK {

        void Init();
        void sendMsg(String s);

        void registerVolumeReceiver();
        void unregisterVolumeReceiver();
        
// 这个方法不通: 是因为安卓SDK还从来不曾真正登录成功过,拿不到用户数据,没有监护人下的玩家信息,所以永远返回的是登录界面.换个别的测
		void ManagePlayers (); 

        int add(int v1, int v2);
		void Logout();
		void ShowLogin();
	}
}
