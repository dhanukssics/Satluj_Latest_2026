using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;


namespace Satluj_Latest.Data
{
    public class PushService : BaseReference
    {

        internal async Task<bool> push_BusStarted(long busId, int? shiftStatus, string tripNumber, string fromLocation)
        {
            int tripNo = Convert.ToInt32(tripNumber);
            var parents = await _Entities.PushBusStartResults
                             .FromSqlRaw(
                                 "EXEC SP_Push_BusStart @busId, @tripNo",
                                 new SqlParameter("@busId", busId),
                                 new SqlParameter("@tripNo", tripNo)
                             )
                             .ToListAsync();
            var message = "Your Kids bus started";
            foreach(var item in parents)
            {
                //pushandroid(item.Token, message, "Bus Started");
            }
            return true;
        }
        //private bool SendPushNotificationIos(string DeviceId, string message, string push)
        //{
        //    int port = 2195;
        //    //string hostname = "gateway.sandbox.push.apple.com";
        //    string hostname = "gateway.push.apple.com";
        //    string certificatePath = "";
        //    //  DeviceId = "b583e33f267ad297faf5486a53bf88acf59087488b872899569451778a0b198e";
        //    //load certificate
        //    //local path
        //    // certificatePath = "D:/Certificates(1).p12";
        //    //server path
        //    string serverPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Certificate/");
        //    certificatePath = serverPath + "Certificates.p12";
        //    string certificatePassword = "sics";
        //    X509Certificate2 clientCertificate = new X509Certificate2(certificatePath, certificatePassword);
        //    X509Certificate2Collection certificatesCollection = new X509Certificate2Collection(clientCertificate);
        //    TcpClient client = new TcpClient(hostname, port);
        //    SslStream sslStream = new SslStream(
        //            client.GetStream(),
        //            false,
        //            new RemoteCertificateValidationCallback(ValidateServerCertificate),
        //            null
        //    );
        //    try
        //    {
        //        sslStream.AuthenticateAsClient(hostname, certificatesCollection, SslProtocols.Tls12, true);
        //    }
        //    catch (AuthenticationException ex)
        //    {
        //        client.Close();
        //        //continue;
        //    }
        //    // Encode a test message into a byte array.
        //    MemoryStream memoryStream = new MemoryStream();
        //    BinaryWriter writer = new BinaryWriter(memoryStream);
        //    writer.Write((byte)0);  //The command
        //    writer.Write((byte)0);  //The first byte of the deviceId length (big-endian first byte)
        //    writer.Write((byte)32); //The deviceId length (big-endian second byte)
        //    writer.Write(HexToData(DeviceId.ToUpper()));
        //    byte[] byteArray = Encoding.ASCII.GetBytes(DeviceId.ToUpper());
        //    //  writer.Write(byteArray);
        //    //        String payload = "{\"aps\": {\"badge\": 1,\"alert\": \"" + message + "\"}}";
        //    //  string sampleBadge = "1";
        //    //String payload = " {\"aps\": {\"alert\" : {\"title\" :\"SchoolMan\",  \"body\" : \"" + message + "\",  \"jobid\" : \"" + jobid + "\",  \"pushType\" : \"" + push + "\"},\"sound\": \"push.wav\"}, \"vibrate\": true}";
        //    String payload = " {\"aps\": {\"alert\" : {\"title\" :\"SchoolMan\",  \"body\" : \"" + message + "\", \"pushType\" : \"" + push + "\"},\"sound\": \"push.wav\"}, \"vibrate\": true}";
        //    writer.Write((byte)0); //First byte of payload length; (big-endian first byte)
        //    writer.Write((byte)payload.Length); //payload length (big-endian second byte)
        //    byte[] b1 = System.Text.Encoding.UTF8.GetBytes(payload);
        //    writer.Write(b1);
        //    writer.Flush();
        //    byte[] array = memoryStream.ToArray();
        //    sslStream.Write(array);
        //    sslStream.Flush();
        //    // Close the client connection.
        //    client.Close();
        //    return true;
        //}
        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);
            // Do not allow this client to communicate with unauthenticated servers. 
            return false;
        }
        private static byte[] HexToData(string deviceId)
        {
            if (deviceId == null)
                return null;

            if (deviceId.Length % 2 == 1)
                deviceId = '0' + deviceId; // Up to you whether to pad the first or last byte

            byte[] data = new byte[deviceId.Length / 2];

            for (int i = 0; i < data.Length; i++)
                data[i] = Convert.ToByte(deviceId.Substring(i * 2, 2), 16);

            return data;
        }
    
    }
}
