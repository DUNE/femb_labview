using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Timers;
using System.IO;

namespace LBNE_DLL
{
    public class LBNE_COMM
    {


        public UInt32 LBNE_DLL_VERSION = 102;


        private int REG_WR_Port = 32000;     //  Register write port    (send only)
        private int REG_RD_Port = 32001;     //  Register read port      (send only)  
        private int REG_RDBK_Port = 32002;   //  Register read back port (read only)
        private int HSDATA_Port = 32003;   // high speed data recive (read only)

        private System.Net.Sockets.UdpClient WRREG_UDP = null;
        private System.Net.IPEndPoint WRREG_EP = null;

        private System.Net.Sockets.UdpClient RDREG_UDP = null;
        private System.Net.IPEndPoint RDREG_EP = null;

        private System.Net.Sockets.UdpClient RDBKREG_UDP = null;
        private System.Net.IPEndPoint RDBKREG_EP = null;

        private System.Net.Sockets.UdpClient HSDATA_UDP = null;
        private System.Net.IPEndPoint HSDATA_EP = null;

        private static System.Timers.Timer aTimer;

                
        public byte[]   receive_byte_array;
        public UInt32[] receive_word_array;
        public UInt64[] List_Mode;
        public bool     HS_THREAD_running = true;
        public string               IP_ADDR = "192.168.121.1";

        public  uint index_G = 0;
        public bool LV_READY = false;
        public UInt32       number_of_samples = 1000;
        public UInt32       HIST_samples = 0;
        public	UInt32[]    ADC_samples;
	    public  UInt32[,] 	ADC_hist;
	    public  double[]	ADC_avg;
	    public  double[]	ADC_savg;
        public  UInt32      CHN_SEL = 0;
        public  UInt32              SYS_ERROR;
        private Thread              HSDATA_Thread = null;
        public bool                 Store_Data = false;
      //  private System.IO.StreamWriter        SFile = null;
        private System.IO.BinaryWriter          SFile = null;
        private System.IO.BinaryWriter          binWriter = null;


  

        public int open_file(String File_name)
        {


            if (SFile == null)
            {

                SFile = new BinaryWriter(File.Open(File_name, FileMode.Create));
             //   SFile = new StreamWriter(File_name, false, Encoding.ASCII);
                Store_Data = true;
                return (1);
            }
            else
            {
                Store_Data = false;
                SFile.Close();
                SFile = null;
                return (0);

            }

  
        }


        public int close_file()
        {
            Store_Data = false;
            if (SFile == null)
                return (0);
            else
            {
                SFile.Close();
                SFile = null;
                return (0);
            }
        }




        public  String      start_udp_IO()
        {
 
            IPAddress IP_ADDRESS = IPAddress.Parse(IP_ADDR);

            try
            {
                WRREG_UDP = new UdpClient(REG_WR_Port);
                WRREG_EP = new IPEndPoint(IP_ADDRESS, REG_WR_Port);

            }
            catch (Exception e)
            {
                return ("wr_port failed " + e.ToString());
            }

            try
            {
                RDREG_UDP = new UdpClient(REG_RD_Port);
                RDREG_EP  = new IPEndPoint(IP_ADDRESS, REG_RD_Port);
            }
            catch (Exception e)
            {
                return ("rd_port failed " + e.ToString());
            }
            try
            {
                RDBKREG_UDP = new UdpClient(REG_RDBK_Port);
                RDBKREG_UDP.Client.ReceiveTimeout = 1000;
                RDBKREG_EP = new IPEndPoint(IPAddress.Any, REG_RDBK_Port);
            }
            catch (Exception e)
            {
                return ("rdbk_port failed " + e.ToString());
            }
            try
            {
                HSDATA_UDP = new UdpClient(HSDATA_Port);
                HSDATA_UDP.Client.ReceiveBufferSize = 100 * 4096;
                HSDATA_EP = new IPEndPoint(IP_ADDRESS, HSDATA_Port);
            }
            catch (Exception e)
            {
                return ("rdbk_port failed " + e.ToString());
            }


            try
            {
                ADC_hist     = new UInt32[16,4096];
                ADC_samples  = new UInt32[16];
                ADC_avg      = new double[16];
                ADC_savg     = new double[16];



                List_Mode    = new UInt64[100];
                SYS_ERROR    = 0;
                HS_THREAD_running = true;
                HSDATA_Thread = new Thread(new ThreadStart(HS_Thread));
                HSDATA_Thread.Name = "HSDATA_Thread";
                HSDATA_Thread.Priority = ThreadPriority.Highest;
                HSDATA_Thread.Start();
            }
            catch (Exception e)
            {
                return ("UDP THREAD failed " + e.ToString());
            }
                return ("OK");
        }



        public string UDP_CLEANUP()
        {

               HS_THREAD_running = false;

            try
            {

                    WRREG_UDP.Close();
            }
            catch (Exception e)
            {
            //    return ("clean up failed" + e.ToString());
            }

            try
            {

                RDREG_UDP.Close();
            }
            catch (Exception e)
            {
           //     return ("clean up failed" + e.ToString());
            }

            try
            {
                    RDBKREG_UDP.Close();
            }
            catch (Exception e)
            {
            //    return ("clean up failed" + e.ToString());
            }


            try
            {
                HSDATA_UDP.Close();
            }
            catch (Exception e)
            {
             //   return ("clean up failed" + e.ToString());
            }


            try
            {
                HSDATA_Thread.Abort();
            }
            catch (Exception e)
            {
                return ("hs clean up failed" + e.ToString());
            }
            return ("ok");
        }


        private void HS_Thread()
        {

            uint chn,chp,PDO;

            uint    i=0; //,index=0;
            uint    data_in;
            UInt16  data_in_raw;
            uint    CHN = 0 ;
            UInt32  num_samp = number_of_samples;

            index_G = 0;
            LV_READY = false;
            num_samp = number_of_samples;
            receive_word_array = new UInt32[num_samp+1];



            while (HS_THREAD_running)
            {
                try
                {
                    receive_byte_array = HSDATA_UDP.Receive(ref HSDATA_EP);
                }
                catch (Exception e)
                {
                    SYS_ERROR++;
                }


                try
                {
                    if ((LV_READY == false) && (index_G > num_samp))
                    {

                        num_samp = number_of_samples;
                        receive_word_array = new UInt32[num_samp+1];
                        index_G = 0;
                    }

                }
                catch (Exception e)
                {
                    SYS_ERROR++;
                }

                try
                {

                    for (i = 16; i < (receive_byte_array.Length); i = i + 2)
                    {
                        if ((i + 2) >= (receive_byte_array.Length)) break;
                            data_in_raw = (UInt16) ((uint)receive_byte_array[i] << 8 | (uint)receive_byte_array[i + 1]);
                            data_in = (uint) data_in_raw & 0xfff;

                            CHN = ((uint)receive_byte_array[i] & 0xf0) >> 4;

                            if( (index_G == num_samp) && (CHN == CHN_SEL))
                            {

                                    receive_word_array[index_G] = data_in;
                                    index_G++;
                                    LV_READY = true;
                            }
                            else if ((index_G < num_samp) && (CHN == CHN_SEL))
                            {
                                receive_word_array[index_G] = data_in;
                                index_G++;
                                 LV_READY = false;
                            }


                            if ((HIST_samples > ADC_samples[CHN]) || (HIST_samples == 0))
                            {
                                ADC_samples[CHN]++;
                                ADC_hist[CHN,data_in]++;
                                ADC_avg[CHN] += data_in;
                                ADC_savg[CHN] += data_in * data_in;
                            }
                            List_Mode[3] =  data_in;

                          //  if ((SFile != null) && (Store_Data == true) && (CHN == CHN_SEL))
                            if ((SFile != null) && (Store_Data == true))
                            {
                                SFile.Write(data_in_raw);
                              //  SFile.WriteLine(CHN.ToString() + ' ' + data_in.ToString());
                            }

                     }
                 }
                catch (Exception e)
                {
                    List_Mode[5] = (UInt64)receive_byte_array.Length;
                    List_Mode[6] = i;
                    SYS_ERROR++;
                }
            }
        }


        public void RESET_STAT()
        {

    //          Array.Clear(PDO_avg,0,2405);

        }





        public string REG_WR(UInt32 DATA,UInt16 ADDRESS,bool IMP)
        {

            byte[] packet = { 0xDE,0xAD,0xBE,0xEF,0x00,0x00,0x00,0x00,0x00,0x00,0xff,0xff};

            packet[4] = (byte)((ADDRESS >> 8) & 0xff);
            packet[5] = (byte) (ADDRESS & 0xff);
            packet[6] = (byte) ((DATA >> 24) & 0xff);
            packet[7] = (byte) ((DATA >> 16) & 0xff);
            packet[8] = (byte) ((DATA >> 8) & 0xff);
            packet[9] = (byte)  (DATA & 0xff);

            try
            {
                    WRREG_UDP.Send(packet, 12, WRREG_EP);
            }
            catch (Exception e)
            {
                return ("clean up failed2" + e.ToString());
            }
            return ("OK");

          }




        public  string  REG_RD(out UInt32 DATA ,Int16 ADDRESS, bool IMP)
        {

            byte[] packet = { 0xDE, 0xAD, 0xBE, 0xEF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff };
            packet[4] = (byte)((ADDRESS >> 8) & 0xff);
            packet[5] = (byte)(ADDRESS & 0xff);
            packet[9] = 0x00;
            try
            {
                RDREG_UDP.Send(packet, 12,RDREG_EP);
                packet = RDBKREG_UDP.Receive(ref RDBKREG_EP);
                DATA = ((UInt32)packet[2] << 24) | ((UInt32)packet[3] << 16) | ((UInt32)packet[4] << 8) | (UInt32)packet[5];
                return("ok V3");
            }
            catch (Exception e)
            {
                DATA = 0;
                return ("read failed" + e.ToString());
            }


        }



        public string REG_BLK_RD(out UInt32[] DATA, Int16 ADDRESS, bool IMP)
        {

            byte[] packet = { 0xDE, 0xAD, 0xBE, 0xEF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff };
            UInt32[] DATA_IN = new UInt32[16];
            uint    index = 0;

            packet[4] = (byte)((ADDRESS >> 8) & 0xff);
            packet[5] = (byte)(ADDRESS & 0xff);
            packet[9] = 0x0f;
            try
            {

                RDREG_UDP.Send(packet, 12, RDREG_EP);
                packet = RDBKREG_UDP.Receive(ref RDBKREG_EP);

                for (index = 0; index <= 15; index++)
                {
                    DATA_IN[index] = ((UInt32)packet[index*6 + 2] << 24) | ((UInt32)packet[index*6 + 3] << 16) | ((UInt32)packet[index*6 + 4] << 8) | (UInt32)packet[index*6+ 5];
                }
                DATA = DATA_IN;
                return ("ok V3");
            }
            catch (Exception e)
            {
                DATA = DATA_IN;
                return ("read failed" + e.ToString());
            }


        }
    }
}
