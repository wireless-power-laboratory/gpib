using System.IO.Ports;

namespace PrologixUsb
{
    public class GpibControl
    {
        SerialPort sp = new SerialPort();
        /// <summary>
        /// Creates the list of available ports.
        /// </summary>
        /// <returns></returns>
        public List<string> PortList()
        {
            List<string> tList = new List<string>();
            foreach (string s in System.IO.Ports.SerialPort.GetPortNames())
            {
                tList.Add(s);
            }

            tList.Sort();
            return tList;
        }
        /// <summary>
        /// Starts the connection to the GPIB interface.
        /// </summary>
        /// <param name="port"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public bool Start(string port, int timeout)
        {
             // in order to start the procedure, and connect to the GPIB interface. 
             // Set port no. as string (com n) and timeout (ms).
            
            // COM port parameters
            sp.PortName = port;
            sp.BaudRate = 115200;
            sp.DataBits = 8;
            sp.Parity = Parity.None;
            sp.StopBits = StopBits.One;
            // RTS/CTS handshaking
            sp.Handshake = Handshake.RequestToSend;
            sp.DtrEnable = true;
            if (timeout == 0)
            {
                timeout = 1000;
            }
            try
            {
                sp.Open();
                sp.Write("++mode 1" + "\r\n");  // sets mode to controller
                sp.Write("++read_tmo_ms" + timeout + "\r\n"); // sets timeout 
                //sp.Write("++auto 0" + "\r\n"); 
                return true;
            }
            catch (Exception e)
            {
                System.Console.Write(e.Message);
                return false;
            }
        }
        /// <summary>
        /// Reads the address at the GPIB interface.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
         public bool Address(int address)
         {
             if (sp.IsOpen == true)
             {
                 try
                 {
                     sp.Write("++addr " + address + "\r\n");

                     return true;
                 }
                 catch (Exception e)
                 {
                     return false;
                 }
             }
             else
             {
                 return false;
             }
         }
        /// <summary>
        /// Reads the connection to the GPIB interface.
        /// </summary>
        /// <returns></returns>
        public string Read()
        {
            if (sp.IsOpen == true)
            {
               string yread = "";
                try
                {
                    sp.DiscardInBuffer();
                    sp.Write("++read eoi" + "\r\n");
                    
                    DateTime lastRead = DateTime.Now;
                    TimeSpan elapsedTime = new TimeSpan();

                    //  timespan
                    TimeSpan TIMEOUT = new TimeSpan(0, 0, 10);

                    // Read from port until TIMEOUT time has elapsed since
                    // last successful read
                    while (TIMEOUT.CompareTo(elapsedTime) > 0)
                    {
                        string buffer = sp.ReadExisting();
                        //buffer = sp.ReadExisting();

                        if (buffer.Length > 0)
                        {
                            yread = buffer;
                           // Console.Write(buffer);
                            lastRead = DateTime.Now;
                        }
                        elapsedTime = DateTime.Now - lastRead;
                    }

                    return yread;
                    
                }
                catch (Exception e)
                {
                    return e.ToString();
                }
            }
            else
            {
                return "port not open";
            }
            
        }
        /// <summary>
        /// Reads data from the device at the specified address.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Write(int address, string y)
        { // writes data
            if (sp.IsOpen == true)
            {
                try
                {
                sp.Write("++addr " + address + "\r\n");
                sp.DiscardInBuffer();
               
                sp.Write(y + "\r\n");
                return true;
                     }
            catch (Exception e)
            {
                return false;
            }
            }
            else
            {
                return false;
            }

        }
        /// <summary>
        /// Writes data to the device at the specified address, then reads the response.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public string WriteRead(int address, string y)
        { // writes data, then reads
            string x="";

            if (sp.IsOpen == true)
            {
                try
                {
                    sp.Write("++addr " + address + "\r\n");
                    sp.DiscardInBuffer();
                    sp.Write(y + "\r\n");
                   
                   sp.DiscardInBuffer();
                   sp.Write("++read eoi" + "\r\n");
                   Thread.Sleep(500);
                   DateTime lastRead = DateTime.Now;
                   TimeSpan elapsedTime = new TimeSpan();

                   //  timespan
                   TimeSpan TIMEOUT = new TimeSpan(0, 0, 10);

                   // Read from port until TIMEOUT time has elapsed since
                   // last successful read
                   while (TIMEOUT.CompareTo(elapsedTime) > 0)
                   {
                       string buffer = sp.ReadExisting();
                       //buffer = sp.ReadExisting();

                       if (buffer.Length > 0)
                       {
                           x = buffer;
                           // Console.Write(buffer);
                           lastRead = DateTime.Now;
                       }
                       elapsedTime = DateTime.Now - lastRead;
                   }
                    return x;
                }
                catch (Exception e)
                {
                    return e.ToString();

                }
            }
            else
            {
                return "port not open";
            }  
        }
        /// <summary>
        /// Clears the device at the specified address.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public bool Clear(int address)
        {
            if(sp.IsOpen == true)
            {
             
            try
            {
                sp.Write("++addr " + address + "\r\n");
                sp.Write("++clr");
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
            }
            else{
            return false;
                 }
        }
        /// <summary>
        /// Sets the EOI mode of the device at the specified address.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public bool Eoi(int x)
        {
            if (sp.IsOpen == true)
            {
                try
            {
                    sp.DiscardInBuffer();
                    sp.Write("++eoi " + x + "\r\n");
                    return true;
            }
            catch (Exception e)
            {
                return false;
            }
            }
            else
            {
                return false;
            }
            
        }
        /// <summary>
        /// Sets the GPIB address of the device at the specified address.
        /// </summary>
        /// <returns></returns>
        public bool Ifc()
        {
           if (sp.IsOpen == true)
            {
                try
            {
                    sp.Write("++ifc" + "\r\n");
                    return true;
            }
            catch (Exception e)
            {
                return false;

            }
           }
               else 
               {
               return false;
               }
            
        }
        /// <summary>
        /// Sets the address of the device at the specified address.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public bool Loc(int address)
        {
            if (sp.IsOpen == true)
            {
                try
            {
                    sp.Write("++addr " + address + "\r\n");
                    sp.Write("++loc" + "\r\n");
                    return true;
            }
            catch (Exception e)
            {
                return false;
            }
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Sets the mode of the device at the specified address.
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Mode(int y)
        {
            if (sp.IsOpen == true)
            {
                try
            {
                    sp.Write("++mode " + y + "\r\n");
                    return true;
            }
            catch (Exception e)
            {
                return false;
            }
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Resets the device at the specified address.
        /// </summary>
        /// <returns></returns>
        public bool Rst()
        {
            if (sp.IsOpen == true)
            {
                try
            {
                    sp.Write("++rst" + "\r\n");
                    return true;
            }
            catch (Exception e)
            {
                return false;
            }
            }
            else 
            {
                return false;
            }
        }
        /// <summary>
        /// Polls the device at the specified address.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public string Spoll(int x)
        {
            if (sp.IsOpen == true)
            {
                try
            {
                    sp.DiscardInBuffer();
                    sp.Write("++spoll "+ x + "\r\n");
                    string y = sp.ReadExisting();
                    return y;

            }
            catch (Exception e)
            {
                return e.ToString();
            }
                
            }
            else
            {
                return "port not open";
            }
        }
        /// <summary>
        /// Polls all devices on the bus.
        /// </summary>
        /// <returns></returns>
        public string[] BusPoll()
        {
            if (sp.IsOpen == true)
            {
               var a  = new string [31];
                for (int i = 1; i < 30; i++)
                {
                    try
                    {
                       sp.Write("++spoll " + i + "\r\n");
                       a[i] = sp.ReadLine();
                       
                    }
                    catch (Exception e)
                    {
                        
                    }
                }
                return a;
            }
                else
                {
                var a = new string[1];
                    a[0] = "0";
                    return a;
                }
            
        }
        /// <summary>
        /// Reads the status byte of the device at the specified address.
        /// </summary>
        /// <returns></returns>
        public string Srq()
        {
            if (sp.IsOpen == true)
            {
                try
                {
                    sp.Write("++srq");
                    string y = sp.ReadLine();
                    return y;
                }
                catch (Exception e)
                {
                    return e.ToString();
                }
            }
            else
            {
                return "error";
            }
        }
        

    }
}
