using Magellanic.I2C;
using System;

namespace Magellanic.Sensors.DS1307
{
    public class DS1307 : AbstractI2CDevice
    {
        private byte I2C_ADDRESS = 0x68;

        public override byte GetI2cAddress()
        {
            return I2C_ADDRESS;
        }

        public override byte[] GetDeviceId()
        {
            throw new NotImplementedException("This device does not have a unique device identifier.");
        }

        public DateTime GetCurrentTime()
        {
            byte[] readBuffer = new byte[7];

            this.Slave.WriteRead(new byte[] { 0x00 }, readBuffer);

            return ConvertByteBufferToDateTime(readBuffer);
        }

        public void SetDateTime(DateTime dateTime)
        {
            this.Slave.Write(ConvertTimeToByteArray(dateTime));
        }

        private byte[] ConvertTimeToByteArray(DateTime dateTime)
        {
            var dateTimeByteArray = new byte[8];

            dateTimeByteArray[0] = 0;
            dateTimeByteArray[1] = DecimalToBinaryCodedDecimal(dateTime.Second);
            dateTimeByteArray[2] = DecimalToBinaryCodedDecimal(dateTime.Minute);
            dateTimeByteArray[3] = DecimalToBinaryCodedDecimal(dateTime.Hour);
            dateTimeByteArray[4] = DecimalToBinaryCodedDecimal((byte)dateTime.DayOfWeek);
            dateTimeByteArray[5] = DecimalToBinaryCodedDecimal(dateTime.Day);
            dateTimeByteArray[6] = DecimalToBinaryCodedDecimal(dateTime.Month);
            dateTimeByteArray[7] = DecimalToBinaryCodedDecimal(dateTime.Year - 2000);
            
            return dateTimeByteArray;
        }

        private DateTime ConvertByteBufferToDateTime(byte[] dateTimeBuffer)
        {
            var second = BinaryCodedDecimalToDecimal(dateTimeBuffer[0]);
            var minute = BinaryCodedDecimalToDecimal(dateTimeBuffer[1]);
            var hour = BinaryCodedDecimalToDecimal(dateTimeBuffer[2]);
            var dayofWeek = BinaryCodedDecimalToDecimal(dateTimeBuffer[3]);
            var day = BinaryCodedDecimalToDecimal(dateTimeBuffer[4]);
            var month = BinaryCodedDecimalToDecimal(dateTimeBuffer[5]);
            var year = 2000 + BinaryCodedDecimalToDecimal(dateTimeBuffer[6]);

            return new DateTime(year, month, day, hour, minute, second);
        }

        private byte DecimalToBinaryCodedDecimal(int value)
        {
            var multipleOfOne = value % 10;
            var multipleOfTen = value / 10;

            // convert to nibbles
            var lowerNibble = multipleOfOne;
            var upperNibble = multipleOfTen << 4;

            return (byte)(lowerNibble + upperNibble);
        }

        private int BinaryCodedDecimalToDecimal(int value)
        {
            var lowerNibble = value;
            var upperNibble = value >> 4;

            var multipleOfOne = value & 0x0F;
            var multipleOfTen = upperNibble * 10;

            return multipleOfOne + multipleOfTen;
        }
    }
}
