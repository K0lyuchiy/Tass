using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Windows;
using Microsoft.Win32;

namespace tass
{
    class COM_Interface
    {
        public String status_text;
        /// <summary>
        /// Recursively enumerates registry subkeys starting with startKeyPath looking for 
        /// "Device Parameters" subkey. If key is present, friendly port name is extracted.
        /// </summary>
        /// <param name="startKeyPath">the start key from which to begin the enumeration</param>
        /// <param name="targetMap">dictionary that will get populated with 
        /// nonfriendly-to-friendly port names</param>
        /// <param name="portsToMap">array of port names (i.e. COM1, COM2, etc)</param>
        public  void MineRegistryForPortName(string startKeyPath, Dictionary<string, string> targetMap,
            string[] portsToMap)
        {
            if (targetMap.Count >= portsToMap.Length)
                return;
            using (RegistryKey currentKey = Registry.LocalMachine)
            {
                try
                {
                    using (RegistryKey currentSubKey = currentKey.OpenSubKey(startKeyPath))
                    {
                        string[] currentSubkeys = currentSubKey.GetSubKeyNames();
                        if (currentSubkeys.Contains("Device Parameters") &&
                            startKeyPath != "SYSTEM\\CurrentControlSet\\Enum")
                        {
                            object portName = Registry.GetValue("HKEY_LOCAL_MACHINE\\" +
                                startKeyPath + "\\Device Parameters", "PortName", null);
                            if (portName == null ||
                                portsToMap.Contains(portName.ToString()) == false)
                                return;
                            object friendlyPortName = Registry.GetValue("HKEY_LOCAL_MACHINE\\" +
                                startKeyPath, "FriendlyName", null);
                            string friendlyName = "N/A";
                            if (friendlyPortName != null)
                                friendlyName = friendlyPortName.ToString();
                            if (friendlyName.Contains(portName.ToString()) == false)
                                friendlyName = string.Format("{0} ({1})", friendlyName, portName);
                            targetMap[portName.ToString()] = friendlyName;
                        }
                        else
                        {
                            foreach (string strSubKey in currentSubkeys)
                                MineRegistryForPortName(startKeyPath + "\\" + strSubKey, targetMap, portsToMap);
                        }
                    }
                }
                catch (Exception)
                { 
                    //MessageBox.Show(startKeyPath); 
                }
            }
        }

        /// <summary>
        /// Begins recursive registry enumeration
        /// </summary>
        /// <param name="portsToMap">array of port names (i.e. COM1, COM2, etc)</param>
        /// <returns>a hashtable mapping Friendly names to non-friendly port values</returns>
        public Dictionary<string, string> BuildPortNameHash(string[] portsToMap)
        {
            Dictionary<string, string> oReturnTable = new Dictionary<string, string>();
            MineRegistryForPortName("SYSTEM\\CurrentControlSet\\Enum", oReturnTable, portsToMap);
            return oReturnTable;
        }

        public SerialPort getSerialPort(string com_str)
        {
            SerialPort _serialPort = new SerialPort();

            if (com_str.Length != 0)   
            {
                _serialPort.PortName = com_str;
                this.status_text = "Измерительная система подключена";
            }
            else
            {
                string[] str = SerialPort.GetPortNames();

                Dictionary<string, string> friendlyPorts = BuildPortNameHash(SerialPort.GetPortNames());

                foreach (KeyValuePair<string, string> kvp in friendlyPorts)
                {
                    if (kvp.Value.Contains("Serial") || kvp.Value.Contains("TASS") || kvp.Value.Contains("Reciever") || kvp.Value.Contains("USB"))
                    {
                        _serialPort.PortName = kvp.Key;
                        this.status_text  = "Измерительная система подключена";
                        break;
                    }               
                    else
                    {
                        _serialPort.PortName = "null";
                        this.status_text = "Измерительная система не подключена";
                    }
                }
            }
            _serialPort.BaudRate = 19200;
            _serialPort.Parity = Parity.None;
            _serialPort.DataBits = 8;
            _serialPort.StopBits = StopBits.One;
            _serialPort.Handshake = Handshake.None;
            _serialPort.ReadTimeout = 0;
            _serialPort.WriteTimeout = 1;

            return _serialPort;
        }
    }
}
