using ET;
using System;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
// 客户端逻辑：这个项目整个的是客户端逻辑。不要服务器是不可能的，一定是要服务器，这个客户端才能够也服务器通信。它是可以与ET 框架的服务器端直接通信的
public class Client : MonoBehaviour {
    private const string TAG = "Client";
    public string address = "127.0.0.1";
    public const int port = 10002; // 这个端口：是用来作什么用的 ?
    TextMeshProUGUI text;  // 给个提示信息：你没有帐房，请先注册；注册成功；登录失败等
    public Button button; // 这两个按钮，但是界面上实际上只有一个。 login Button
    // public Button signUpbutton; // 注册: 这个可以暂不要就可以了呀。界面上没有这个按钮，所以把它去掉了
    NetKcpComponent NetKcpComponent;
    Session session;
    public TMP_InputField username;
    public TMP_InputField password;
    public TextMeshProUGUI ping;

    private void Awake() {
        username.text = name;
        text = button.GetComponentInChildren<TextMeshProUGUI>();
        NetKcpComponent = GetComponent<NetKcpComponent>(); // 它身上持了这个组件，是的，所以初始化也是从这里初始化的
    }
    private void Start() {
        button.onClick.AddListener(OnButtonClick);
        // signUpbutton.onClick.AddListener(OnSignUpButtonClick);
        text.text = "Connect";
    }
    bool isConnected => session != null && !session.IsDisposed; // 会话框建立起来了，并且还没有回收
    private async void OnButtonClick() { // 异步方法 
        if (isConnected) { // 如果是连接着的状态：就断开连接 
            text.text = "Connect";
            ping.text = "Ping: - ";
            session.Send(new C2M_Stop()); // 向服务器发消息: 停止消息 Client-to-Map 
            session.Dispose(); // 感觉这里就有点儿怪：不奇怪，连接好的状态，再点，就停止 
            session = null;
        } else { // 如果是断开状态，就登录远程服务器
            var host = $"{address}:{port}"; // 远程服务器的地址：Realm 注册登录服的 IP 地址 
            var result = await LoginAsync(host); // 登录远程服务器：自己的逻辑，这里是 Realm 注册登录服，先注册才登录
            text.text = result ? "Connected" : "Try again"; // Try.again: 这里就出错了
        }
    }
    // // 注册成功后：按钮失活。去ET 框架里找下，注册与登录所用的 Session 需要回收吗？注册登录过程应该很短，可以不缓存
    // // 现项目没有服务器端的注册登录逻辑。Realm 注册登录用，需要缓存保存用户登录，与个人帐户信息。【现在的基本要求，以后写熟悉了，可以加保持用户游戏数据】
    // // 需要添加 MongoDB 数据库相关模块，但仍用 allServer 模式 
    // private async void OnSignUpButtonClick() {
    //     signUpbutton.gameObject.SetActive(false); // 按钮失活
    //     // 创建一个ETModel层的Session
    //     R2C_Login r2CLogin;
    //     Session forRealm = NetKcpComponent.Create(NetworkHelper.ToIPEndPoint(address));
    //     r2CLogin = (R2C_Login)await forRealm.Call(new C2R_Login() { Account=username.text, Password=password.text });
    //     // forRealm?.Dispose(); // 延后处理：怎么处理到某个自动逻辑里去？
    //     Debug.Log($"{nameof(Client)}: ");
    // }
    public async ETTask<bool> LoginAsync(string address) {
        bool isconnected = true;
        try {
            // 创建一个ETModel层的Session
            Session forgate = NetKcpComponent.Create(NetworkHelper.ToIPEndPoint(address));
            R2C_Login r2CLogin = (R2C_Login)await forgate.Call(new C2R_Login() { Account=username.text, Password=password.text });
            forgate?.Dispose();

            Debug.Log(TAG + " r2CLogin.Address: " + r2CLogin.Address);
            // 创建一个gate Session,并且保存到SessionComponent中
            session = NetKcpComponent.Create(NetworkHelper.ToIPEndPoint(r2CLogin.Address)); // 这里拿到的地址，应该是网关服的地址
            session.ping = new ET.Ping(session); // 这是它的一个基本通信测试：测试这个消息与服务器的通信。可是现在，这个过程，TChannel 抛异常了
            session.ping.OnPingRecalculated += (delay) => { ping.text = $"Ping: {delay}"; };
            // 这里可能有两处从服务器返回消息的：一个是Ping, 一个是登录返回，两处要知道是哪处出错了？打几个日志，区分一下Ping 与登录消息，看是哪个出错的？
            G2C_LoginGate g2CLoginGate = (G2C_LoginGate)await session.Call(new C2G_LoginGate() { Key = r2CLogin.Key, GateId = r2CLogin.GateId });
            Debug.Log("登陆gate成功!"); // 这里没有问题。登录成功。
            // 登录 map 服务器
            // 进入地图
            var request = new C2G_EnterMap() ; // 这里可以改成自己的：进入某个特定的游戏 
            G2C_EnterMap map = await session.Call(request) as G2C_EnterMap;// 这里去看服务器端的逻辑
            // 进入地图成功：  Net_id = 1732270104318435333 爱表哥，爱生活！！！
            Debug.Log($"进入地图成功：  Net_id = {map.MyId}"); // 这里是客户端逻辑，客户端说消息发完就成功了，但是服务器可能还并没有处理这个消息？【总之，服务器端还有个小错】
        }
        catch (Exception e) {
            isconnected = false;
            Debug.LogError($"登陆失败 - {e}");
        }
        return isconnected;
    }
}