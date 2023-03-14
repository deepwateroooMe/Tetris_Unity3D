using System;
namespace ET {
    public static class TimeHelper {

        public const long OneDay = 86400000;
        public const long Hour = 3600000;
        public const long Minute = 60000;
        // 客户端时间: 是从客户端来的吗？应该是从客户端来的，否则不 make-sense
        public static long ClientNow() => TimeInfo.Instance.ClientNow;
        public static long ClientNowSeconds() => ClientNow() / 1000;
        public static DateTime DateTimeNow() => DateTime.Now;
        public static long ServerNow() => TimeInfo.Instance.ServerNow();
        public static long ClientFrameTime() => TimeInfo.Instance.ClientFrameTime();
        public static long ServerFrameTime() => TimeInfo.Instance.ServerFrameTime();
    }
}
