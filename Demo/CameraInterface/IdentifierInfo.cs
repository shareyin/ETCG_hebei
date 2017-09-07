using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ETCF
{
    public class IdentifierInfo//识别器信息
    {
        public string PlateNo = "";                                //Plate
        public byte[] PlateNoBuf = new byte[20];                   //Buffer for plate
        public byte[] VehImage = new byte[524288];          //buffer for Vehicle image, max 512k
        public Int32 VehImagelen = 524288;
        public byte[] PlateImage = new byte[102400];        //buffer for plate image, max 100k
        public byte[] BinaryImage = new byte[102400];       //buffer for binary plate image, max 100k
        public byte[] FarBigImage = new byte[524288];       //buffer for far big image, max 512k
        
        public int VehImageLen = 0;                         //length of the Vehicle image
        public int PlateImageLen = 0;                       //length of the plate image
        public int BinaryImageLen = 0;                      //length of the binary plate image 
        public Int32 FarBigImageLen = 0;                      //length of the far bif image
    }
}
