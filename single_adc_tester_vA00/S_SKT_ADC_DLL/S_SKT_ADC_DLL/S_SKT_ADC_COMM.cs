using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Timers;
using System.IO;

namespace S_SKT_ADC_DLL
{
    public class S_SKT_ADC_COMM
    {


        public UInt32 S_SKT_DLL_VERSION = 0xA00;


        //WIB MAIN UDP

        private int REG_WR_Port   = 32000;     //  Register write port    (send only)
        private int REG_RD_Port   = 32001;     //  Register read port      (send only)  
        private int REG_RDBK_Port = 32002;   //  Register read back port (read only)
        private int HSDATA_Port   = 32003;   // high speed data recive (read only)

        private System.Net.Sockets.UdpClient WRREG_UDP = null;
        private System.Net.IPEndPoint WRREG_EP = null;

        private System.Net.Sockets.UdpClient RDREG_UDP = null;
        private System.Net.IPEndPoint RDREG_EP = null;

        private System.Net.Sockets.UdpClient RDBKREG_UDP = null;
        private System.Net.IPEndPoint RDBKREG_EP = null;


        //  FEMB 0
        private int REG_FEMB0_WR_Port   = 32016;   //  Register write port    (send only)
        private int REG_FEMB0_RD_Port   = 32017;   //  Register read port      (send only)  
        private int REG_FEMB0_RDBK_Port = 32018;   //  Register read back port (read only)


        private System.Net.Sockets.UdpClient WRREG_FEMB0 = null;
        private System.Net.IPEndPoint WRREG_FEMB0_EP = null;

        private System.Net.Sockets.UdpClient RDREG_FEMB0 = null;
        private System.Net.IPEndPoint RDREG_FEMB0_EP = null;

        private System.Net.Sockets.UdpClient RDBKREG_FEMB0 = null;
        private System.Net.IPEndPoint RDBKREG_FEMB0_EP = null;


        //  FEMB 1
        private int REG_FEMB1_WR_Port   = 32032;   //  Register write port    (send only)
        private int REG_FEMB1_RD_Port   = 32033;   //  Register read port      (send only)  
        private int REG_FEMB1_RDBK_Port = 32034;   //  Register read back port (read only)

        private System.Net.Sockets.UdpClient WRREG_FEMB1 = null;
        private System.Net.IPEndPoint WRREG_FEMB1_EP = null;

        private System.Net.Sockets.UdpClient RDREG_FEMB1 = null;
        private System.Net.IPEndPoint RDREG_FEMB1_EP = null;

        private System.Net.Sockets.UdpClient RDBKREG_FEMB1 = null;
        private System.Net.IPEndPoint RDBKREG_FEMB1_EP = null;


        //  FEMB 2
        private int REG_FEMB2_WR_Port   = 32048;   //  Register write port    (send only)
        private int REG_FEMB2_RD_Port   = 32049;   //  Register read port      (send only)  
        private int REG_FEMB2_RDBK_Port = 32050;   //  Register read back port (read only)

        private System.Net.Sockets.UdpClient WRREG_FEMB2 = null;
        private System.Net.IPEndPoint WRREG_FEMB2_EP = null;

        private System.Net.Sockets.UdpClient RDREG_FEMB2 = null;
        private System.Net.IPEndPoint RDREG_FEMB2_EP = null;

        private System.Net.Sockets.UdpClient RDBKREG_FEMB2 = null;
        private System.Net.IPEndPoint RDBKREG_FEMB2_EP = null;

        //  FEMB3
        private int REG_FEMB3_WR_Port   = 32064;   //  Register write port    (send only)
        private int REG_FEMB3_RD_Port   = 32065;   //  Register read port      (send only)  
        private int REG_FEMB3_RDBK_Port = 32066;   //  Register read back port (read only)

        private System.Net.Sockets.UdpClient WRREG_FEMB3 = null;
        private System.Net.IPEndPoint WRREG_FEMB3_EP = null;

        private System.Net.Sockets.UdpClient RDREG_FEMB3 = null;
        private System.Net.IPEndPoint RDREG_FEMB3_EP = null;

        private System.Net.Sockets.UdpClient RDBKREG_FEMB3 = null;
        private System.Net.IPEndPoint RDBKREG_FEMB3_EP = null;
 


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
        public  UInt16[]    CHN_DATA;
	    public  UInt32[,] 	ADC_hist;
	    public  double[]	ADC_avg;
	    public  double[]	ADC_savg;
        public  UInt32      CHN_SEL = 0;
        public  UInt32      SYS_ERROR;

        public  Int32       BUF_LEN;
        private Thread      HSDATA_Thread = null;
        public bool         Store_Data = false;
      //  private System.IO.StreamWriter        SFile = null;
        private System.IO.BinaryWriter          SFile = null;
        private System.IO.BinaryWriter          binWriter = null;



        public UInt32 DROP_PKT = 0;
        public int Ring_BUF_MAX = 100;
        public int Ring_BUF_HEAD = 0;
        public int Ring_BUF_TAIL = 0;
        public byte[][] RING_BUF;
        private Thread HSDATA_Thread2 = null;
 


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





        public String start_udp_IO(int THRD_SEL,int RING_BUF_SIZE)
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
                ADC_hist = new UInt32[16, 4096];
                ADC_samples = new UInt32[16];
                CHN_DATA = new UInt16[16];
                ADC_avg = new double[16];
                ADC_savg = new double[16];

                List_Mode = new UInt64[100];
                SYS_ERROR = 0;
                HS_THREAD_running = true;

                if (THRD_SEL == 0)
                    HSDATA_Thread = new Thread(new ThreadStart(HS_Thread));
                else
                    HSDATA_Thread = new Thread(new ThreadStart(HS_Thread_UDP_Receive));

                HSDATA_Thread.Name = "HSDATA_Thread";
                HSDATA_Thread.Priority = ThreadPriority.Normal;
                HSDATA_Thread.Start();
            }
            catch (Exception e)
            {
                return ("UDP THREAD failed " + e.ToString());
            }

            try
            {
                if (THRD_SEL == 1)
                {

                    RB_Create(RING_BUF_SIZE);
                    HSDATA_Thread2 = new Thread(new ThreadStart(HS_Thread_Process));
                    HSDATA_Thread2.Name = "HSDATA_Thread2";
                    HSDATA_Thread2.Priority = ThreadPriority.Normal;
                    HSDATA_Thread2.Start();
                }
                if (THRD_SEL == 2)
                {

                    RB_Create(RING_BUF_SIZE);
                    HSDATA_Thread2 = new Thread(new ThreadStart(HS_Thread_Process_2));
                    HSDATA_Thread2.Name = "HSDATA_Thread2";
                    HSDATA_Thread2.Priority = ThreadPriority.Normal;
                    HSDATA_Thread2.Start();
                }
            }
            catch (Exception e)
            {
                return ("THREAD 2 startup" + e.ToString());
            }


            // FEMB 0
            try
            {
                WRREG_FEMB0 = new UdpClient(REG_FEMB0_WR_Port);
                WRREG_FEMB0_EP = new IPEndPoint(IP_ADDRESS, REG_FEMB0_WR_Port);

            }
            catch (Exception e)
            {
                return ("wr_port failed " + e.ToString());
            }

            try
            {
                RDREG_FEMB0 = new UdpClient(REG_FEMB0_RD_Port);
                RDREG_FEMB0_EP = new IPEndPoint(IP_ADDRESS, REG_FEMB0_RD_Port);
            }
            catch (Exception e)
            {
                return ("rd_port failed " + e.ToString());
            }
            try
            {
                RDBKREG_FEMB0 = new UdpClient(REG_FEMB0_RDBK_Port);
                RDBKREG_FEMB0.Client.ReceiveTimeout = 1000;
                RDBKREG_FEMB0_EP = new IPEndPoint(IPAddress.Any, REG_FEMB0_RDBK_Port);
            }
            catch (Exception e)
            {
                return ("rdbk_port failed " + e.ToString());
            }



            // FEMB 1
            try
            {
                WRREG_FEMB1 = new UdpClient(REG_FEMB1_WR_Port);
                WRREG_FEMB1_EP = new IPEndPoint(IP_ADDRESS, REG_FEMB1_WR_Port);

            }
            catch (Exception e)
            {
                return ("wr_port failed " + e.ToString());
            }

            try
            {
                RDREG_FEMB1 = new UdpClient(REG_FEMB1_RD_Port);
                RDREG_FEMB1_EP = new IPEndPoint(IP_ADDRESS, REG_FEMB1_RD_Port);
            }
            catch (Exception e)
            {
                return ("rd_port failed " + e.ToString());
            }
            try
            {
                RDBKREG_FEMB1 = new UdpClient(REG_FEMB1_RDBK_Port);
                RDBKREG_FEMB1.Client.ReceiveTimeout = 1000;
                RDBKREG_FEMB1_EP = new IPEndPoint(IPAddress.Any, REG_FEMB1_RDBK_Port);
            }
            catch (Exception e)
            {
                return ("rdbk_port failed " + e.ToString());
            }


            // FEMB 2
            try
            {
                WRREG_FEMB2 = new UdpClient(REG_FEMB2_WR_Port);
                WRREG_FEMB2_EP = new IPEndPoint(IP_ADDRESS, REG_FEMB2_WR_Port);

            }
            catch (Exception e)
            {
                return ("wr_port failed " + e.ToString());
            }

            try
            {
                RDREG_FEMB2 = new UdpClient(REG_FEMB2_RD_Port);
                RDREG_FEMB2_EP = new IPEndPoint(IP_ADDRESS, REG_FEMB2_RD_Port);
            }
            catch (Exception e)
            {
                return ("rd_port failed " + e.ToString());
            }
            try
            {
                RDBKREG_FEMB2 = new UdpClient(REG_FEMB2_RDBK_Port);
                RDBKREG_FEMB2.Client.ReceiveTimeout = 1000;
                RDBKREG_FEMB2_EP = new IPEndPoint(IPAddress.Any, REG_FEMB2_RDBK_Port);
            }
            catch (Exception e)
            {
                return ("rdbk_port failed " + e.ToString());
            }


            // FEMB 3
            try
            {
                WRREG_FEMB3 = new UdpClient(REG_FEMB3_WR_Port);
                WRREG_FEMB3_EP = new IPEndPoint(IP_ADDRESS, REG_FEMB3_WR_Port);

            }
            catch (Exception e)
            {
                return ("wr_port failed " + e.ToString());
            }

            try
            {
                RDREG_FEMB3 = new UdpClient(REG_FEMB3_RD_Port);
                RDREG_FEMB3_EP = new IPEndPoint(IP_ADDRESS, REG_FEMB3_RD_Port);
            }
            catch (Exception e)
            {
                return ("rd_port failed " + e.ToString());
            }
            try
            {
                RDBKREG_FEMB3 = new UdpClient(REG_FEMB3_RDBK_Port);
                RDBKREG_FEMB3.Client.ReceiveTimeout = 1000;
                RDBKREG_FEMB3_EP = new IPEndPoint(IPAddress.Any, REG_FEMB3_RDBK_Port);
            }
            catch (Exception e)
            {
                return ("rdbk_port failed " + e.ToString());
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
                   return ("clean up failed" + e.ToString());
               }
               try
               {
                   RDREG_UDP.Close();
               }
               catch (Exception e)
               {
                   return ("clean up failed" + e.ToString());
               }
               try
               {
                   RDBKREG_UDP.Close();
               }
               catch (Exception e)
               {
                   return ("clean up failed" + e.ToString());
               }
               try
               {
                   HSDATA_UDP.Close();
               }
               catch (Exception e)
               {
                   return ("clean up failed" + e.ToString());
               }


               //FEMB 0

               try
               {
                   WRREG_FEMB0.Close();
               }
               catch (Exception e)
               {
                   return ("clean up failed" + e.ToString());
               }


               try
               {
                   RDREG_FEMB0.Close();

               }
               catch (Exception e)
               {
                   return ("clean up failed" + e.ToString());
               }



               try
               {
                   RDBKREG_FEMB0.Close();
               }
               catch (Exception e)
               {
                   return ("clean up failed" + e.ToString());
               }



               //FEMB 1

               try
               {
                   WRREG_FEMB1.Close();
               }
               catch (Exception e)
               {
                   return ("clean up failed" + e.ToString());
               }


               try
               {
                   RDREG_FEMB1.Close();

               }
               catch (Exception e)
               {
                   return ("clean up failed" + e.ToString());
               }



               try
               {
                   RDBKREG_FEMB1.Close();
               }
               catch (Exception e)
               {
                   return ("clean up failed" + e.ToString());
               }

               //FEMB 2

               try
               {
                   WRREG_FEMB2.Close();
               }
               catch (Exception e)
               {
                   return ("clean up failed" + e.ToString());
               }


               try
               {
                   RDREG_FEMB2.Close();

               }
               catch (Exception e)
               {
                   return ("clean up failed" + e.ToString());
               }



               try
               {
                   RDBKREG_FEMB2.Close();
               }
               catch (Exception e)
               {
                   return ("clean up failed" + e.ToString());
               }

               //FEMB 3

               try
               {
                   WRREG_FEMB3.Close();
               }
               catch (Exception e)
               {
                   return ("clean up failed" + e.ToString());
               }


               try
               {
                   RDREG_FEMB3.Close();

               }
               catch (Exception e)
               {
                   return ("clean up failed" + e.ToString());
               }



               try
               {
                   RDBKREG_FEMB3.Close();
               }
               catch (Exception e)
               {
                   return ("clean up failed" + e.ToString());
               }

            try
            {
                HSDATA_Thread.Abort();
            }
            catch (Exception e)
            {
                return ("hs thread clean up failed" + e.ToString());
            }

            return ("ok");
        }


        private void HS_Thread()
        {

            uint chn,chp,PDO;

            uint i = 0;
            uint j = 0;
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



                if (receive_byte_array != null)
                {

                    try
                    {
                        if ((LV_READY == false) && (index_G > num_samp))
                        {

                            num_samp = number_of_samples;
                            receive_word_array = new UInt32[num_samp + 1];
                            index_G = 0;
                        }

                    }
                    catch (Exception e)
                    {
                        SYS_ERROR++;
                    }

                    try
                    {

                        for (i = 16; i < (receive_byte_array.Length); i = i + 26)
                        {
                            if ((i + 25) >= (receive_byte_array.Length)) break;
                             data_in_raw = (UInt16)((uint)receive_byte_array[i] << 8 | (uint)receive_byte_array[i + 1]);
 
                            j = 2;
                            CHN_DATA[7] =  (UInt16) (((uint)receive_byte_array[j+i]     << 8 | (uint)receive_byte_array[j+i + 1]) & 0xfff);
                            CHN_DATA[6] =  (UInt16) (((uint)receive_byte_array[j+i+3]   << 4 | (uint)receive_byte_array[j+i] >> 4) & 0xfff);
                            CHN_DATA[5] =  (UInt16) (((uint)receive_byte_array[j+i+5]   << 8 | (uint)receive_byte_array[j+i+2]) & 0xfff);
                            CHN_DATA[4] =  (UInt16)( ((uint)receive_byte_array[j+i + 4] << 4 | (uint)receive_byte_array[j+i + 5] >> 4) & 0xfff);
                            j=2+6;
                            CHN_DATA[3] = (UInt16)(((uint)receive_byte_array[j + i] << 8 | (uint)receive_byte_array[j + i + 1]) & 0xfff);
                            CHN_DATA[2] = (UInt16)(((uint)receive_byte_array[j + i + 3] << 4 | (uint)receive_byte_array[j + i] >> 4) & 0xfff);
                            CHN_DATA[1] = (UInt16)(((uint)receive_byte_array[j + i + 5] << 8 | (uint)receive_byte_array[j + i + 2]) & 0xfff);
                            CHN_DATA[0] = (UInt16)(((uint)receive_byte_array[j + i + 4] << 4 | (uint)receive_byte_array[j + i + 5] >> 4) & 0xfff);
                            j=2+12;
                            CHN_DATA[15] =  (UInt16) (((uint)receive_byte_array[j+i]     << 8 | (uint)receive_byte_array[j+i + 1]) & 0xfff);
                            CHN_DATA[14] =  (UInt16) (((uint)receive_byte_array[j+i+3]   << 4 | (uint)receive_byte_array[j+i] >> 4) & 0xfff);
                            CHN_DATA[13] =  (UInt16) (((uint)receive_byte_array[j+i+5]   << 8 | (uint)receive_byte_array[j+i+2]) & 0xfff);
                            CHN_DATA[12] =  (UInt16)( ((uint)receive_byte_array[j+i + 4] << 4 | (uint)receive_byte_array[j+i + 5] >> 4) & 0xfff);
                            j = 2 + 18;
                            CHN_DATA[11] = (UInt16)(((uint)receive_byte_array[j + i] << 8 | (uint)receive_byte_array[j + i + 1]) & 0xfff);
                            CHN_DATA[10] = (UInt16)(((uint)receive_byte_array[j + i + 3] << 4 | (uint)receive_byte_array[j + i] >> 4) & 0xfff);
                            CHN_DATA[9] = (UInt16)(((uint)receive_byte_array[j + i + 5] << 8 | (uint)receive_byte_array[j + i + 2]) & 0xfff);
                            CHN_DATA[8] = (UInt16)(((uint)receive_byte_array[j + i + 4] << 4 | (uint)receive_byte_array[j + i + 5] >> 4) & 0xfff);

                            if (index_G == num_samp)
                            {

                                receive_word_array[index_G] = CHN_DATA[CHN_SEL];
                                index_G++;
                                LV_READY = true;
                            }
                            else if (index_G < num_samp)
                            {
                                receive_word_array[index_G] = CHN_DATA[CHN_SEL];
                                index_G++;
                                LV_READY = false;
                            }

                            for(j=0;j<=15;j++)
                            {
                                if ((HIST_samples > ADC_samples[j]) || (HIST_samples == 0))
                                {
                                    ADC_samples[j]++;
                                    ADC_hist[j, CHN_DATA[j]]++;
                                    ADC_avg[j] += CHN_DATA[j];
                                    ADC_savg[j] += CHN_DATA[j] * CHN_DATA[j];
                                }
                            }

                            List_Mode[3] = CHN_DATA[CHN_SEL];

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
        }


        public void RESET_STAT()
        {

    //          Array.Clear(PDO_avg,0,2405);

        }




        public string REG_WR(UInt32 DATA,UInt16 ADDRESS)
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




        public  string  REG_RD(out UInt32 DATA ,Int16 ADDRESS)
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



        public string REG_BLK_RD(out UInt32[] DATA, Int16 ADDRESS)
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



        public string REG_FEMB_WR(UInt32 FEMB_ADDR,UInt32 DATA, UInt16 ADDRESS)
        {

            byte[] packet = { 0xDE, 0xAD, 0xBE, 0xEF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff };

            packet[4] = (byte)((ADDRESS >> 8) & 0xff);
            packet[5] = (byte)(ADDRESS & 0xff);
            packet[6] = (byte)((DATA >> 24) & 0xff);
            packet[7] = (byte)((DATA >> 16) & 0xff);
            packet[8] = (byte)((DATA >> 8) & 0xff);
            packet[9] = (byte)(DATA & 0xff);

            try
            {
                if (FEMB_ADDR == 0) 
                    WRREG_FEMB0.Send(packet, 12, WRREG_FEMB0_EP);
                else if (FEMB_ADDR == 1)
                    WRREG_FEMB1.Send(packet, 12, WRREG_FEMB1_EP);
                else if (FEMB_ADDR == 2)
                    WRREG_FEMB2.Send(packet, 12, WRREG_FEMB2_EP);
                else if (FEMB_ADDR == 3)
                    WRREG_FEMB3.Send(packet, 12, WRREG_FEMB3_EP);
                else
                    WRREG_FEMB0.Send(packet, 12, WRREG_FEMB0_EP);
                Thread.Sleep(1);
            }
            catch (Exception e)
            {
                return ("clean up failed2" + e.ToString());
            }
            return ("OK");

        }



        public string REG_FEMB_RD(UInt32 FEMB_ADDR, out UInt32 DATA, Int16 ADDRESS)
        {

            byte[] packet = { 0xDE, 0xAD, 0xBE, 0xEF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff };
            packet[4] = (byte)((ADDRESS >> 8) & 0xff);
            packet[5] = (byte)(ADDRESS & 0xff);
            packet[9] = 0x00;
            try
            {
                if (FEMB_ADDR == 0) 
                    RDREG_FEMB0.Send(packet, 12, RDREG_FEMB0_EP);
                else if (FEMB_ADDR == 1)
                    RDREG_FEMB1.Send(packet, 12, RDREG_FEMB1_EP);
                else if (FEMB_ADDR == 2)
                    RDREG_FEMB2.Send(packet, 12, RDREG_FEMB2_EP);
                else if (FEMB_ADDR == 3)
                    RDREG_FEMB3.Send(packet, 12, RDREG_FEMB3_EP);
                else
                    RDREG_FEMB0.Send(packet, 12, RDREG_FEMB0_EP);

                if (FEMB_ADDR == 0)
                    packet = RDBKREG_FEMB0.Receive(ref RDBKREG_FEMB0_EP);
                else if (FEMB_ADDR == 1)
                    packet = RDBKREG_FEMB1.Receive(ref RDBKREG_FEMB1_EP);
                else if (FEMB_ADDR == 2)
                    packet = RDBKREG_FEMB2.Receive(ref RDBKREG_FEMB2_EP);
                else if (FEMB_ADDR == 3)
                    packet = RDBKREG_FEMB3.Receive(ref RDBKREG_FEMB3_EP);
                else
                    packet = RDBKREG_FEMB0.Receive(ref RDBKREG_FEMB0_EP);

                DATA = ((UInt32)packet[2] << 24) | ((UInt32)packet[3] << 16) | ((UInt32)packet[4] << 8) | (UInt32)packet[5];
                return ("ok V3");
            }
            catch (Exception e)
            {
                DATA = 0;
                return ("read failed" + e.ToString());
            }
        }



        private void HS_Thread_UDP_Receive()
        {
            byte[] rbyte_array = null;
            uint cur_PKT = 0, last_PKT = 0;
            while (HS_THREAD_running)
            {
                try
                {
                    rbyte_array = HSDATA_UDP.Receive(ref HSDATA_EP);
                    if (rbyte_array != null)
                    {
                        RB_PUSH(rbyte_array);

                        cur_PKT =(uint)(rbyte_array[2] << 8 | rbyte_array[3]);


                        List_Mode[0] = (UInt64)rbyte_array.Length;
                        List_Mode[1] = 0;

                        List_Mode[6] = (UInt64)last_PKT;
                        List_Mode[7] = (UInt64)cur_PKT;
                        if (cur_PKT != (last_PKT + 1))
                        {
                            DROP_PKT++;
                        }
                        List_Mode[8] = (UInt64)DROP_PKT;
                        last_PKT = cur_PKT;
                    }
                }
                catch (Exception e)
                {
                    SYS_ERROR++;
                }

            }
        }



        private void HS_Thread_Process()
        {
            byte[] rbyte_array = null;
            uint chn, chp, PDO;

            uint i = 0;
            uint j = 0;
            uint k = 0;
            uint data_in;
            UInt16 data_in_raw;
            uint CHN = 0;
            UInt32 num_samp = number_of_samples;


            index_G = 0;
            LV_READY = false;
            num_samp = number_of_samples;
            receive_word_array = new UInt32[num_samp + 1];

            while (HS_THREAD_running)
            {
                try
                {
                    rbyte_array = RB_POP();
                }
                catch (Exception e)
                {
                    SYS_ERROR++;
                }

                if (rbyte_array != null)
                {

                    try
                    {
                        if ((LV_READY == false) && (index_G > num_samp))
                        {

                            num_samp = number_of_samples;
                            receive_word_array = new UInt32[num_samp + 1];
                            index_G = 0;
                        }

                    }
                    catch (Exception e)
                    {
                        SYS_ERROR++;
                    }

                    try
                    {

                        for (i = 16; i < (rbyte_array.Length); i = i + 26)
                        {
                            if ((i + 25) >= (rbyte_array.Length)) break;
                            data_in_raw = (UInt16)((uint)rbyte_array[i] << 8 | (uint)rbyte_array[i + 1]);

                            j = 2;
                            CHN_DATA[7] = (UInt16)(((uint)rbyte_array[j + i] << 8 | (uint)rbyte_array[j + i + 1]) & 0xfff);
                            CHN_DATA[6] = (UInt16)(((uint)rbyte_array[j + i + 3] << 4 | (uint)rbyte_array[j + i] >> 4) & 0xfff);
                            CHN_DATA[5] = (UInt16)(((uint)rbyte_array[j + i + 5] << 8 | (uint)rbyte_array[j + i + 2]) & 0xfff);
                            CHN_DATA[4] = (UInt16)(((uint)rbyte_array[j + i + 4] << 4 | (uint)rbyte_array[j + i + 5] >> 4) & 0xfff);
                            j = 2 + 6;
                            CHN_DATA[3] = (UInt16)(((uint)rbyte_array[j + i] << 8 | (uint)rbyte_array[j + i + 1]) & 0xfff);
                            CHN_DATA[2] = (UInt16)(((uint)rbyte_array[j + i + 3] << 4 | (uint)rbyte_array[j + i] >> 4) & 0xfff);
                            CHN_DATA[1] = (UInt16)(((uint)rbyte_array[j + i + 5] << 8 | (uint)rbyte_array[j + i + 2]) & 0xfff);
                            CHN_DATA[0] = (UInt16)(((uint)rbyte_array[j + i + 4] << 4 | (uint)rbyte_array[j + i + 5] >> 4) & 0xfff);
                            j = 2 + 12;
                            CHN_DATA[15] = (UInt16)(((uint)rbyte_array[j + i] << 8 | (uint)rbyte_array[j + i + 1]) & 0xfff);
                            CHN_DATA[14] = (UInt16)(((uint)rbyte_array[j + i + 3] << 4 | (uint)rbyte_array[j + i] >> 4) & 0xfff);
                            CHN_DATA[13] = (UInt16)(((uint)rbyte_array[j + i + 5] << 8 | (uint)rbyte_array[j + i + 2]) & 0xfff);
                            CHN_DATA[12] = (UInt16)(((uint)rbyte_array[j + i + 4] << 4 | (uint)rbyte_array[j + i + 5] >> 4) & 0xfff);
                            j = 2 + 18;
                            CHN_DATA[11] = (UInt16)(((uint)rbyte_array[j + i] << 8 | (uint)rbyte_array[j + i + 1]) & 0xfff);
                            CHN_DATA[10] = (UInt16)(((uint)rbyte_array[j + i + 3] << 4 | (uint)rbyte_array[j + i] >> 4) & 0xfff);
                            CHN_DATA[9] = (UInt16)(((uint)rbyte_array[j + i + 5] << 8 | (uint)rbyte_array[j + i + 2]) & 0xfff);
                            CHN_DATA[8] = (UInt16)(((uint)rbyte_array[j + i + 4] << 4 | (uint)rbyte_array[j + i + 5] >> 4) & 0xfff);

                            if (index_G == num_samp)
                            {

                                receive_word_array[index_G] = CHN_DATA[CHN_SEL];
                                index_G++;
                                LV_READY = true;
                            }
                            else if (index_G < num_samp)
                            {
                                receive_word_array[index_G] = CHN_DATA[CHN_SEL];
                                index_G++;
                                LV_READY = false;
                            }

                            for (j = 0; j <= 15; j++)
                            {
                                if ((HIST_samples > ADC_samples[j]) || (HIST_samples == 0))
                                {
                                    ADC_samples[j]++;
                                    ADC_hist[j, CHN_DATA[j]]++;
                                    ADC_avg[j] += CHN_DATA[j];
                                    ADC_savg[j] += CHN_DATA[j] * CHN_DATA[j];
                                }
                            }

                            List_Mode[3] = CHN_DATA[CHN_SEL];
                            for (k=0;k<=15;k++)
                                if ((SFile != null) && (Store_Data == true))
                                {
                                    SFile.Write(CHN_DATA[k]);
                                }

                        }
                    }
                    catch (Exception e)
                    {
                        List_Mode[5] = (UInt64)rbyte_array.Length;
                        List_Mode[6] = i;
                        SYS_ERROR++;
                    }
                }
                else
                    Thread.Sleep(1);
            }
        }


        private void HS_Thread_Process_2()
        {
            byte[] rbyte_array = null;
            uint chn, chp, PDO;

            uint i = 0;
            uint j = 0;
            uint k = 0;
            uint data_in;
            UInt16 data_in_raw;
            uint CHN = 0;
            UInt32 num_samp = number_of_samples;


            index_G = 0;
            LV_READY = false;
            num_samp = number_of_samples;
            receive_word_array = new UInt32[num_samp + 1];

            while (HS_THREAD_running)
            {
                try
                {
                    rbyte_array = RB_POP();
                }
                catch (Exception e)
                {
                    SYS_ERROR++;
                }

                if (rbyte_array != null)
                {

                    try
                    {
                        if ((LV_READY == false) && (index_G > num_samp))
                        {

                            num_samp = number_of_samples;
                            receive_word_array = new UInt32[num_samp + 1];
                            index_G = 0;
                        }

                    }
                    catch (Exception e)
                    {
                        SYS_ERROR++;
                    }

                    try
                    {

                        for (i = 16; i < (rbyte_array.Length); i = i + 26)
                        {
                            if ((i + 25) >= (rbyte_array.Length)) break;
                            data_in_raw = (UInt16)((uint)rbyte_array[i] << 8 | (uint)rbyte_array[i + 1]);

                            j = 2;
                            CHN_DATA[7] = (UInt16)(((uint)rbyte_array[j + i] << 8 | (uint)rbyte_array[j + i + 1]) & 0xfff);
                            CHN_DATA[6] = (UInt16)(((uint)rbyte_array[j + i + 3] << 4 | (uint)rbyte_array[j + i] >> 4) & 0xfff);
                            CHN_DATA[5] = (UInt16)(((uint)rbyte_array[j + i + 5] << 8 | (uint)rbyte_array[j + i + 2]) & 0xfff);
                            CHN_DATA[4] = (UInt16)(((uint)rbyte_array[j + i + 4] << 4 | (uint)rbyte_array[j + i + 5] >> 4) & 0xfff);
                            j = 2 + 6;
                            CHN_DATA[3] = (UInt16)(((uint)rbyte_array[j + i] << 8 | (uint)rbyte_array[j + i + 1]) & 0xfff);
                            CHN_DATA[2] = (UInt16)(((uint)rbyte_array[j + i + 3] << 4 | (uint)rbyte_array[j + i] >> 4) & 0xfff);
                            CHN_DATA[1] = (UInt16)(((uint)rbyte_array[j + i + 5] << 8 | (uint)rbyte_array[j + i + 2]) & 0xfff);
                            CHN_DATA[0] = (UInt16)(((uint)rbyte_array[j + i + 4] << 4 | (uint)rbyte_array[j + i + 5] >> 4) & 0xfff);
                            j = 2 + 12;
                            CHN_DATA[15] = (UInt16)(((uint)rbyte_array[j + i] << 8 | (uint)rbyte_array[j + i + 1]) & 0xfff);
                            CHN_DATA[14] = (UInt16)(((uint)rbyte_array[j + i + 3] << 4 | (uint)rbyte_array[j + i] >> 4) & 0xfff);
                            CHN_DATA[13] = (UInt16)(((uint)rbyte_array[j + i + 5] << 8 | (uint)rbyte_array[j + i + 2]) & 0xfff);
                            CHN_DATA[12] = (UInt16)(((uint)rbyte_array[j + i + 4] << 4 | (uint)rbyte_array[j + i + 5] >> 4) & 0xfff);
                            j = 2 + 18;
                            CHN_DATA[11] = (UInt16)(((uint)rbyte_array[j + i] << 8 | (uint)rbyte_array[j + i + 1]) & 0xfff);
                            CHN_DATA[10] = (UInt16)(((uint)rbyte_array[j + i + 3] << 4 | (uint)rbyte_array[j + i] >> 4) & 0xfff);
                            CHN_DATA[9] = (UInt16)(((uint)rbyte_array[j + i + 5] << 8 | (uint)rbyte_array[j + i + 2]) & 0xfff);
                            CHN_DATA[8] = (UInt16)(((uint)rbyte_array[j + i + 4] << 4 | (uint)rbyte_array[j + i + 5] >> 4) & 0xfff);

                            if (index_G == num_samp)
                            {

                                receive_word_array[index_G] = CHN_DATA[CHN_SEL];
                                index_G++;
                                LV_READY = true;
                            }
                            else if (index_G < num_samp)
                            {
                                receive_word_array[index_G] = CHN_DATA[CHN_SEL];
                                index_G++;
                                LV_READY = false;
                            }

                            for (j = 0; j <= 15; j++)
                            {
                                if ((HIST_samples > ADC_samples[j]) || (HIST_samples == 0))
                                {
                                    ADC_samples[j]++;
                                    ADC_hist[j, CHN_DATA[j]]++;
                                    ADC_avg[j] += CHN_DATA[j];
                                    ADC_savg[j] += CHN_DATA[j] * CHN_DATA[j];
                                }
                            }

                            List_Mode[3] = CHN_DATA[CHN_SEL];
                        }
                        if ((SFile != null) && (Store_Data == true))
                        {
                            SFile.Write(rbyte_array);
                        }


                    }
                    catch (Exception e)
                    {
                        List_Mode[5] = (UInt64)rbyte_array.Length;
                        List_Mode[6] = i;
                        SYS_ERROR++;
                    }
                }
                else
                    Thread.Sleep(1);
            }
        }

        public string RB_Create(int RING_BUFF_DEPTH)
        {
            try
            {
                Ring_BUF_MAX = RING_BUFF_DEPTH;
                Ring_BUF_HEAD = 0;
                Ring_BUF_TAIL = 0;
                RING_BUF = new byte[RING_BUFF_DEPTH][];
            }
            catch (Exception e)
            {
                return ("Ring buffer aloc failed " + e.ToString());
            }
            return ("OK");
        }


        public int RB_USED()
        {
            try
            {
                if ((Ring_BUF_HEAD + 1) == Ring_BUF_TAIL)  // RING buffer is full
                   return Ring_BUF_MAX;
                else if (Ring_BUF_HEAD == Ring_BUF_TAIL)   // RING buffer is empty
                    return 0;
                else if (Ring_BUF_HEAD > Ring_BUF_TAIL)
                    return (Ring_BUF_HEAD - Ring_BUF_TAIL);
                else
                {
                   return  (Ring_BUF_MAX - Ring_BUF_HEAD + Ring_BUF_TAIL); 
                }
            }
            catch (Exception e)
            {
                return (-1);
            }
        }

        public int RB_PUSH(byte[] Buffer_In)
        {

            int next = 0;

            try
            {
                next = Ring_BUF_HEAD + 1;
                if (next >= Ring_BUF_MAX)  // 
                    next = 0;
                List_Mode[4] = 0;
                if (next == Ring_BUF_TAIL)  // RING buffer is full
                {
                    List_Mode[4] = 0xffffff;
                    return -1;      // quit with an error
                }
                RING_BUF[Ring_BUF_HEAD] = Buffer_In;
                Ring_BUF_HEAD = next;
                return 0;
            }
            catch (Exception e)
            {
                return (-1);
            }
            return (0);
        }

        public byte[] RB_POP()
        {
            byte[] Buffer_OUT = null;
            int next = 0;
            try
            {
                List_Mode[5] = 0;
                if (Ring_BUF_HEAD == Ring_BUF_TAIL)
                {
                    List_Mode[5] = 0xeeeeeeee;
                    return null;  // quit with an error
                }
                Buffer_OUT = RING_BUF[Ring_BUF_TAIL];
                RING_BUF[Ring_BUF_TAIL] = null;
                next = Ring_BUF_TAIL + 1;
                if (next >= Ring_BUF_MAX)
                    next = 0;
                Ring_BUF_TAIL = next;
                return (Buffer_OUT);
            }
            catch (Exception e)
            {
                return (null);
            }
            return (null);
        }
       
    }
}
