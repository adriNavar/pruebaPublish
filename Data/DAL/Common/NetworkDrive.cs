using System;
using System.ComponentModel;
using System.Net;
using System.Runtime.InteropServices;

public class NetworkDrive : IDisposable
{
    private readonly string _networkName;

    public NetworkDrive(string networkName, string user, string passwd, string domain)
        : this(networkName, new NetworkCredential(user, passwd, domain)) { }
    public NetworkDrive(string networkName, string drive, string user, string passwd, string domain)
    : this(networkName, drive, new NetworkCredential(user, passwd, domain)) { }
    public NetworkDrive(string networkName, NetworkCredential credentials)
        : this(networkName, null, credentials) { }
    public NetworkDrive(string networkName, string drive, NetworkCredential credentials)
    {
        var netResource = new NetResource
        {
            Scope = ResourceScope.GlobalNetwork,
            ResourceType = ResourceType.Any,
            DisplayType = ResourceDisplaytype.Share,
            RemoteName = networkName,
        };

        if (!string.IsNullOrEmpty(drive))
        {
            netResource.LocalName = $"{drive}:";
        }
        _networkName = netResource.LocalName ?? netResource.RemoteName;

        string userName = string.IsNullOrEmpty(credentials.Domain)
            ? credentials.UserName
            : $@"{credentials.Domain}\{credentials.UserName}";

        int ret = WNetAddConnection2(netResource, credentials.Password, userName, 0);

        if (ret != 0 && ret != 85/*no se cerro bien y no se desregistro*/)
        {
            throw new Win32Exception(ret, "Error connecting to remote share");
        }
    }

    ~NetworkDrive()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        int ret = WNetCancelConnection2(_networkName, 0, true);
    }

    [DllImport("mpr.dll")]
    private static extern int WNetAddConnection2(NetResource netResource,
        string password, string username, int flags);

    [DllImport("mpr.dll")]
    private static extern int WNetCancelConnection2(string name, int flags,
        bool force);

    [StructLayout(LayoutKind.Sequential)]
    public class NetResource
    {
        public ResourceScope Scope;
        public ResourceType ResourceType;
        public ResourceDisplaytype DisplayType;
        public int Usage;
        public string LocalName;
        public string RemoteName;
        public string Comment;
        public string Provider;
    }

    public enum ResourceScope : int
    {
        Connected = 1,
        GlobalNetwork,
        Remembered,
        Recent,
        Context
    };

    public enum ResourceType : int
    {
        Any = 0,
        Disk = 1,
        Print = 2,
        Reserved = 8,
    }

    public enum ResourceDisplaytype : int
    {
        Generic = 0x0,
        Domain = 0x01,
        Server = 0x02,
        Share = 0x03,
        File = 0x04,
        Group = 0x05,
        Network = 0x06,
        Root = 0x07,
        Shareadmin = 0x08,
        Directory = 0x09,
        Tree = 0x0a,
        Ndscontainer = 0x0b
    }
}