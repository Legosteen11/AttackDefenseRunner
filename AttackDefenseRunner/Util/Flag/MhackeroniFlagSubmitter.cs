using System;
using System.Net.Sockets;
using Serilog;

namespace AttackDefenseRunner.Util.Flag
{
    public class MhackeroniFlagSubmitter : IFlagSubmitter
    {
        public void Submit(string flag)
        {
            if (flag == "") 
                return;

            var port = 31337;
            var server = "127.0.0.1";
            TcpClient client = new TcpClient(server, port);
            //Translate the flag into ASCII and store it as a Byte array
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(flag);
            
            //Get stream for writing
            NetworkStream stream = client.GetStream();
            stream.Write(data,0,data.Length);
            
            Log.Information("Submitted flag: {flag}", flag);
        }
    }
}


// private static Int32 port = 31337;
// private TcpClient client = new TcpClient(server, port);
//                 
// //Translate the flag into ASCII and store it as a Byte array
// Byte[] data = System.Text.Encoding.ASCII.GetBytes(flag);
// private NetworkStream stream = TcpClient.GetStream();