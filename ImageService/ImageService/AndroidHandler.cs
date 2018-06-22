using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImageService
{
    class AndroidHandler : IClientHandler
    {
        private NetworkStream stream;
        private BinaryReader reader;
        private BinaryWriter writer;
        public bool hasSentInitial = false;
        private bool sentSettings = false;
        private bool isListening = false;
        private int id;
        private static Mutex mutex;
        public EventHandler<ImageRecievedEventArgs> gotNewMessage;
        public EventHandler<MessageRecievedEventArgs> InitialWritingHappened;
        public EventHandler<ClientEventArgs> settingsWritten;
        public AndroidHandler(Mutex mut, int id)
        {
            this.id = id;
            mutex = mut;
        }
        public void setStream(TcpClient client)
        {
            this.stream = client.GetStream();
            this.reader = new BinaryReader(stream);
            this.writer = new BinaryWriter(stream);
        }

        public void HandleClient(TcpClient client)
        {
            return;
        }


        public void Listen()
        {
            if (isListening)
            {
                return;
            }
            isListening = true;
            new Task(() =>
            {
                while (true)
                {
                    try
                    {
                        byte[] numOfBytes = this.reader.ReadBytes(4);
                        if (BitConverter.IsLittleEndian)
                        {
                            Array.Reverse(numOfBytes);
                        }
                        int size = BitConverter.ToInt32(numOfBytes, 0);
                        
                        
                        byte[] image = this.reader.ReadBytes(size);
                        byte[] sizeOfName = this.reader.ReadBytes(4);
                        if (BitConverter.IsLittleEndian)
                        {
                            Array.Reverse(sizeOfName);
                        }
                        int sizeTwo = BitConverter.ToInt32(sizeOfName, 0);
                        byte[] nameInBytes = this.reader.ReadBytes(sizeTwo);
                        String name = System.Text.Encoding.UTF8.GetString(nameInBytes, 0, sizeTwo);
                        //MessageRecievedEventArgs args = this.getArgsFromBytes(image);
                        ImageRecievedEventArgs args = new ImageRecievedEventArgs(image, name);

                        if (this.gotNewMessage != null)
                        {
                            this.gotNewMessage.Invoke(this, args);
                        }
                    }
                    catch (Exception e)
                    {
                        
                    }
                }
            }).Start();
        }

        private MessageRecievedEventArgs getArgsFromBytes(byte[] message)
        {
            /*
            ImageConverter imageConverter = new ImageConverter();
            Bitmap bm = (Bitmap)imageConverter.ConvertFrom(message);

            if (bm != null && (bm.HorizontalResolution != (int)bm.HorizontalResolution ||
                               bm.VerticalResolution != (int)bm.VerticalResolution))
            {
                // Correct a strange glitch that has been observed in the test program when converting 
                //  from a PNG file image created by CopyImageToByteArray() - the dpi value "drifts" 
                //  slightly away from the nominal integer value
                bm.SetResolution((int)(bm.HorizontalResolution + 0.5f),
                                 (int)(bm.VerticalResolution + 0.5f));
            }
            */
            String imageString = Convert.ToBase64String(message);
            //System.Text.Encoding.ASCII.GetString(imageString);
            //System.Text.Encoding.GetEncoding("shift_jis").GetString(imageString);
            //System.Text.Encoding.GetEncoding("euc-jp").GetString(data);
            //System.Text.Encoding.Unicode.GetString(data);
            //System.Text.Encoding.UTF8.GetString(data);
            return new MessageRecievedEventArgs(0, imageString);
        }
    }
}
