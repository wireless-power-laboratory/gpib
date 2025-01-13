using System;
using System.Collections.Generic;

namespace PrologixUsb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Gpib bus = new Gpib();
            List<string> tList = bus.PortList();
        }
    }
}