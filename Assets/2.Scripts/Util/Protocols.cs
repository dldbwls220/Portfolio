using System.Runtime.InteropServices;
using System;
using UnityEngine;

namespace Protocols
{
    #region [유틸 클래스]
    public class PacketConverter
    {
        public static Packet CreatePacket(ulong uuid, uint protocol, int size, byte[] data)
        {
            Packet packet;
            packet._uuid = uuid;
            Debug.Log(packet._uuid);
            packet._protocol = protocol;
            packet._dataSize = size;
            packet._data = new byte[1008];
            if (data != null)
                Array.Copy(data, packet._data, data.Length);

            return packet;
        }
        public static byte[] StructureToByteArray(object pack)
        {
            int dataSize = Marshal.SizeOf(pack);
            IntPtr ptr = Marshal.AllocHGlobal(dataSize);
            Marshal.StructureToPtr(pack, ptr, false);
            byte[] data = new byte[dataSize];
            Marshal.Copy(ptr, data, 0, dataSize);
            Marshal.FreeHGlobal(ptr);

            return data;
        }
        public static T ByteArrayToStructure<T>(byte[] data, int size) where T : struct
        {
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(data, 0, ptr, size);
            T obj = Marshal.PtrToStructure<T>(ptr);
            Marshal.FreeHGlobal(ptr);

            return obj;
        }
    }
    #endregion [유틸 클래스}
    #region [패킷 구조체]
    [StructLayout(LayoutKind.Sequential)]
    public struct Packet
    {
        [MarshalAs(UnmanagedType.U8)]
        public ulong _uuid;
        [MarshalAs(UnmanagedType.U4)]
        public uint _protocol;
        [MarshalAs(UnmanagedType.I4)]
        public int _dataSize;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1008)]
        public byte[] _data;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct Packet_Join
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string _id;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string _password;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string _userName;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct Packet_UserData
    {
        [MarshalAs(UnmanagedType.U8)]
        public ulong _uuid;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string _id;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string _password;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string _userName;
        [MarshalAs(UnmanagedType.U4)]
        public uint _diamond;
        [MarshalAs(UnmanagedType.U4)]
        public uint _skeleton;
        [MarshalAs(UnmanagedType.U4)]
        public uint _slime;
        [MarshalAs(UnmanagedType.U4)]
        public uint _bat;
        [MarshalAs(UnmanagedType.U4)]
        public uint _golem;
        [MarshalAs(UnmanagedType.U4)]
        public uint _direBat;
        [MarshalAs(UnmanagedType.U4)]
        public uint _redDragon;
        [MarshalAs(UnmanagedType.U4)]
        public uint _banshee;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string _itemID;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Packet_Check
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string _id;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct Packet_StdFailed
    {
        [MarshalAs(UnmanagedType.U4)]
        public uint _errorCode;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct Packet_Login
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string _id;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string _password;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct Packet_Update
    {
        [MarshalAs(UnmanagedType.U4)]
        public uint _diamond;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Packet_UpdateMonster
    {
        [MarshalAs(UnmanagedType.U4)]
        public uint _skeleton;
        [MarshalAs(UnmanagedType.U4)]
        public uint _slime;
        [MarshalAs(UnmanagedType.U4)]
        public uint _bat;
        [MarshalAs(UnmanagedType.U4)]
        public uint _golem;
        [MarshalAs(UnmanagedType.U4)]
        public uint _direBat;
        [MarshalAs(UnmanagedType.U4)]
        public uint _redDragon;
        [MarshalAs(UnmanagedType.U4)]
        public uint _banshee;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Packet_UpdateItem
    {
        [MarshalAs(UnmanagedType.U8)]
        public ulong _uuid;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string _itemID;
    }

    #endregion [패킷 구조체]
    #region [SV프로토콜]
    public class SVProtocol
    {
        public enum Send
        {
            Join,
            Login,
            Check,
            Update,
            UpdateMonster,
            UpdateItem,

            Client_Connect_Success = 200,

            Client_Join_Success,
            Client_Join_Failed,

            Client_Login_Success,
            Client_Login_Failed,

            Client_Check_Success,
            Client_Check_Failed,

            Client_Update_Success,
        }
        public enum Receive
        {
            DBConnect_Success,

            Join_Success,
            Join_Failed,

            Login_Success,
            Login_Failed,

            Check_Success,
            Check_Failed,

            Update_Success,

            Client_Join = 200,
            Client_Login,
            Client_Check,
            Client_Update,
            Client_UpdateItem
        }
    }
    #endregion [SV프로토콜]
    #region [DB프로토콜]
    public class DBProtocol
    {
        public enum Send
        {
            DBConnect_Success,

            Join_Success,
            Join_Failed,

            Login_Success,
            Login_Failed,

            Check_Success,
            Check_Failed,

            Update_Success,

            GetLastUUID_Success
        }
        public enum Receive
        {
            Join,
            Login,
            Check,
            Update,
            UpdateMonster,
            UpdateItem,
            GetLastUUID,
        }

    }
    #endregion [DB프로토콜]
    #region [CL프로토콜]
    public class CLProtocol
    {
        public enum Send
        {
            Client_Join = 200,
            Client_Login,
            Client_Check,
            Client_Update,
            Client_UpdateMosnter,
            Client_UpdateItem,
        }
        public enum Receive
        {
            Client_Connect_Success = 200,

            Client_Join_Success,
            Client_Join_Failed,

            Client_Login_Success,
            Client_Login_Failed,

            Client_Check_Success,
            Client_Check_Failed,

            Client_Update_Success,
        }
    }
    #endregion [CL프로토콜]
}
