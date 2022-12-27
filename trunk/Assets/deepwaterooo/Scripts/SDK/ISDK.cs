using System;

namespace DWater {
    public interface ISDK {

        void Init();
        void sendMsg(String s);
		void ManagePlayers ();

        int add(int v1, int v2);
		void Logout();
		void ShowLogin();
	}
    
}
